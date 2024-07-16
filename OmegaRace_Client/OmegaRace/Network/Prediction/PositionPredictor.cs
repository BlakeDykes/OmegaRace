using Box2DX.Common;

namespace OmegaRace
{
    public class Prediction
    {
        private float _Time;
        private float _Angle;
        private Vec2 _Pos;
        private Vec2 _Velocity;

        public Prediction()
        {
            _Pos = new Vec2(0.0f, 0.0f);
            _Velocity = new Vec2(0.0f, 0.0f);
        }

        public void Set(PredictableObject subject)
        {
            _Time = TimeManager.GetCurrentTime();
            _Pos.Set(subject.GetPixelPosition().X, subject.GetPixelPosition().Y);
            _Angle = subject.GetAngle_Deg();
        }

        public void Set(Prediction pred)
        {
            _Time = pred._Time;
            _Angle = pred._Angle;
            _Pos = pred._Pos;
            _Velocity = pred._Velocity;
        }

        public void Set(float time, float angle, Vec2 pos)
        {
            float timeDif = time - _Time > 0
                                ? time - _Time
                                : 1.0f;

            _Velocity.X = (pos.X - _Pos.X) / timeDif;
            _Velocity.Y = (pos.Y - _Pos.Y) / timeDif;

            _Angle = angle;
            _Time = time;
            _Pos.Set(pos.X, pos.Y);
        }

        public void Set(float time, float angle, Vec2 pos, Vec2 vel)
        {
            if (time >= _Time)
            {
                _Time = time;
                _Angle = angle;
                _Velocity = vel;
                _Pos = pos;
            }
            else if (time < _Time)
            {
                _Velocity = vel;
                _Angle = angle;
                _Pos = pos + (_Time - time) * _Velocity;
            }
        }

        public void Set_Immediate(float time, float angle, Vec2 pos, Vec2 vel)
        {
            _Time = time;
            _Angle = angle;
            _Pos = pos;
            _Velocity = vel;
        }

        public Vec2 GetPos(float time)
        {
            return _Pos + (time - _Time) * _Velocity;
        }

        public Vec2 GetPos()
        {
            return _Pos;
        }

        public Vec2 GetVelocity()
        {
            return _Velocity;
        }

        public float GetAngle()
        {
            return _Angle;
        }

        public void Clear()
        {
            _Time = 0.0f;
            _Angle = 0.0f;
            _Pos.SetZero();
            _Velocity.SetZero();
        }
    };

    public abstract class PositionPredictor
    {
        protected PredictableObject _Subject;
        protected Prediction _Target;
        protected Prediction _Prediction;

        public PositionPredictor()
        {
            _Subject = null;
            _Target = new Prediction();
            _Prediction = new Prediction();
        }

        public void Set(PredictableObject subject)
        {
            _Subject = subject;
            _Target.Set(_Subject);
            _Prediction.Set(_Subject);
        }

        public virtual float GetTargetAngle_Deg()
        {
            return _Target.GetAngle();
        }

        public virtual Vec2 GetTargetPos()
        {
            return _Target.GetPos(TimeManager.GetCurrentTime());
        }

        public abstract void ReceivePrediction(float time, float angle, Vec2 pos);
        public abstract void ReceivePrediction(float time, float angle, Vec2 pos, Vec2 vel);

        public abstract void Execute();

        public virtual void SetSubjectToTarget()
        {
            _Subject.SetPosAndAngleFromPred(_Target.GetAngle(), _Target.GetPos());
            _Prediction.Set(_Target);
        }

        public virtual void SetPredictionsToSubject()
        {
            _Prediction.Set(_Subject);
            _Target.Set(_Subject);
        }

        public virtual void Set_Immediate(float time, float angle, Vec2 pos, Vec2 vel)
        {
            _Prediction.Set_Immediate(time, angle, pos, vel);
            _Target.Set_Immediate(time, angle, pos, vel);
            _Subject.SetPhysicalPosition(pos);
            _Subject.SetPixelVelocity(vel);
        }

        public virtual void Clear()
        {
            _Subject = null;
            _Target.Clear();
            _Prediction.Clear();
        }

    }
}
