using System;
using System.Diagnostics;
using System.Collections.Generic;
using Lidgren.Network;

namespace OmegaRace
{
    class ServerState_Connected : ServerState
    {
        List<Command> UpdatePosCommands;
        MessageDeliverySettings PositionDeliverySettings;

        public ServerState_Connected()
        {
            UpdatePosCommands = new List<Command>();
            PositionDeliverySettings = new MessageDeliverySettings(NetDeliveryMethod.ReliableOrdered, 2);
        }

        public override void Update(byte[] playerIds)
        {
            UpdatePlayerPos(playerIds);
            UpdateMissilePos(playerIds);

            SendNewState();
        }

        private void UpdatePlayerPos(byte[] playerIds)
        {
            foreach (byte player in playerIds)
            {
                C_SetPlayerPos playerPos = (C_SetPlayerPos)CommandQueueManager.GetCommand(COMMAND_TYPE.C_SetPlayerPos);
                playerPos.Subject = player;

                UpdatePosCommands.Add(playerPos);
            }
        }

        private void UpdateMissilePos(byte[] playerIds)
        {
            foreach (byte player in playerIds)
            {
                PlayerData playerData = GameSceneCollection.ScenePlay.PlayerMgr[player];

                for (int i = 0; i < playerData.missileList.Count; ++i)
                {
                    C_SetMissilePos missilePos = (C_SetMissilePos)CommandQueueManager.GetCommand(COMMAND_TYPE.C_SetMissilePos);
                    missilePos.Subject = player;
                    missilePos.Id = playerData.missileList[i].getID();

                    UpdatePosCommands.Add(missilePos);
                }
            }
        }

        private void SendNewState()
        {
            CommandQueueManager.Broadcast(false, PositionDeliverySettings, UpdatePosCommands.ToArray());
            UpdatePosCommands.Clear();
        }
    }
}
