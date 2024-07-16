using System;
using System.Diagnostics;

namespace OmegaRace
{
    class NullQueue : QueueBase
    {
        public NullQueue()
            : base(QUEUE_NAME.UNDEFINED, QUEUE_TYPE.NULL, null)
        { }

        public NullQueue(QUEUE_NAME name)
            : base(name, QUEUE_TYPE.NULL, null)
        {}

        public override void Broadcast(Command com)
        {
        }

        public override void ProcessIn()
        {
        }

        public override void ProcessOut()
        {
        }
    }
}
