using System;
using System.IO;
using System.Collections.Generic;
using Lidgren.Network;

namespace OmegaRace
{

    public enum COMMAND_TYPE : byte 
    {
        UNDEFINED = 0,          // <-- Keep as 0
        
        C_StartClientGame = 1,
        C_PlayerMove,
        C_PlayerRequestFire,
        C_PlayerFire,
        C_PlayerRequestLayMine,
        C_PlayerLayMine,
        C_SetPlayerPos,
        C_SetMissilePos,

        C_AcceptCollision,

        C_CreatePlayer,
        C_RequestPlayerMessageId,
        C_SetPlayerMessageId,

        C_TimeRequest,
        C_TimeResponse,

        C_PlaybackWrapper,
        C_DeliveryWrapper,

        C_UpdatePosPrediction
    }

    public struct MessageDeliverySettings
    {
        public NetDeliveryMethod DeliveryMethod { get; private set; }
        public int SequenceChannel { get; private set; }

        public MessageDeliverySettings(NetDeliveryMethod deliveryMethod, int sequenceChannel)
        {
            DeliveryMethod = deliveryMethod;
            SequenceChannel = sequenceChannel;
        }
    }

    public abstract class Command
    {
        // ----------------------------------------------------------------------
        // Static Methods -------------------------------------------------------
        // ----------------------------------------------------------------------

        public static COMMAND_TYPE GetType(ref BinaryReader reader)
        {
            COMMAND_TYPE type = (COMMAND_TYPE)reader.ReadByte();
            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            return type;
        }

        /// <summary>
        /// Reads input queue from binary input stream
        /// </summary>
        /// <param name="reader">Assumes reader's position is at start of command data</param>
        /// <returns></returns>
        public static QUEUE_NAME GetInputQueue(ref BinaryReader reader)
        {
            reader.BaseStream.Seek(sizeof(COMMAND_TYPE), SeekOrigin.Current);
            
            QUEUE_NAME queue = (QUEUE_NAME)reader.ReadByte();

            reader.BaseStream.Seek(-1 * (sizeof(QUEUE_NAME) + sizeof(COMMAND_TYPE)), SeekOrigin.Current);

            return queue;
        }

        public static MessageDeliverySettings GetDefaultDeliverySettings(COMMAND_TYPE type)
        {
            switch (type)
            {
                // ---------------------------------
                // ------ Reliable Ordered ---------
                // ---------------------------------
                case COMMAND_TYPE.C_StartClientGame:            return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 1);
                case COMMAND_TYPE.C_CreatePlayer:               return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 1);
                case COMMAND_TYPE.C_RequestPlayerMessageId:     return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 1);
                case COMMAND_TYPE.C_SetPlayerMessageId:         return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 1);

                case COMMAND_TYPE.C_PlayerMove:                 return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 2);
                case COMMAND_TYPE.C_AcceptCollision:            return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 2);
                case COMMAND_TYPE.C_PlayerLayMine:              return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 2);
                case COMMAND_TYPE.C_PlayerFire:                 return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 2);

                case COMMAND_TYPE.C_UpdatePosPrediction: return new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 3);

                // ---------------------------------
                // ------ Reliable Sequenced -------
                // ---------------------------------
                case COMMAND_TYPE.C_SetPlayerPos:               return new MessageDeliverySettings(NetDeliveryMethod.ReliableSequenced, 4);
                case COMMAND_TYPE.C_SetMissilePos:               return new MessageDeliverySettings(NetDeliveryMethod.ReliableSequenced, 5);

                // ---------------------------------
                // ------ Reliable Unordered -------
                // ---------------------------------
                case COMMAND_TYPE.C_TimeRequest:                return new MessageDeliverySettings(NetDeliveryMethod.ReliableUnordered, 0);
                case COMMAND_TYPE.C_TimeResponse:               return new MessageDeliverySettings(NetDeliveryMethod.ReliableUnordered, 0);
                case COMMAND_TYPE.C_PlayerRequestFire:          return new MessageDeliverySettings(NetDeliveryMethod.ReliableUnordered, 0);
                case COMMAND_TYPE.C_PlayerRequestLayMine:       return new MessageDeliverySettings(NetDeliveryMethod.ReliableUnordered, 0);

                // ---------------------------------
                // ------ Unreliable Sequenced -----
                // ---------------------------------

                // ---------------------------------
                // ------ Unreliable ---------------
                // ---------------------------------

                // ---------------------------------
                // ------ Unknown ------------------
                // ---------------------------------
                case COMMAND_TYPE.C_DeliveryWrapper:            return new MessageDeliverySettings(NetDeliveryMethod.Unknown, 0);
                case COMMAND_TYPE.C_PlaybackWrapper:            return new MessageDeliverySettings(NetDeliveryMethod.Unknown, 0);

                // ---------------------------------
                // ---------------------------------
                case COMMAND_TYPE.UNDEFINED:
                default:
                    throw new ArgumentException();
            }
        }

        // ----------------------------------------------------------------------
        // Protected Members  ---------------------------------------------------
        // ----------------------------------------------------------------------

        protected COMMAND_TYPE _Type;
        protected QUEUE_NAME _InQueue;
        protected QUEUE_NAME _OutQueue;
        protected COMMAND_TYPE _NetCallback;
        protected byte _Origin;
        protected byte _Destination;
        protected byte _Subject;
        protected NetDeliveryMethod _DeliveryMethod;
        protected byte _Sequence;

        protected byte _Padding_1;
        protected byte _Padding_2;
        protected byte _Padding_3;

        // ----------------------------------------------------------------------
        // Accessors ------------------------------------------------------------
        // ----------------------------------------------------------------------

        public virtual COMMAND_TYPE Type { get => _Type; }
        public virtual QUEUE_NAME InQueue { get => _InQueue ; }
        public virtual QUEUE_NAME OutQueue { get => _OutQueue; }
        public virtual COMMAND_TYPE NetCallback { get => _NetCallback; set => _NetCallback = value; }
        public virtual byte Origin { get => _Origin; set => _Origin = value; }
        public virtual byte Destination { get => _Destination; set => _Destination = value; }
        public virtual byte Subject { get => _Subject; set => _Subject = value; }
        public NetDeliveryMethod DeliveryMethod { get => _DeliveryMethod; set => _DeliveryMethod = value; }
        public byte Sequence { get => _Sequence; set => _Sequence = value; }

        // ----------------------------------------------------------------------
        // Constructors ---------------------------------------------------------
        // ----------------------------------------------------------------------

        public Command()
        {
            _Type = COMMAND_TYPE.UNDEFINED;
            _InQueue = QUEUE_NAME.UNDEFINED;
            _OutQueue = QUEUE_NAME.UNDEFINED;
            _NetCallback = COMMAND_TYPE.UNDEFINED;
            _Origin = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
            _Destination = NetworkEnv.SERVER_MESSAGE_ID;
            _Subject = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
            _DeliveryMethod = NetDeliveryMethod.ReliableOrdered;
            _Sequence = 1;
            _Padding_1 = 0x0;
            _Padding_2 = 0x0;
            _Padding_3 = 0x0;
        }

        public Command(COMMAND_TYPE type, QUEUE_NAME inQueue, QUEUE_NAME outQueue, COMMAND_TYPE netCallback = COMMAND_TYPE.UNDEFINED)
        {
            _Type = type;
            _InQueue = inQueue;
            _OutQueue = outQueue;
            _NetCallback = netCallback;
            _Origin = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
            _Destination = NetworkEnv.SERVER_MESSAGE_ID;
            _Subject = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
            _DeliveryMethod = NetDeliveryMethod.ReliableOrdered;
            _Sequence = 1;
            _Padding_1 = 0x0;
            _Padding_2 = 0x0;
            _Padding_3 = 0x0;
        }

        public void SetDeliverySettings(MessageDeliverySettings settings)
        {
            _DeliveryMethod = settings.DeliveryMethod;
            _Sequence = (byte)settings.SequenceChannel;
        }

        // ----------------------------------------------------------------------
        // Abstract Methods -----------------------------------------------------
        // ----------------------------------------------------------------------

        public abstract void Serialize(ref BinaryWriter writer);
        public abstract void Deserialize(ref BinaryReader reader);
        public abstract void Execute(ref NetworkEnv env);
        public abstract bool Initialize(NetworkEnv env);

        // ----------------------------------------------------------------------
        // Virtual Methods ------------------------------------------------------
        // ----------------------------------------------------------------------

        public virtual void Copy(Command c)
        {
            this._Subject = c.Subject;
            this._DeliveryMethod = c._DeliveryMethod;
            this._Sequence = c._Sequence;
        }

        public virtual Command GetCallback()
        {
            if (this.NetCallback != COMMAND_TYPE.UNDEFINED)
            {
                Command c = null;

                c = CommandQueueManager.GetCommand(this.NetCallback);
                c.Destination = Origin;

                return c;
            }
            else
            {
                return null;
            }
        }

        public virtual void ReturnSelf(ref Dictionary<COMMAND_TYPE, CommandPool> pools)
        {
            Clear();
            pools[this.Type].Push(this);
        }

        public virtual void Print(string prependString)
        {
            Console.WriteLine("{0} - {1}", prependString, Type.ToString());
        }


        public virtual void BaseInitialize(NetworkEnv env)
        {
            _Origin = env.MessageId;
        }

        public virtual void SetDestAndSubject(byte dest, byte subject)
        {
            _Destination = dest;
            _Subject = subject;
        }

        protected virtual void BaseSerialize(ref BinaryWriter writer)
        {
            writer.Write((byte)_Type);
            writer.Write((byte)_InQueue);
            writer.Write((byte)_OutQueue);
            writer.Write((byte)_NetCallback);
            writer.Write(_Origin);
            writer.Write(_Destination);
            writer.Write(_Subject);
            writer.Write((byte)_DeliveryMethod);
            writer.Write(_Sequence);
            writer.Write(_Padding_1);
            writer.Write(_Padding_2);
            writer.Write(_Padding_3);
        }

        protected virtual void BaseDeserialize(ref BinaryReader reader)
        {
            _Type = (COMMAND_TYPE)reader.ReadByte();
            _InQueue = (QUEUE_NAME)reader.ReadByte();
            _OutQueue = (QUEUE_NAME)reader.ReadByte();
            _NetCallback = (COMMAND_TYPE)reader.ReadByte();
            _Origin = reader.ReadByte();
            _Destination = reader.ReadByte();
            _Subject = reader.ReadByte();
            _DeliveryMethod = (NetDeliveryMethod)reader.ReadByte();
            _Sequence= reader.ReadByte();
            _Padding_1 = reader.ReadByte();
            _Padding_2 = reader.ReadByte();
            _Padding_3 = reader.ReadByte();
        }

        public virtual void Clear()
        {
            _Origin = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
            _Destination = NetworkEnv.SERVER_MESSAGE_ID;
            _Subject = NetworkEnv.UNINITIALIZED_MESSAGE_ID;
        }

        protected COMMAND_TYPE protGetType(ref BinaryReader reader)
        {
            COMMAND_TYPE type = (COMMAND_TYPE)reader.ReadByte();
            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            return type;
        }

    }
}

