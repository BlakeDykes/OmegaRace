using Box2DX.Common;

namespace OmegaRace
{
    public class PosP_DReck : PositionPredictor
    {

        const float MAX_PREDICTION_DIFFERENCE = 0.9f;

        public override void Execute()
        {
            // Get actual position - 401, 400
            Vec2 pos = _Subject.GetPixelPosition();

            // Get what the client has predicted - 400, 400
            Vec2 predPos = _Target.GetPos(TimeManager.GetCurrentTime());

            // Calculate difference in x,y position
            float xDif = pos.X > predPos.X 
                    ? pos.X - predPos.X 
                    : predPos.X - pos.X;

            float yDif = pos.Y > predPos.Y 
                ? pos.Y - predPos.Y 
                : predPos.Y - pos.Y;

            // If the position the client predicted is off by too much, send an update prediction command
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
            this._Target.Set(time, angle, pos);
        }

        public override void ReceivePrediction(float time, float angle, Vec2 pos, Vec2 vel)
        {
            this._Target.Set(time, angle, pos, vel);
        }
    }
}
