using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.IO;

namespace OmegaRace
{
    public class CommandPool
    {
        private Type _Type;
        private MessageDeliverySettings _Settings;
        private Stack<Command> _Pool;

        public MessageDeliverySettings Settings { get => _Settings; }

        public CommandPool(COMMAND_TYPE type, MessageDeliverySettings deliverySettings)
        {
            _Type = Type.GetType(typeof(COMMAND_TYPE).Namespace + "." + type.ToString());
            _Settings = deliverySettings;
            _Pool = new Stack<Command>();
            Refresh();
        }

        public void Push(Command com)
        {
            if (_Pool.Count < int.MaxValue)
            {
                _Pool.Push(com);
            }
        }

        public Command Pop()
        {
            if (_Pool.Count > 0)
            {
                return _Pool.Pop();
            }
            else
            {
                Refresh();

                return _Pool.Pop();
            }
        }

        private void Refresh()
        {
            for (int i = 0; i < CommandCollection.POOL_REFRESH_COUNT; ++i)
            {
                Command c = (Command)Activator.CreateInstance(_Type);
                c.SetDeliverySettings(_Settings);

                _Pool.Push(c);
            }
        }

        public void Reserve(int size)
        {
            if (size < int.MaxValue)
            {
                while (_Pool.Count < size)
                {
                    Command c = (Command)Activator.CreateInstance(_Type);
                    c.SetDeliverySettings(_Settings);

                    _Pool.Push(c);
                }
            }
        }
    }

    class CommandCollection
    {
        public const int POOL_REFRESH_COUNT = 3;

        private Dictionary<COMMAND_TYPE, CommandPool> _Commands;

        public int Size { get => _Commands.Count; }

        Stats _CommandStats;

        public CommandCollection()
        {
            _Commands = new Dictionary<COMMAND_TYPE, CommandPool>();

            COMMAND_TYPE[] CommandVals = (COMMAND_TYPE[])Enum.GetValues(typeof(COMMAND_TYPE));

            _CommandStats = new Stats("Client-Prediction Client Message Stats");

            for (int i = 1; i < CommandVals.Length; i++)
            {
                _CommandStats.AddCommand(CommandVals[i]);

                _Commands.Add(CommandVals[i], new CommandPool(CommandVals[i], Command.GetDefaultDeliverySettings(CommandVals[i])));
            }
        }

        public Command GetCommand(COMMAND_TYPE type)
        {
            _CommandStats.Start();

            Command c = _Commands[type].Pop();

            _CommandStats.Stop();

            return c;
        }

        public Command[] GetCommand(COMMAND_TYPE type, int count)
        {
            _CommandStats.Start();

            Command[] commands = new Command[count];

            _Commands[type].Reserve(count);

            for (int i = 0; i < count; ++i)
            {
                commands[i] = _Commands[type].Pop();
            }

            _CommandStats.Stop();

            return commands;
        }

        public Command GetCommand(ref BinaryReader reader)
        {
            _CommandStats.Start();

            COMMAND_TYPE type = Command.GetType(ref reader);
            Command com = _Commands[type].Pop();
            com.Deserialize(ref reader);

            _CommandStats.Stop();

            return com;
        }

        public void ReturnCommand(Command com)
        {
            _CommandStats.IncrementCount(com.Type);

            com.ReturnSelf(ref _Commands);
        }

        public static Command CreateCommand(COMMAND_TYPE type)
        {
            return (Command)Activator.CreateInstance(Type.GetType(typeof(COMMAND_TYPE).Namespace + "." + type.ToString()));
        }

        public void PrintStats()
        {
            _CommandStats.Print();
        }

        /// <summary>
        /// Class for testing command access timing and count.
        /// </summary>
        public class Stats
        {
            string Name;
            private Dictionary<COMMAND_TYPE, int> Counts;
            Stopwatch Timer;
            ulong PrintCount;
            ulong TotalCount;

            public Stats(string name)
            {
                Name = name;
                Counts = new Dictionary<COMMAND_TYPE, int>();
                Timer = new Stopwatch();

                PrintCount = 10000;
                TotalCount = 0;
            }

            public void AddCommand(COMMAND_TYPE com)
            {
                Counts.Add(com, 0);
            }

            public void IncrementCount(COMMAND_TYPE com)
            {
                Counts[com] += 1;
                ++TotalCount;
            }

            public void Start()
            {
                Timer.Start();
            }
            public void Stop(COMMAND_TYPE comType)
            {
                Timer.Stop();
                ++Counts[comType];

                if (++TotalCount % PrintCount == 0)
                {
                    Print();
                }
            }

            public void Stop()
            {
                Timer.Stop();
            }

            public void Print()
            {
                Debug.Print("--------------------------------");
                Debug.Print("-- {0} -------------------------", Name);
                Debug.Print("-- Commands Processed - {0}", TotalCount);
                Debug.Print("---------- Time Spent - {0}", (Timer.Elapsed.TotalMilliseconds * 1000 * 1000 / 100000000).ToString("0.00 ns"));
                Debug.Print("--------------------------------");
                Debug.Print("-- Command Counts --------------");

                foreach (KeyValuePair<COMMAND_TYPE, int> count in Counts)
                {
                    Debug.Print("-- {0, 24} - {1, -4}", count.Key.ToString(), count.Value);
                }

                Debug.Print("--------------------------------\n");

            }
        }
    }
}
