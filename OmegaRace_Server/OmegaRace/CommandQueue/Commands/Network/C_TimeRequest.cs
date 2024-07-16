using System;
using System.Diagnostics;
using System.IO;

namespace OmegaRace
{
    public class C_TimeRequest : Command
    {
        private float _T0;
        public float T0 { get => _T0; set => _T0 = value; }

        public C_TimeRequest()
            : base(COMMAND_TYPE.C_TimeRequest, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        { }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            T0 = reader.ReadSingle();
        }

        public override void Execute(ref NetworkEnv env)
        {
            // Set and return response
            C_TimeResponse response = (C_TimeResponse)CommandQueueManager.GetCommand(COMMAND_TYPE.C_TimeResponse);
            response.T0 = this.T0;
            response.Destination = Origin;
            CommandQueueManager.RouteOut(response);
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);
            T0 = TimeManager.GetCurrentTime();

            return true;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write(T0);
        }

        public override void Copy(Command c)
        {
            C_TimeRequest com = (C_TimeRequest)c;
            this._T0 = com._T0;
        }

        public override void Clear()
        {
            base.Clear();
            _T0 = 0.0f;
        }
    }

    class C_TimeResponse : Command
    {
        private float _T;
        private float _T0;

        public float T0 { get => _T0; set => _T0 = value; }

        public C_TimeResponse()
            : base(COMMAND_TYPE.C_TimeResponse, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        { }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            _T = reader.ReadSingle();
            _T0 = reader.ReadSingle();
        }

        public override void Execute(ref NetworkEnv env)
        {
            // Sets time on execution
            TimeManager.SetCurrentTime_Cristians(_T, _T0);
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);
            _T = TimeManager.GetCurrentTime();

            return true;
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write(_T);
            writer.Write(_T0);
        }

        public override void Copy(Command c)
        {
            C_TimeResponse com = (C_TimeResponse)c;
            this._T = com._T;
            this._T0 = com._T0;
        }

        public override void Clear()
        {
            base.Clear();
            _T0 = 0.0f;
            _T = 0.0f;
        }
    }
}
