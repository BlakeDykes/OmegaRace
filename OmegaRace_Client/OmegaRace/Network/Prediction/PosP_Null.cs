using Box2DX.Common;

namespace OmegaRace
{
    public class PosP_Null : PositionPredictor
    {
        public override float GetTargetAngle_Deg()
        {
            return _Subject.GetAngle_Deg();
        }

        public override Vec2 GetTargetPos()
        {
            return _Subject.GetPixelPosition();
        }

        public override void Set_Immediate(float time, float angle, Vec2 pos, Vec2 vel)
        {
            _Subject.SetPhysicalPosition(pos);
            _Subject.SetPixelVelocity(new Vec2(0, 0));
        }

        public override void ReceivePrediction(float time, float angle, Vec2 pos)
        {
        }

        public override void ReceivePrediction(float time, float angle, Vec2 pos, Vec2 vel)
        {
        }

        public override void Execute()
        {
        }
    }
}
