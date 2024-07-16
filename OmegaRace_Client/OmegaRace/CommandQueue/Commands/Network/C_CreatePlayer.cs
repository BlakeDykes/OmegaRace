using System;
using System.Diagnostics;
using System.IO;

namespace OmegaRace
{
    class C_CreatePlayer : Command
    {

        private PlayerName _Name;

        public PlayerName Name { get => _Name; set => _Name = value; }

        public C_CreatePlayer()
            : base(COMMAND_TYPE.C_CreatePlayer, QUEUE_NAME.NETWORK_IN, QUEUE_NAME.NETWORK_OUT)
        {
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            _Name = (PlayerName)reader.ReadByte();
        }

        public override void Execute(ref NetworkEnv env)
        {
            NetworkEnvManager.CreatePlayer(Origin, _Name);
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);
            ClientManager client = (ClientManager)env;

            _Name = client.PlayerName;

            return true;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write((byte)_Name);
        }
    }
}
