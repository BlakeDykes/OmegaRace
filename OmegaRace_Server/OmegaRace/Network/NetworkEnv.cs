using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;

namespace OmegaRace
{
    public abstract class NetworkEnv
    {
        public const byte SERVER_MESSAGE_ID = 0;
        public const byte LOCAL_PLAYER_1_MESSAGE_ID = 1;
        public const byte LOCAL_PLAYER_2_MESSAGE_ID = 2;
        public const byte LOCAL_ENV_MESSAGE_ID = 3;
        public const byte UNINITIALIZED_MESSAGE_ID = byte.MaxValue;
        public const byte MAX_CONNECTIONS = 2;

        //Network Simulation Settings - Pretty bad
        protected const float SIMULATED_LOSS = 0.05f;
        protected const float SIMULATED_DUPLICATE_CHANCE = 0.05f;
        protected const float SIMULATED_MINIMUM_LATENCY = 0.075f;
        protected const float SIMULATED_RANDOM_LATENCY = 0.01f;

        //// Network Simulation Settings - Not great
        //protected const float SIMULATED_LOSS = 0.05f;
        //protected const float SIMULATED_DUPLICATE_CHANCE = 0.05f;
        //protected const float SIMULATED_MINIMUM_LATENCY = 0.035f;
        //protected const float SIMULATED_RANDOM_LATENCY = 0.01f;

        //// Network Simulation Settings - Perfect
        //protected const float SIMULATED_LOSS = 0.0f;
        //protected const float SIMULATED_DUPLICATE_CHANCE = 0.0f;
        //protected const float SIMULATED_MINIMUM_LATENCY = 0.0f;
        //protected const float SIMULATED_RANDOM_LATENCY = 0.0f;

        public enum TYPE
        {
            UNDEFINED = 0,
            CLIENT = 1,
            SERVER,
            LOCAL
        }

        public enum STATUS : byte
        {
            AWAITING = 0,
            CONNECTED,
            DISCONNECTED,
            LOCAL
        }

        protected MessageHandler _MessageHandler;
        protected NetPeerConfiguration _Config;
        protected bool bShouldShutdown = false;
        protected STATUS _ConStatus;
        protected byte _MessageId;
        protected Dictionary<byte, NetConnection> _Connections;
        protected TYPE _EnvType;
        protected PlayerName _PlayerName;

        public bool IsConnected { get => _ConStatus == STATUS.CONNECTED; }
        public byte MessageId { get => _MessageId; set => _MessageId = value; }

        public TYPE EnvType { get => _EnvType; }

        public byte[] Connections { get => _Connections.Keys.ToArray(); }

        public NetworkEnv()
        {
            _MessageHandler = new MessageHandler();
            _MessageId = UNINITIALIZED_MESSAGE_ID;
            _Connections = new Dictionary<byte, NetConnection>();
        }

        protected abstract void Setup();
        public abstract void Start();
        public virtual void Shutdown()
        {
            bShouldShutdown = true;
        }
        public virtual void CreatePlayer(byte id, PlayerName name) { }
        public virtual PlayerName GetPlayerName() { return PlayerName.ALL; }

        public abstract void Update();
        public abstract void UpdateState();
        public abstract void SendData(Command c);
        protected abstract void ReceiveData(NetIncomingMessage im);
        protected abstract void ReceiveStatusChanged(NetIncomingMessage im);
        protected abstract void ReceiveDiscoveryResponse(NetIncomingMessage im);
        protected abstract void ReceiveDiscoveryRequest(NetIncomingMessage im);
        protected abstract void ProcessIncoming();
        protected virtual void Destroy()
        { }
        protected abstract void HandleConnection(NetIncomingMessage im);
        protected abstract void HandleDisconnection(NetIncomingMessage im);
        public abstract void HandleContact(int idA, int idB);

        public static string Stringify(NetworkEnv.TYPE type)
        {
            switch (type)
            {
                case TYPE.CLIENT:       return "Client";
                case TYPE.SERVER:       return "Server";
                case TYPE.LOCAL:        return "Local";

                case TYPE.UNDEFINED:
                default:                return "Undefined";
            }
        }

        public string GetName()
        {
            return Stringify(this._EnvType) + "__" + PlayerData.Stringify(this._PlayerName);
        }

        protected virtual void PrintNetworkConditions()
        {
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(" ");
            ScreenLog.Add(Colors.Green, "-- Simulated Network -------------");
            ScreenLog.Add(Colors.Green, "-----------------------------------");
            ScreenLog.Add(Colors.Green, "---------------- Loss chance - {0}%", SIMULATED_LOSS * 100.0f);
            ScreenLog.Add(Colors.Green, "----------- Duplicate chance - {0}%", SIMULATED_DUPLICATE_CHANCE * 100.0f);
            ScreenLog.Add(Colors.Green, "------------ Minimum latency - {0}ms", SIMULATED_MINIMUM_LATENCY * 1000.0f);
            ScreenLog.Add(Colors.Green, "-- Random latency deviation - {0}ms", SIMULATED_RANDOM_LATENCY * 1000.0f);
        }
    }
}
