using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Lidgren.Network;

namespace OmegaRace
{
    class C_DeliveryWrapper : Command
    {
        protected int _CommandCount;
        protected List<Command> _Commands;

        // ----------------------------------------------------------------------
        // Virtual Members  -----------------------------------------------------
        // ----------------------------------------------------------------------
        public override COMMAND_TYPE Type { get => _Type; }
        public override QUEUE_NAME InQueue { get => _InQueue; }
        public override QUEUE_NAME OutQueue { get => _OutQueue; }
        public override COMMAND_TYPE NetCallback { get => _NetCallback; set => _NetCallback = value; }
        public override byte Origin { get => _Origin; set => _Origin = value; }
        public override byte Destination { get => _Destination; set => _Destination = value; }
        public override byte Subject { get => _Subject; set => _Subject = value; }


        public C_DeliveryWrapper()
            : base(COMMAND_TYPE.C_DeliveryWrapper, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
            _CommandCount = 0;
            _Commands = new List<Command>();
        }

        public void Add(params Command[] coms)
        {
            Debug.Assert(coms.Length != 0);

            foreach(Command com in coms)
            {
                _Commands.Add(com);

                ++_CommandCount;
            }
        }

        public void Add(params COMMAND_TYPE[] coms)
        {
            Debug.Assert(coms.Length != 0);

            foreach(COMMAND_TYPE c in coms)
            {
                Command com = CommandQueueManager.GetCommand(c);
                com.SetDestAndSubject(_Destination, _Subject);
                _Commands.Add(com);

                ++_CommandCount;
            }
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);

            _CommandCount = reader.ReadInt32();

            for(int i = 0; i < _CommandCount; ++i)
            {
                Command c = CommandQueueManager.GetCommand(protGetType(ref reader));
                c.Deserialize(ref reader);

                _Commands.Add(c);
            }
        }

        public override void Execute(ref NetworkEnv env)
        {
            foreach(Command c in _Commands)
            {
                c.Execute(ref env);
            }
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);

            foreach(Command c in _Commands)
            {
                c.Initialize(env);
            }

            SetDeliverSettings();
            
            return (_CommandCount > 0);
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);

            writer.Write(_CommandCount);

            for (int i = 0; i < _CommandCount; ++i)
            {
                _Commands[i].Serialize(ref writer);
            }
        }

        public override void Copy(Command c)
        {
            base.Copy(c);
            C_DeliveryWrapper com = (C_DeliveryWrapper)c;

            this._CommandCount = com._CommandCount;

            foreach(Command nestedCom in com._Commands)
            {
                Command newNested = CommandQueueManager.GetCommand(nestedCom.Type);
                newNested.Copy(nestedCom);

                this._Commands.Add(newNested);
            }
        }

        public override void ReturnSelf(ref Dictionary<COMMAND_TYPE, CommandPool> pools)
        {
            foreach(Command c in _Commands)
            {
                c.ReturnSelf(ref pools);
            }

            Clear();
            pools[this.Type].Push(this);
        }

        public override void Print(string prependString)
        {
            //Console.WriteLine("{0} - {1}", prependString, Type.ToString());
            //foreach(Command c in _Commands)
            //{
            //    c.Print("--- Containing: ");
            //}
        }

        public override void Clear()
        {
            base.Clear();
            _CommandCount = 0;
            _Commands.Clear();
            DeliveryMethod = NetDeliveryMethod.Unknown;
            Sequence = 0;
        }

        private void SetDeliverSettings()
        {
            Debug.Assert(_Commands.Count > 0);

            this._DeliveryMethod = _Commands[0].DeliveryMethod;
            this._Sequence = _Commands[0].Sequence;
        }
    }
}
