using Box2DX.Common;

namespace OmegaRace
{
    public class PosP_ClientSide : PositionPredictor
    {
        public override void ReceivePrediction(float time, float angle, Vec2 pos)
        {
            this._Target.Set(time, angle, pos);
        }

        public override void ReceivePrediction(float time, float angle, Vec2 pos, Vec2 vel)
        {
            this._Target.Set(time, angle, pos, vel);
        }

        public override void Execute()
        {
            float time = TimeManager.GetCurrentTime();

            Vec2 pos = (_Subject.GetPixelPosition() + _Target.GetPos(time));
            pos.X /= 2.0f;
            pos.Y /= 2.0f;

            float angle = (_Subject.GetAngle_Deg() + _Target.GetAngle()) / 2.0f;

            _Subject.SetPosAndAngleFromPred(angle, pos);
        }
    }
}
