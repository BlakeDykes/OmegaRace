using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;


namespace OmegaRace
{
    class PlaybackReader
    {
        private class PlaybackQueueHelper
        {
            private UInt64 _NextFreeIndex;
            private C_PlaybackWrapper[] _Commands;

            public C_PlaybackWrapper[] Commands { get => _Commands; }

            public PlaybackQueueHelper(UInt64 count)
            {
                _NextFreeIndex = 0;
                _Commands = new C_PlaybackWrapper[count];

                for (UInt64 i = 0; i < count; ++i)
                {
                    _Commands[i] = new C_PlaybackWrapper();
                }
            }

            public void AddCommand(float executeTime, Command c)
            {
                _Commands[_NextFreeIndex].Set(executeTime, c);

                _NextFreeIndex += 1;
            }
        }

        private Dictionary<QUEUE_NAME, PlaybackQueueHelper> _PlaybackComs;
        private UInt64 _CurrentComIndex;
        private Command[] _Coms;


        public PlaybackReader(PlaybackHeader playbackHeader, ref BinaryReader reader)
        {
            _PlaybackComs = new Dictionary<QUEUE_NAME, PlaybackQueueHelper>();
            _CurrentComIndex = 0;
            _Coms = new Command[playbackHeader.CommandCount()];

            PlaybackHeader.QueueInfo[] queueInfo = playbackHeader.GetQueueInfo();

            foreach (PlaybackHeader.QueueInfo queue in queueInfo)
            {
                AddQueue(queue.Name, queue.CommandCount);
            }

            for(UInt64 i = 0; i < (UInt64)_Coms.LongLength; ++i)
            {
                AddCommand(ref reader);
            }
        }

        private void AddQueue(QUEUE_NAME name, UInt64 comCount)
        {
            _PlaybackComs.Add(name, new PlaybackQueueHelper(comCount));
        }

        private void AddCommand(ref BinaryReader reader)
        {
            float time = reader.ReadSingle();
            COMMAND_TYPE type = Command.GetType(ref reader);
            QUEUE_NAME queue = Command.GetInputQueue(ref reader);

            _Coms[_CurrentComIndex] = CommandCollection.CreateCommand(type);
            _Coms[_CurrentComIndex].Deserialize(ref reader);

            _PlaybackComs[queue].AddCommand(time, _Coms[_CurrentComIndex]);
        }

        public void RouteCommandsIn()
        {
            foreach(KeyValuePair<QUEUE_NAME, PlaybackQueueHelper> queueCommands in _PlaybackComs)
            {
                CommandQueueManager.RouteIn(queueCommands.Key, queueCommands.Value.Commands);
            }
        }

    }
}
