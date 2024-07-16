using System;
using System.Diagnostics;

namespace OmegaRace
{
    public enum RECORDER_TYPE : byte
    {
        NULL,
        BASIC
    }

    public abstract class PlaybackRecorder
    {
        abstract public void Record(QUEUE_NAME queue, Command c);
        abstract public void StartRecord();
    }

    class PlaybackRecorder_Basic : PlaybackRecorder
    {
        private PlaybackManager Instance;

        public PlaybackRecorder_Basic(PlaybackManager inst)
        {
            Instance = inst;
        }

        public override void Record(QUEUE_NAME queue, Command c)
        {
            Instance.Record(queue, c);
        }

        public override void StartRecord()
        {
            Instance.StartRecord();
        }
    }

    class PlaybackRecorder_Null : PlaybackRecorder
    {
        public PlaybackRecorder_Null()
        {
        }

        public override void Record(QUEUE_NAME queue, Command c)
        {
        }
        public override void StartRecord()
        {
        }
    }

}
