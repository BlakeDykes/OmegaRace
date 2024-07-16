using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmegaRace
{
    public enum QUEUE_TYPE 
    {
        NULL,
        LOCAL,
        NETWORK,
        PLAYBACK
    }

    public abstract class QueueBase
    {
        protected QUEUE_NAME _Name;
        protected QUEUE_TYPE _Type;
        protected NetworkEnv _Env;
        protected Queue<Command> _Queue;
        public QUEUE_NAME Name { get => _Name; }
        public QUEUE_TYPE QueueType { get => _Type; }
        public NetworkEnv Env { set => _Env = value; }

        protected PlaybackRecorder _Recorder;

        public QueueBase(QUEUE_NAME name, QUEUE_TYPE type, NetworkEnv env, PlaybackRecorder recorder)
        {
            _Name = name;
            _Type = type;
            _Queue = new Queue<Command>();
            _Env = env;
            _Recorder = recorder;
        }

        public QueueBase(QUEUE_NAME name, QUEUE_TYPE type, PlaybackRecorder recorder)
        {
            _Name = name;
            _Type = type;
            _Queue = new Queue<Command>();
            _Env = null;
            _Recorder = recorder;
        }

        public virtual void Receive(Command msg)
        {
            _Queue.Enqueue(msg);
        }

        public virtual void Receive(Command[] commands)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            _Queue.Clear();
        }

        public virtual void StartRecord()
        {
            _Recorder.StartRecord();
        }

        /// <summary>
        /// Sends a command to all connections in NetworkEnv
        /// </summary>
        /// <param name="com"></param>
        public abstract void Broadcast(Command com);
        public abstract void ProcessIn();
        public abstract void ProcessOut();
    }
}
