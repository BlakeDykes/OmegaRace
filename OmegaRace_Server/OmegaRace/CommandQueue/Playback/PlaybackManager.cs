using System;
using System.Diagnostics;
using System.IO;

namespace OmegaRace
{
    class PlaybackManager
    {
        //private const int HEADER_ALIGNMENT = 4;

        private static PlaybackManager instance = null;
        protected const string RECORD_FILE_DIR = "../bin/Debug/QueueRecordings/";

        private string _FilePath;
        private BinaryWriter _Writer;
        private BinaryReader _Reader;
        private FileStream _Stream;
        private MessageHandler _MessageHandler;
        private PlaybackHeader _Header;
        private PlaybackReader _PlaybackReader;
        bool bRecording;

        public static void Initialize()
        {
            if(instance == null)
            {
                instance = new PlaybackManager();
            }
        }

        private PlaybackManager()
        {
            Directory.CreateDirectory(RECORD_FILE_DIR);
            _Header = new PlaybackHeader();
            bRecording = false;
        }

        public static PlaybackRecorder CreateRecorder(QUEUE_NAME name, RECORDER_TYPE type)
        {
            switch (type)
            {
                case RECORDER_TYPE.NULL:       
                    return new PlaybackRecorder_Null();

                case RECORDER_TYPE.BASIC:
                {
                    PlaybackManager inst = GetInstance();
                    inst._Header.AddQueue(name);
                    return new PlaybackRecorder_Basic(GetInstance());
                }
                
                default: return null;
            }
        }

        public void StartRecord()
        {
            if (!bRecording)
            {
                bRecording = true;

                _FilePath = RECORD_FILE_DIR + NetworkEnvManager.GetEnvName() + "__" + DateTime.Now.ToString("HH-mm-ss-MM-dd-yyyy") + ".bin";
                _MessageHandler = new MessageHandler();
                _Stream = File.OpenWrite(_FilePath);
                _Writer = new BinaryWriter(_Stream);

                _Header.SetStart(TimeManager.GetCurrentTime());
                _Writer.Seek(_Header.GetSize(), SeekOrigin.Begin);
            }

        }

        public static void StopRecord()
        {
            PlaybackManager inst = GetInstance();

            if (inst.bRecording)
            {
                inst.bRecording = false;

                inst._Header.SetPlayers(GameSceneCollection.ScenePlay.PlayerMgr.PlayerData);

                inst._Writer.Seek(0, SeekOrigin.Begin);
                inst._Header.Serialize(ref inst._Writer);

                inst._Writer.Flush();
                inst._Writer.Close();

                Debug.Print("------ Total Game Time - {0}", TimeManager.GetCurrentTime() - inst._Header.StartTime);
            }
        }

        /// <summary>
        /// Finds most current playback file and initializes Stream/BinaryReader
        /// </summary>
        /// <param name="envType"></param>
        /// <param name="player"></param>
        public static void StartPlayback(NetworkEnv.TYPE envType, PlayerName player)
        {
            PlaybackManager inst = GetInstance();

            string fileSearchString = "*" + NetworkEnv.Stringify(envType) + "__" + PlayerData.Stringify(player) + "*";

            DirectoryInfo di = new DirectoryInfo(RECORD_FILE_DIR);
            FileInfo mostCurrentFile = null;
            DateTime latest = DateTime.MinValue;

            foreach (FileInfo f in di.EnumerateFiles(fileSearchString))
            {
                if(f.LastWriteTime > latest)
                {
                    mostCurrentFile = f;
                }
            }

            Debug.Assert(mostCurrentFile != null);

            inst._Stream = File.OpenRead(mostCurrentFile.FullName);
            inst._Reader = new BinaryReader(inst._Stream);

            inst._Header.Deserialize(ref inst._Reader);

            GameSceneCollection.ScenePlay.InitializePlayback(inst._Header);
            inst._PlaybackReader = new PlaybackReader(inst._Header, ref inst._Reader);
            inst._PlaybackReader.RouteCommandsIn();
        }

        public void Record(QUEUE_NAME execQueue, Command c)
        {
            if (bRecording)
            {
                _MessageHandler.SetData(c);

                _Writer.Write(TimeManager.GetCurrentTime());
                _Writer.Write(_MessageHandler.Data);

                _Stream.Flush();

                _Header.IncrementCommandCount(execQueue);
            }
        }

        private static PlaybackManager GetInstance()
        {
            Debug.Assert(instance != null);

            return instance;
        }
    }
}
