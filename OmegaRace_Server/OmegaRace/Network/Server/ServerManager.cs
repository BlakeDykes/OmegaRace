using System;
using System.Linq;
using Lidgren.Network;

namespace OmegaRace
{
    public class ServerManager : NetworkEnv
    {
        private NetServer _Server;
        private ServerState _State;
        private int _PlayerCount;
        bool _SimulatingNetwork;

        public ServerState State { set => _State = value; }

        public ServerManager()
        {
            _State = new ServerState_Listening();
            _PlayerName = PlayerName.ALL;
            _EnvType = TYPE.SERVER;
            _SimulatingNetwork = false;

            Setup();
            Start();
        }

        protected override void Setup()
        {
            this._MessageId = SERVER_MESSAGE_ID;
            this._Config = new NetPeerConfiguration(NetworkEnvManager.APP_ID);

            _Config.AutoFlushSendQueue = true;
            _Config.AcceptIncomingConnections = true;
            _Config.MaximumConnections = 100;
            _Config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            _Config.EnableMessageType(NetIncomingMessageType.Data);
            _Config.EnableMessageType(NetIncomingMessageType.StatusChanged);
            _Config.Port = NetworkEnvManager.SERVER_PORT;

            _Server = new NetServer(_Config);

            this._ConStatus = STATUS.DISCONNECTED;
        }

        public override void Start()
        {
            _Server.Start();

            this._ConStatus = STATUS.AWAITING;

            Console.WriteLine("Server waiting for client");
        }

        public override void Update()
        {
            if (_SimulatingNetwork)
            {
                PrintNetworkConditions();
            }

            ProcessIncoming();
        }

        public override void SendData(Command c)
        {
            _MessageHandler.SetData(c);

            NetOutgoingMessage om = _Server.CreateMessage();
            om.Write(_MessageHandler.Data);

            _Server.SendMessage(om, 
                                _Connections[c.Destination], 
                                c.DeliveryMethod, 
                                c.Sequence);
        }

        protected override void ReceiveData(NetIncomingMessage im)
        {
            _MessageHandler.RouteCommand(im.ReadBytes(im.LengthBytes));
        }

        protected override void ReceiveStatusChanged(NetIncomingMessage im)
        {
            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
            string reason = im.ReadString();

            switch (status)
            {
                case NetConnectionStatus.Connected:
                    {
                        HandleConnection(im);
                        break;
                    }

                case NetConnectionStatus.Disconnecting:
                case NetConnectionStatus.Disconnected:
                    {
                        HandleDisconnection(im);
                        break;
                    }

                case NetConnectionStatus.None:
                case NetConnectionStatus.InitiatedConnect:
                case NetConnectionStatus.ReceivedInitiation:
                case NetConnectionStatus.RespondedAwaitingApproval:
                case NetConnectionStatus.RespondedConnect:
                default:
                    break;
            }

            Console.WriteLine("Connection Status Change - {0}\n--{1}", status, reason);
        }

        protected override void ReceiveDiscoveryResponse(NetIncomingMessage im)
        {

        }

        protected override void ReceiveDiscoveryRequest(NetIncomingMessage im)
        {
            if(_Server.ConnectionsCount < 2)
            {
                NetOutgoingMessage om = _Server.CreateMessage();
                om.Write("Local server responding to discovery request");
                _Server.SendDiscoveryResponse(om, im.SenderEndPoint);
            }
        }

        protected override void ProcessIncoming()
        {
            NetIncomingMessage im;

            while (!bShouldShutdown && (im = _Server.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        ReceiveData(im);
                        break;

                    case NetIncomingMessageType.DiscoveryResponse:
                        ReceiveDiscoveryResponse(im);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        ReceiveStatusChanged(im);
                        break;

                    case NetIncomingMessageType.DiscoveryRequest:
                        ReceiveDiscoveryRequest(im);
                        break;

                    case NetIncomingMessageType.Error:
                    case NetIncomingMessageType.UnconnectedData:
                    case NetIncomingMessageType.ConnectionApproval:
                    case NetIncomingMessageType.Receipt:
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.NatIntroductionSuccess:
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                    default:
                        Console.WriteLine("{0} - {1}", im.MessageType, im.ToString());
                        break;
                }
            }
        }

        protected override void HandleConnection(NetIncomingMessage im)
        {
            _ConStatus = STATUS.CONNECTED;

            Console.WriteLine("{0} - {1}", im.MessageType, im.ToString());

            if (_Server.ConnectionsCount <= MAX_CONNECTIONS)
            {
                C_SetPlayerMessageId com = (C_SetPlayerMessageId)CommandQueueManager.GetCommand(COMMAND_TYPE.C_SetPlayerMessageId);
                _Connections.Add((byte)(_Server.ConnectionsCount), im.SenderConnection);
                com.PlayerMessageId = (byte)(_Server.ConnectionsCount);
                com.Destination = com.PlayerMessageId;
                com.NetCallback = COMMAND_TYPE.C_CreatePlayer;
                CommandQueueManager.RouteOut(com);
            }
        }

        protected override void HandleDisconnection(NetIncomingMessage im)
        {
            Console.WriteLine("{0} - {1}", im.MessageType, im.ToString());

            if (_Server.ConnectionsCount == 0)
            {
                _ConStatus = STATUS.AWAITING;
                _State = new ServerState_Listening();
            }
        }

        public override void UpdateState()
        {
            _State.Update(_Connections.Keys.ToArray());
        }

        public override void HandleContact(int idA, int idB)
        {
            C_AcceptCollision com = (C_AcceptCollision)CommandQueueManager.GetCommand(COMMAND_TYPE.C_AcceptCollision);
            com.Set(idA, idB);
            CommandQueueManager.Broadcast(true, com);
        }

        public override void CreatePlayer(byte pId, PlayerName name)
        {
            if(_PlayerCount < MAX_CONNECTIONS)
            {
                GameManager.AddPlayer(pId, name);
                
                if(++_PlayerCount == MAX_CONNECTIONS)
                {
                    C_StartClientGame startCom = (C_StartClientGame)CommandQueueManager.GetCommand(COMMAND_TYPE.C_StartClientGame);
                    startCom.PlayerIds = _Connections.Keys.ToArray();

                    CommandQueueManager.Broadcast(false, startCom);
                    _State = new ServerState_Connected();
                    GameManager.StartServer();

                    SimulateNetwork();
                }
            }
        }

        private void SimulateNetwork()
        {
            _Server.Configuration.SimulatedLoss = SIMULATED_LOSS;
            _Server.Configuration.SimulatedDuplicatesChance = SIMULATED_DUPLICATE_CHANCE;
            _Server.Configuration.SimulatedMinimumLatency = SIMULATED_MINIMUM_LATENCY;
            _Server.Configuration.SimulatedRandomLatency = SIMULATED_RANDOM_LATENCY;

            _SimulatingNetwork = true;
        }
    }
}

