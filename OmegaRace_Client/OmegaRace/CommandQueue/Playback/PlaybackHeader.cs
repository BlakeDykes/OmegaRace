using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace OmegaRace
{
    public class PlaybackHeader
    {
        // -------------------------------------------
        // PlayerInfo Implementation 
        // -------------------------------------------
        public class PlayerInfo
        {
            private const byte PAD_SIZE = 2;

            private PlayerName _Name;
            private byte _Id;
            byte[] _Pad;

            public PlayerName Name { get => _Name; }
            public byte Id { get => _Id; }

            public PlayerInfo()
            {
                _Name = PlayerName.ALL;
                _Id = 0;
                _Pad = new byte[PAD_SIZE];
                _Pad[0] = PAD_SIZE;
            }

            public PlayerInfo(PlayerData player)
            {
                _Name = player.Name;
                _Id = player.player;
                _Pad = new byte[PAD_SIZE];
                _Pad[0] = PAD_SIZE;
            }

            public static int Size() { return sizeof(PlayerName) + sizeof(byte) + (sizeof(byte) * PlayerInfo.PAD_SIZE); }

            public void Serialize(ref BinaryWriter writer)
            {
                writer.Write((byte)_Name);
                writer.Write(_Id);
                writer.Write(_Pad);
            }

            public void Deserialize(ref BinaryReader reader)
            {
                _Name = (PlayerName)reader.ReadByte();
                _Id = reader.ReadByte();
                _Pad = reader.ReadBytes(PAD_SIZE);
            }
        }

        // -------------------------------------------
        // QueueInfo Implementation 
        // -------------------------------------------
        public class QueueInfo
        {
            private const byte PAD_SIZE = 7;

            private QUEUE_NAME _Name;
            private byte[] _Pad;
            private UInt64 _CommandCount;

            public QUEUE_NAME Name { get => _Name; }
            public UInt64 CommandCount { get => _CommandCount; }

            public void IncrementCommandCount() { _CommandCount += 1; }

            public QueueInfo()
            {
                _Name = QUEUE_NAME.UNDEFINED;
                _Pad = new byte[PAD_SIZE];
                _Pad[0] = PAD_SIZE;
                _CommandCount = 0;
            }

            public QueueInfo(QUEUE_NAME name)
            {
                _Name = name;
                _Pad = new byte[PAD_SIZE];
                _Pad[0] = PAD_SIZE;
                _CommandCount = 0;
            }

            public static int Size() { return sizeof(QUEUE_NAME) + (sizeof(byte) * PAD_SIZE) + sizeof(UInt64); }

            public void Serialize(ref BinaryWriter writer)
            {
                writer.Write((byte)_Name);
                writer.Write(_Pad);
                writer.Write(_CommandCount);
            }

            public void Deserialize(ref BinaryReader reader)
            {
                _Name = (QUEUE_NAME)reader.ReadByte();
                _Pad = reader.ReadBytes(PAD_SIZE);
                _CommandCount = reader.ReadUInt64();
            }
        }

        // -------------------------------------------
        // PlaybackHeader implementation begins
        // -------------------------------------------
        private const byte HEADER_VERSION = 1;
        private const byte PAD_SIZE = 3;

        private byte _HeaderVersion;
        private byte[] _Pad;
        private float _StartTime;
        private int _QueueCount;
        private int _PlayerCount;
        private PlayerInfo[] _PlayerInfo;
        private Dictionary<QUEUE_NAME, QueueInfo> _QueueInfo;

        public int PlayerCount { get => _PlayerCount; }
        public int QueueCount { get => _QueueCount; }

        public PlaybackHeader()
        {
            _HeaderVersion = HEADER_VERSION;
            _Pad = new byte[PAD_SIZE];
            _Pad[0] = PAD_SIZE;
            _StartTime = 0;
            _QueueCount = 0;
            _PlayerCount = 0;
            _PlayerInfo = null;
            _QueueInfo = new Dictionary<QUEUE_NAME, QueueInfo>();
        }
        
        public UInt64 CommandCount(QUEUE_NAME queueName) { return _QueueInfo[queueName].CommandCount; }
        public UInt64 CommandCount()
        {
            UInt64 count = 0;

            foreach (KeyValuePair<QUEUE_NAME, QueueInfo> queue in _QueueInfo)
            {
                count += queue.Value.CommandCount;
            }

            return count;
        }

        public PlayerInfo GetPlayerInfo(int i) { return _PlayerInfo[i]; }

        public QueueInfo[] GetQueueInfo()
        {
            QueueInfo[] queues = new QueueInfo[_QueueInfo.Count];
            _QueueInfo.Values.CopyTo(queues, 0);

            return queues;
        }

        public void IncrementCommandCount(QUEUE_NAME queueName)
        {

            if (_QueueInfo.ContainsKey(queueName))
            {
                _QueueInfo[queueName].IncrementCommandCount();
            }
            else
            {
                _QueueInfo.Add(queueName, new QueueInfo(queueName));
                _QueueInfo[queueName].IncrementCommandCount();
            }
        }

        public void SetPlayers(Dictionary<byte, PlayerData> playerData)
        {
            _PlayerCount = playerData.Count;
            _PlayerInfo = new PlayerInfo[_PlayerCount];

            int i = 0;
            foreach (KeyValuePair<byte, PlayerData> player in playerData)
            {
                _PlayerInfo[i++] = new PlayerInfo(player.Value);
            }
        }

        public void SetStart(float startTime)
        {
            _StartTime = startTime;
        }

        public void AddQueue(QUEUE_NAME name)
        {
            _QueueInfo.Add(name, new QueueInfo(name));
            _QueueCount++;
        }

        public int GetSize()
        {
            int size = sizeof(byte)                                     // _HeaderVersion
                       + PAD_SIZE                                       // _Pad
                       + sizeof(float)                                  // _StartTime
                       + sizeof(int) * 2                                // _PlayerCount, _QueueCount
                       + (_PlayerInfo != null ?
                            _PlayerInfo.Length * PlayerInfo.Size() 
                            : 2 * PlayerInfo.Size())                    // _PlayerInfo
                       + (_QueueInfo.Count * QueueInfo.Size());         // _QueueInfo


            //if(size % HEADER_ALIGNMENT != 0)
            //{
            //    size = size + HEADER_ALIGNMENT - (size % HEADER_ALIGNMENT);
            //}

            return size;
        }

        public void Serialize(ref BinaryWriter writer)
        {
            writer.Write(_HeaderVersion);
            writer.Write(_Pad);
            writer.Write(_StartTime);
            writer.Write(_QueueCount);
            writer.Write(_PlayerCount);

            for (int i = 0; i < _PlayerCount; ++i)
            {
                _PlayerInfo[i].Serialize(ref writer);
            }

            foreach (KeyValuePair<QUEUE_NAME, QueueInfo> queueInfo in _QueueInfo)
            {
                queueInfo.Value.Serialize(ref writer);
            }
        }

        public void Deserialize(ref BinaryReader reader)
        {
            _HeaderVersion = reader.ReadByte();
            Debug.Assert(_HeaderVersion == HEADER_VERSION);

            _Pad = reader.ReadBytes(PAD_SIZE);

            _StartTime = reader.ReadSingle();
            _QueueCount = reader.ReadInt32();
            _PlayerCount = reader.ReadInt32();

            _PlayerInfo = new PlayerInfo[_PlayerCount];

            for (int i = 0; i < _PlayerCount; ++i)
            {
                _PlayerInfo[i] = new PlayerInfo();
                _PlayerInfo[i].Deserialize(ref reader);
            }

            for (int i = 0; i < _QueueCount; ++i)
            {
                QueueInfo info = new QueueInfo();
                info.Deserialize(ref reader);
                _QueueInfo.Add(info.Name, info);
            }
        }
    }
}
