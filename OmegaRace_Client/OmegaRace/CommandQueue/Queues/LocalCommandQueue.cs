using System;
using System.Diagnostics;

namespace OmegaRace
{
    class LocalCommandQueue : QueueBase
    {

        public LocalCommandQueue(QUEUE_NAME name, NetworkEnv env, PlaybackRecorder recorder)
            : base(name, QUEUE_TYPE.LOCAL, env, recorder)
        { }

        public LocalCommandQueue(QUEUE_NAME name, PlaybackRecorder recorder)
            : base(name, QUEUE_TYPE.LOCAL, recorder)
        { }

        public override void Broadcast(Command com)
        {
            CommandQueueManager.RouteOut(com);
        }

        public override void ProcessIn()
        {
            Command c;

            while (_Queue.Count > 0)
            {
                c = _Queue.Dequeue();

                //c.Print("Executing");

                c.Execute(ref _Env);

                _Recorder.Record(this._Name, c);

                CommandQueueManager.ReturnCommand(c);
            }
        }

        public override void ProcessOut()
        {
            while (_Queue.Count > 0)
            {
                Command c = _Queue.Dequeue();

                if (c.Initialize(_Env))
                {
                    //c.Print("Sending");

                    CommandQueueManager.RouteIn(c);
                }
            }
        }
    }
}
