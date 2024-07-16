using System;
using System.Diagnostics;
using Azul;
using Box2DX.Common;

namespace OmegaRace
{
    public abstract class PredictableObject : PhysicsObject
    {
        protected PositionPredictor _PosPredictor;

        public PredictableObject(GAMEOBJECT_TYPE _type, Rect textureRect, Rect screenRect, Texture text, Color c, bool usingPhysics = true, float angle = 0.0f)
            : base(_type, textureRect, screenRect, text, c, usingPhysics, angle)
        {
            _PosPredictor = PredictionManager.GetPrediction(this);
        }

        public override void Update()
        {
            _PosPredictor.Execute();
            base.Update();
        }

        public float GetTargetAngle_Deg()
        {
            return _PosPredictor.GetTargetAngle_Deg();
        }

        public Vec2 GetTargetPosition()
        {
            return _PosPredictor.GetTargetPos();
        }

        public virtual void RefreshPrediction()
        {
            _PosPredictor.SetPredictionsToSubject();
        }

        public virtual void SetPrediction(float time, float angle, Vec2 pos, Vec2 vel)
        {
            _PosPredictor.Set_Immediate(time, angle, pos, vel);
        }

        /// <summary>
        /// Receive position and angle update as an update to client side POS prediction
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ang"></param>
        public void SetPosAndAngle(float time, float x, float y, float ang)
        {
            _PosPredictor.ReceivePrediction(time, ang, new Vec2(x, y));
        }

        public virtual void ReceivePrediction(float time, float angle, Vec2 pos, Vec2 vel)
        {
            _PosPredictor.ReceivePrediction(time, angle, pos, vel);
        }

        public virtual void SetPosAndAngleFromPred(float angle, Vec2 pos)
        {
            base.SetPosAndAngle(pos.X, pos.Y, angle);
        }

        public override void Destroy()
        {
            PredictionManager.ReturnPrediction(_PosPredictor);
            base.Destroy();
        }
    }
}
