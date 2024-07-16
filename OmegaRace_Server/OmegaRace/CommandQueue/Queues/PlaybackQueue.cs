using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OmegaRace
{
    class PlaybackQueue : QueueBase
    {
        C_PlaybackWrapper[] _PlaybackQueue;
        int CurrentIndex;

        public PlaybackQueue(QUEUE_NAME name)
            : base(name, QUEUE_TYPE.PLAYBACK, null)
        {
            _Env = null;
            _PlaybackQueue = null;
            CurrentIndex = 0;
        }

        public override void Receive(Command msg)
        {
            CommandQueueManager.ReturnCommand(msg);
        }

        public override void Receive(Command[] commands)
        {
            _PlaybackQueue = (C_PlaybackWrapper[]) commands;
        }


        public override void Broadcast(Command com)
        {
        }

        public override void ProcessIn()
        {
            float time = TimeManager.GetCurrentTime();

            while(CurrentIndex < _PlaybackQueue.Length && _PlaybackQueue[CurrentIndex].ExecuteTime < time)
            {
                _PlaybackQueue[CurrentIndex++].Execute(ref _Env);
            }
        }

        public override void ProcessOut()
        {
        }
    }
}
