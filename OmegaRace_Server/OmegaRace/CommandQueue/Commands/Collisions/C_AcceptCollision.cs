using Lidgren.Network;
using System;
using System.Diagnostics;
using System.IO;

namespace OmegaRace
{
    class C_AcceptCollision : Command
    {
        int ID_A;
        int ID_B;

        public C_AcceptCollision() : base(COMMAND_TYPE.C_AcceptCollision, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
            ID_A = 0;
            ID_B = 0;
        }

        public C_AcceptCollision(int idA, int idB) : base(COMMAND_TYPE.C_AcceptCollision, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
            ID_A = idA;
            ID_B = idB;
        }

        public void Set(int idA, int idB)
        {
            ID_A = idA;
            ID_B = idB;
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            ID_A = reader.ReadInt32();
            ID_B = reader.ReadInt32();
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write(ID_A);
            writer.Write(ID_B);
        }

        public override void Execute(ref NetworkEnv env)
        {
            GameObject a = GameManager.Find(ID_A);
            GameObject b = GameManager.Find(ID_B);

            if(a != null && b != null)
            {
                a.Accept(b);
            }
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);

            return true;
        }

        public override void Copy(Command c)
        {
            C_AcceptCollision com = (C_AcceptCollision)c;

            this.ID_A = com.ID_A;
            this.ID_B = com.ID_B;
        }

        public override void Clear()
        {
            ID_A = 0;
            ID_B = 0;
        }
    }
}
