using System.Collections.Generic;
using Lidgren.Network;

namespace OmegaRace
{
    public enum QUEUE_NAME : byte
    {
        UNDEFINED = 0,
        SCENE_PLAY_IN = 1,
        SCENE_PLAY_OUT,
        NETWORK_IN,
        NETWORK_OUT,
    }


    public class CommandQueue : QueueBase
    {
        public CommandQueue(QUEUE_NAME name, NetworkEnv env, PlaybackRecorder recorder)
            : base(name, QUEUE_TYPE.NETWORK, env, recorder)
        { }

        public CommandQueue(QUEUE_NAME name, PlaybackRecorder recorder)
            : base(name, QUEUE_TYPE.NETWORK, recorder)
        { }

        public override void Broadcast(Command com)
        {
            byte[] destinations = _Env.Connections;
            com.Destination = destinations[0];
            _Queue.Enqueue(com);

            for(int i = 1; i < destinations.Length; ++i)
            {
                Command nextCom = CommandQueueManager.GetCommand(com.Type);
                nextCom.Copy(com);
                nextCom.Destination = destinations[i];

                _Queue.Enqueue(nextCom);
            }
        }

        public override void ProcessIn()
        {
            Command c;

            while (_Queue.Count > 0)
            {
                c = _Queue.Dequeue();

                //c.Print("Executing");

                c.Execute(ref _Env);

                _Recorder.Record(this.Name, c);

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

                    NetworkEnvManager.SendData(c);
                }

                CommandQueueManager.ReturnCommand(c);
            }
        }        
    }
}
