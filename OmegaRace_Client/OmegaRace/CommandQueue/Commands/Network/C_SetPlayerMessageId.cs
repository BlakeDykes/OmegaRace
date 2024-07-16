using System;
using System.Diagnostics;
using System.IO;

namespace OmegaRace
{
    public class C_RequestPlayerMessageId : Command
    {
        private PlayerName _Name;
        public PlayerName Name { get => _Name; set => _Name = value; }

        public C_RequestPlayerMessageId()
            : base(COMMAND_TYPE.C_RequestPlayerMessageId, QUEUE_NAME.NETWORK_IN, QUEUE_NAME.NETWORK_OUT)
        {

        }

        public override void Deserialize(ref BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Execute(ref NetworkEnv env)
        {
            throw new NotImplementedException();
        }

        public override bool Initialize(NetworkEnv env)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }


    public class C_SetPlayerMessageId : Command
    {
        private byte _PlayerMessageId;
        public byte PlayerMessageId { get => _PlayerMessageId; set => _PlayerMessageId = value; }
        
        public C_SetPlayerMessageId()
            : base(COMMAND_TYPE.C_SetPlayerMessageId, QUEUE_NAME.NETWORK_IN, QUEUE_NAME.NETWORK_OUT)
        {
            _PlayerMessageId = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            _PlayerMessageId = reader.ReadByte();
        }

        public override void Execute(ref NetworkEnv env)
        {
            env.MessageId = _PlayerMessageId;
            GameManager.AddPlayer(_PlayerMessageId);
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);

            return true;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write(_PlayerMessageId);
        }
        public override void Copy(Command c)
        {
            throw new NotImplementedException();
        }
        public override void Clear()
        {
            base.Clear();
            _PlayerMessageId = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
        }
    }
}
