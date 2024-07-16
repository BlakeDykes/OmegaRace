using System;
using System.Diagnostics;
using System.IO;
using Box2DX.Common;

namespace OmegaRace
{
    class C_UpdatePosPrediction : Command
    {
        private Vec2 _Pos;
        private Vec2 _Vel;
        private float _Angle;
        private float _Time;
        private int _Id;

        public int Id { set => _Id = value; }

        public C_UpdatePosPrediction()
            : base(COMMAND_TYPE.C_UpdatePosPrediction, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
            _Pos = new Vec2();
            _Vel = new Vec2();
        }

        public override void Execute(ref NetworkEnv env)
        {
            PredictableObject p = (PredictableObject)GameManager.Find(_Id);
            if(p != null)
            {
                p.ReceivePrediction(_Time, _Angle, _Pos, _Vel);
            }
            else
            {
                base.Print("~~~~~~ Failed");
            }
        }

        public override bool Initialize(NetworkEnv env)
        {
            PredictableObject subject = (PredictableObject)GameManager.Find(_Id);
            if (subject != null)
            {
                _Pos = subject.GetPixelPosition();
                _Vel = subject.GetPixelVelocity();
                _Angle = subject.GetAngle_Deg();
                _Time = TimeManager.GetCurrentTime();

                return true;
            }

            base.Print("~~~~~~ Failed");
            return false;
        }
        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            _Pos.X = reader.ReadSingle();
            _Pos.Y = reader.ReadSingle();
            _Vel.X = reader.ReadSingle();
            _Vel.Y = reader.ReadSingle();
            _Angle = reader.ReadSingle();
            _Time = reader.ReadSingle();
            _Id = reader.ReadInt32();
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write(_Pos.X);
            writer.Write(_Pos.Y);
            writer.Write(_Vel.X);
            writer.Write(_Vel.Y);
            writer.Write(_Angle);
            writer.Write(_Time);
            writer.Write(_Id);
        }

        public override void Copy(Command c)
        {
            base.Copy(c);
            C_UpdatePosPrediction com = (C_UpdatePosPrediction)c;
            this._Id = com._Id;
        }
    }
}
