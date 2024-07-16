using Box2DX.Common;
using System.Diagnostics;

namespace OmegaRace
{
    public class PosP_ClientSide_Server : PositionPredictor
    {

        const float MAX_PREDICTION_DIFFERENCE = 1.0f;

        public PosP_ClientSide_Server(GameObject subject) : base(subject)
        {
        }

        public override void Execute()
        {
            Vec2 pos = _Subject.GetPixelPosition();
            Vec2 predPos = _Prediction.GetPos(TimeManager.GetCurrentTime());
            float xDif = pos.X > predPos.X ? pos.X - predPos.X : predPos.X - pos.X;
            float yDif = pos.Y > predPos.Y ? pos.Y - predPos.Y : predPos.Y - pos.Y;


            if (xDif > MAX_PREDICTION_DIFFERENCE
                || yDif > MAX_PREDICTION_DIFFERENCE)
            {
                C_UpdatePosPrediction com = (C_UpdatePosPrediction)CommandQueueManager.GetCommand(COMMAND_TYPE.C_UpdatePosPrediction);
                com.Id = _Subject.getID();

                CommandQueueManager.Broadcast(true, com);
            }
        }

        public override void ReceivePrediction(float time, float angle, Vec2 pos)
        {
            this._Prediction.Set(TimeManager.GetCurrentTime(), angle, pos);
        }
    }
}
