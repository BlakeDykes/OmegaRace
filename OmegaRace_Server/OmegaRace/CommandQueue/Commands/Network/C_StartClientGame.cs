using System;
using System.IO;
using System.Linq;

namespace OmegaRace
{
    class C_StartClientGame : Command
    {
        private byte _PlayerCount;
        private byte[] _PlayerIds;

        public byte[] PlayerIds
        {
            get => _PlayerIds;
            set
            {
                _PlayerIds = value;
                _PlayerCount = (byte)_PlayerIds.Length;
            }
        }

        public C_StartClientGame()
            :base(COMMAND_TYPE.C_StartClientGame, QUEUE_NAME.NETWORK_IN, QUEUE_NAME.NETWORK_OUT)
        { }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            _PlayerCount = reader.ReadByte();
            _PlayerIds = new byte[_PlayerCount];

            _PlayerIds = reader.ReadBytes(_PlayerCount);
        }

        public override void Execute(ref NetworkEnv env)
        {
            GameManager.StartClient(_PlayerIds);
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);

            return true;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write(_PlayerCount);
            writer.Write(_PlayerIds);
        }

        public override void Copy(Command c)
        {
            C_StartClientGame com = (C_StartClientGame)c;
            _PlayerCount = com._PlayerCount;
            _PlayerIds = new byte[_PlayerCount];

            com._PlayerIds.CopyTo(this._PlayerIds, 0);
        }

        public override void Clear()
        {
            base.Clear();
            _PlayerCount = 0;
        }
    }
}
