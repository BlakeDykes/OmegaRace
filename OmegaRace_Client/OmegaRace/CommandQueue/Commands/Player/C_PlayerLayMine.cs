using System;
using System.IO;


namespace OmegaRace
{
    public class C_PlayerRequestLayMine : Command
    {
        public C_PlayerRequestLayMine()
            : base(COMMAND_TYPE.C_PlayerRequestLayMine, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
        }

        public override void Execute(ref NetworkEnv env)
        {
            C_PlayerLayMine com = (C_PlayerLayMine)CommandQueueManager.GetCommand(COMMAND_TYPE.C_PlayerLayMine);
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
    public class C_PlayerLayMine : Command
    {
        public C_PlayerLayMine()
            : base(COMMAND_TYPE.C_PlayerLayMine, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        { }

        public override void Execute(ref NetworkEnv env)
        {
            GameSceneCollection.ScenePlay.PlayerMgr[Subject].LayMine();
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
