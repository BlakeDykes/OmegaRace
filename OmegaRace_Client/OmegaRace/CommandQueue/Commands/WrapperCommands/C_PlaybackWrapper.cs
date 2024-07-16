using System;
using System.Diagnostics;
using System.IO;

namespace OmegaRace
{
    public class C_PlaybackWrapper : Command
    {
        private float _ExecuteTime;
        private Command _Com;

        public float ExecuteTime { get => _ExecuteTime; }
        public Command Com { get => _Com; }

        public C_PlaybackWrapper()
            : base(COMMAND_TYPE.C_PlaybackWrapper, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
        }

        public override void Serialize(ref BinaryWriter writer)
        {
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            _ExecuteTime = reader.ReadSingle();
        }

        public override void Execute(ref NetworkEnv env)
        {
            Debug.Assert(TimeManager.GetCurrentTime() > _ExecuteTime);
            Debug.Assert(_Com != null);

            _Com.Execute(ref env);
        }

        public override bool Initialize(NetworkEnv env)
        {
            return false;
        }

        public void Set(float executeTime, Command c)
        {
            _ExecuteTime = executeTime;

            _Com = c;
            base.Copy(c);
        }
    }
}
