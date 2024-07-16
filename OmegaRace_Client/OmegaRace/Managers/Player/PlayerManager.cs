using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Common;

namespace OmegaRace
{
    public struct PlayerSettings
    {
        private Vec2 _Spawn;
        private Azul.Color _Color;

        public Vec2 Spawn { get => _Spawn; set => _Spawn = value; }
        public Azul.Color Color { get => _Color; set => _Color = value; }


        public PlayerSettings(Vec2 spawn, Azul.Color color)
        {
            _Spawn = spawn;
            _Color = color;
        }
    }

    public class PlayerManager
    {
        private Dictionary<byte, PlayerData> _PlayerData;
        private Dictionary<PlayerName, PlayerSettings> _PlayerSettings;

        int midline;

        public Dictionary<PlayerName, PlayerSettings> PlayerSettings { get => _PlayerSettings; }
        public Dictionary<byte, PlayerData> PlayerData { get => _PlayerData; }
        public PlayerData this[byte pId] { get => _PlayerData[pId]; }

        public PlayerManager()
        {
            _PlayerData = new Dictionary<byte, PlayerData>();
            _PlayerSettings = new Dictionary<PlayerName, PlayerSettings>();
            midline = 250;

            _PlayerSettings.Add(PlayerName.Player1, new PlayerSettings(new Vec2(400, 100), Colors.Green));
            _PlayerSettings.Add(PlayerName.Player2, new PlayerSettings(new Vec2(400, 400), Colors.ColdBlue));
        }

        public void AddPlayer(byte playerId, PlayerName name)
        {
            PlayerSettings settings = _PlayerSettings[name];
            _PlayerData.Add(playerId, new PlayerData(this, name, settings, playerId));
        }

        public void InitializeTwoPlayer(byte[] playerIds)
        {
            Debug.Assert(_PlayerData.Count > 0);

            if(playerIds.Length > _PlayerData.Count)
            {
                PlayerName name = _PlayerData.ElementAt(0).Value.Name == PlayerName.Player1 ? PlayerName.Player2 : PlayerName.Player1;

                if (_PlayerData.ContainsKey(playerIds[0]))
                {
                    AddPlayer(playerIds[1], name);
                }
                else
                {
                    AddPlayer(playerIds[0], name);
                }
            }
        }

        public PlayerData GetPlayerData(PlayerName name)
        {
            for(int i = 0; i < _PlayerData.Count; ++i)
            {
                if(_PlayerData.ElementAt(i).Value.Name == name)
                {
                    return _PlayerData.ElementAt(i).Value;
                }
            }

            return null;
        }

        public void PlayerKilled(PlayerData playerKilled, PlayerData other)
        {
   
            // Adjust lives
            playerKilled.SubtractLife();

            // Respawn the killed player away from the live one
            if (other.ship.GetPixelPosition().Y > midline)
            {
                playerKilled.ship.Respawn(_PlayerSettings[PlayerName.Player1].Spawn);
            }
            else
            {
                playerKilled.ship.Respawn(_PlayerSettings[PlayerName.Player2].Spawn);
            }

            // If this was the end of a round, reset everything
            if (playerKilled.lives <= 0)
            {
                other.AddScore();
                playerKilled.ResetLives();
                other.ResetLives();

                foreach(KeyValuePair<byte, PlayerData> player in _PlayerData)
                {
                    player.Value.ship.Respawn(_PlayerSettings[player.Value.Name].Spawn);
                }
            }
        }
    }
}
