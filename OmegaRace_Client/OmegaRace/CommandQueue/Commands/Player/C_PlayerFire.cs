using System.IO;
using Box2DX.Common;
using System.Diagnostics;
using System;

namespace OmegaRace
{
    public class C_PlayerRequestFire : Command
    {
        public C_PlayerRequestFire()
            : base(COMMAND_TYPE.C_PlayerRequestFire, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
        }

        public override void Execute(ref NetworkEnv env)
        {
            C_PlayerFire com = (C_PlayerFire)CommandQueueManager.GetCommand(COMMAND_TYPE.C_PlayerFire);
            com.Subject = this.Subject;

            CommandQueueManager.Broadcast(true, com);
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);

            return true;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
        }
    }



    public class C_PlayerFire : Command
    {
        public C_PlayerFire()
            : base(COMMAND_TYPE.C_PlayerFire, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
        }

        public override void Execute(ref NetworkEnv env)
        {
            GameSceneCollection.ScenePlay.PlayerMgr[Subject].FireMissile();
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);

            return true;
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
        }
    }
}
