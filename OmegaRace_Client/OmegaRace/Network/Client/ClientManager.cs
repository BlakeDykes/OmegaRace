using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Lidgren.Network;
using System.Net;
using System.IO;

namespace OmegaRace
{
    public class ClientManager : NetworkEnv
    {
        private NetConnection _Server;
        private NetClient _Client;
        private bool _SimulatingNetwork;

        public PlayerName PlayerName { get => _PlayerName; set => _PlayerName = value; }

        public ClientManager(PlayerName playerName)
        {
            this._ConStatus = STATUS.DISCONNECTED;
            _PlayerName = playerName;
            _EnvType = TYPE.CLIENT;
            _SimulatingNetwork = false;

            Setup();
            Start();
        }

        protected override void Setup()
        {
            this._Config = new NetPeerConfiguration(NetworkEnvManager.APP_ID);
            _Config.AutoFlushSendQueue = true;
            _Config.AcceptIncomingConnections = true;
            _Config.MaximumConnections = 100;
            _Config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            _Config.EnableMessageType(NetIncomingMessageType.Data);
            _Config.EnableMessageType(NetIncomingMessageType.StatusChanged);

            _Config.Port = NetworkEnvManager.CLIENT_PORT;

            _Client = new NetClient(_Config);
        }

        public override void Start()
        {
            _Client.Start();

            _Client.DiscoverKnownPeer("localhost", NetworkEnvManager.SERVER_PORT);

            Console.WriteLine("Client waiting to connect");
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
            if(_Server != null && _Server.Status == NetConnectionStatus.Connected)
            {
                _MessageHandler.SetData(c);

                NetOutgoingMessage om = _Client.CreateMessage();
                om.Write(_MessageHandler.Data);

                _Client.SendMessage(om, _Server, c.DeliveryMethod, c.Sequence);
            }
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
                        Shutdown();
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
            _Client.Connect(im.SenderEndPoint);
        }

        protected override void ReceiveDiscoveryRequest(NetIncomingMessage im)
        {
        }

        protected override void ProcessIncoming()
        {
            NetIncomingMessage im;

            while (!bShouldShutdown && (im = _Client.ReadMessage()) != null)
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
            _Server = im.SenderConnection;
            _ConStatus = STATUS.CONNECTED;
            _Connections.Add(SERVER_MESSAGE_ID, im.SenderConnection);

            SimulateNetwork();
        }

        protected override void HandleDisconnection(NetIncomingMessage im)
        {
            Debug.Assert(im.SenderConnection.RemoteUniqueIdentifier == _Server.RemoteUniqueIdentifier);

            _Server = null;
            _ConStatus = STATUS.DISCONNECTED;
        }

        public override void UpdateState()
        {
        }

        public override void HandleContact(int idA, int idB)
        {
            throw new NotImplementedException();
        }

        private void SimulateNetwork()
        {
            _Client.Configuration.SimulatedLoss = SIMULATED_LOSS;
            _Client.Configuration.SimulatedDuplicatesChance = SIMULATED_DUPLICATE_CHANCE;
            _Client.Configuration.SimulatedMinimumLatency = SIMULATED_MINIMUM_LATENCY;
            _Client.Configuration.SimulatedRandomLatency = SIMULATED_RANDOM_LATENCY;

            _SimulatingNetwork = true;
        }
    }
}
