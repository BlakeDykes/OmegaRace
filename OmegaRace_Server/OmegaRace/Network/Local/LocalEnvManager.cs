using Lidgren.Network;
using System;

namespace OmegaRace
{
    class LocalEnvManager : NetworkEnv
    {
        public LocalEnvManager()
        {
            this._ConStatus = STATUS.LOCAL;
            this._Connections.Add(LOCAL_ENV_MESSAGE_ID, null);
            this._MessageId = LOCAL_ENV_MESSAGE_ID;
            _PlayerName = PlayerName.ALL;
            _EnvType = TYPE.LOCAL;
        }

        public override void Update()
        {
        }

        protected override void ProcessIncoming()
        {
            throw new NotImplementedException();
        }

        public override void SendData(Command c)
        {
            CommandQueueManager.RouteIn(c);
        }

        protected override void Setup()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
        }

        protected override void ReceiveData(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        protected override void ReceiveDiscoveryRequest(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        protected override void ReceiveDiscoveryResponse(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        protected override void ReceiveStatusChanged(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        protected override void HandleConnection(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }
        protected override void HandleDisconnection(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        public override void UpdateState()
        {
        }

        public override void HandleContact(int idA, int idB)
        {
            C_AcceptCollision com = (C_AcceptCollision)CommandQueueManager.GetCommand(COMMAND_TYPE.C_AcceptCollision);
            com.Set(idA, idB);
            CommandQueueManager.RouteIn(com);
        }
    }
}
