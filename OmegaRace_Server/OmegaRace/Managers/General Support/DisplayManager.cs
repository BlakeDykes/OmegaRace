using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace
{
    public struct PlayerStatsDisplay 
    {
        private string _Label;
        private Azul.Sprite _LifeDisplay;
        private Azul.Sprite _MineDisplay;
        private int _XPos;
        private int _YPos;

        public string Label { get => _Label; }
        public Azul.Sprite LifeDisplay { get => _LifeDisplay; }
        public Azul.Sprite MineDisplay { get => _MineDisplay; }
        public int XPos { get => _XPos; }
        public int YPos { get => _YPos; }

        public PlayerStatsDisplay(string label, Azul.Sprite lifeDisplay, Azul.Sprite mineDisplay, int xPos, int yPos)
        {
            _Label = label;
            _LifeDisplay = lifeDisplay;
            _LifeDisplay.angle = 90 * PhysicWorld.DEG_TO_RAD;
            _MineDisplay = mineDisplay;
            _XPos = xPos;
            _YPos = yPos;
        }
    }

    public class DisplayManager
    {
        private Dictionary<PlayerName, PlayerStatsDisplay> _PlayerStats;

        public DisplayManager()
        {
            _PlayerStats = new Dictionary<PlayerName, PlayerStatsDisplay>();

            _PlayerStats.Add( PlayerName.Player1,
                                new PlayerStatsDisplay("P1",
                                new Azul.Sprite(TextureCollection.shipTexture, new Azul.Rect(0, 0, 32, 32), new Azul.Rect(0, 0, 32, 32), Colors.Green),
                                new Azul.Sprite(TextureCollection.mineTexture, new Azul.Rect(0, 0, 12, 12), new Azul.Rect(0, 0, 20, 20), Colors.Green),
                                245, 
                                220));

            _PlayerStats.Add(PlayerName.Player2,
                                new PlayerStatsDisplay("P2",
                                new Azul.Sprite(TextureCollection.shipTexture, new Azul.Rect(0, 0, 32, 32), new Azul.Rect(0, 0, 32, 32), Colors.ColdBlue),
                                new Azul.Sprite(TextureCollection.mineTexture, new Azul.Rect(0, 0, 12, 12), new Azul.Rect(0, 0, 20, 20), Colors.ColdBlue),
                                455, 
                                220));
        }

        public void DisplayHUD(PlayerData[] players)
        {
            foreach(PlayerData player in players)
            {
                ShowPlayerStats(player, _PlayerStats[player.Name]);
            }
        }

        void ShowPlayerStats( PlayerData player, PlayerStatsDisplay display)
        {
            TextureCollection.scoreFont.Render( display.Label + " WINS: " + player.score, display.XPos, display.YPos);

            for (int i = 0; i < player.lives; i++)
            {
                display.LifeDisplay.x = display.XPos + 14 + (i * 36);
                display.LifeDisplay.y = display.YPos + 65;
                display.LifeDisplay.Update();
                display.LifeDisplay.Render();
            }

            for (int i = 0; i < player.mineCount; i++)
            {
                display.MineDisplay.x = display.XPos - 10 + (i * 30);
                display.MineDisplay.y = display.YPos + 30;
                display.MineDisplay.Update();
                display.MineDisplay.Render();
            }
        }
    }
}
