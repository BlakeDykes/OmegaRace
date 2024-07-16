using System;
using System.Diagnostics;

namespace OmegaRace
{
    public class NetworkEnvManager
    {
        static private NetworkEnvManager instance = null;
        static public int SERVER_PORT = 14240;
        static public int CLIENT_PORT = 0;
        static public string APP_ID = "Omega Race";

        protected NetworkEnv Env;

        private QueueBase _InQueue;
        private QueueBase _OutQueue;

        protected QueueBase InQueue { get => _InQueue; set => _InQueue = value; }
        protected QueueBase OutQueue { get => _OutQueue; set => _OutQueue = value; }

        public static void Initialize()
        {
            if(instance == null)
            {
                instance = new NetworkEnvManager();
            }
        }

        private NetworkEnvManager()
        {
            Env = new LocalEnvManager();
            _InQueue = new NullQueue();
            _OutQueue = new NullQueue();
        }

        public static void InitializeServerEnv()
        {
            NetworkEnvManager inst = GetInstance();
            inst.Env = new ServerManager();

            inst._InQueue = CommandQueueManager.Add(QUEUE_NAME.NETWORK_IN, QUEUE_TYPE.NETWORK, RECORDER_TYPE.NULL);
            inst._OutQueue = CommandQueueManager.Add(QUEUE_NAME.NETWORK_OUT, QUEUE_TYPE.NETWORK, RECORDER_TYPE.NULL);
            inst._InQueue.Env = inst.Env;
            inst._OutQueue.Env = inst.Env;

            inst._InQueue.StartRecord();
        }

        public static void InitializeLocalEnv()
        {
            NetworkEnvManager inst = GetInstance();
            inst.Env = new LocalEnvManager();

            inst._InQueue = CommandQueueManager.Add(QUEUE_NAME.NETWORK_IN, QUEUE_TYPE.LOCAL, RECORDER_TYPE.NULL);
            inst._OutQueue = CommandQueueManager.Add(QUEUE_NAME.NETWORK_OUT, QUEUE_TYPE.LOCAL, RECORDER_TYPE.NULL);
            inst._InQueue.Env = inst.Env;
            inst._OutQueue.Env = inst.Env;

            inst._InQueue.StartRecord();
        }

        public static NetworkEnv GetEnv()
        {
            return GetInstance().Env;
        }

        public static void Update()
        {
            NetworkEnvManager inst = GetInstance();

            ScreenLog.Add(Colors.LawnGreen, NetworkEnv.Stringify(inst.Env.EnvType));

            inst.Env.Update();

            inst.InQueue.ProcessIn();
            inst.OutQueue.ProcessOut();

            inst.Env.UpdateState();
        }

        public static void Shutdown()
        {
            NetworkEnvManager inst = GetInstance();

            inst.Env.Shutdown();
        }

        public static void SendData(Command c)
        {
            GetInstance().Env.SendData(c);
        }

        public static void HandleContact(int idA, int idB)
        {
            GetInstance().Env.HandleContact(idA, idB);
        }

        private static NetworkEnvManager GetInstance()
        {
            Debug.Assert(instance != null);

            return instance;
        }

        public static string GetEnvName()
        {
            return GetInstance().Env.GetName();
        }

        public static void CreatePlayer(byte id, PlayerName name)
        {
            GetInstance().Env.CreatePlayer(id, name);
        }

        public static PlayerName GetPlayerName()
        {
            return GetInstance().Env.GetPlayerName();
        }
    }
}
