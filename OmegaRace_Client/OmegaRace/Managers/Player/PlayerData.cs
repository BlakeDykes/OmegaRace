using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Common;
using System.Diagnostics;

namespace OmegaRace
{
    public enum PlayerName : byte
    {
        Player1,
        Player2,
        ALL
    }

    public class PlayerData
    {
        public PlayerName Name { get; private set; }
        public Ship ship { get; private set; }
        public int score { get; private set; }
        public int lives { get; private set; }
        public Azul.Color shipColor { get; private set; }
        public byte player { get; }
        public int mineCount { get; private set; }
        public int missileCount { get; private set; }
        public List<Missile> missileList { get; }

        const int MAX_MISSILE = 3;
        const int MAX_MINE = 5;
        const int MAX_LIFE = 3;

        public PlayerData(PlayerManager pMgr, PlayerName name, PlayerSettings settings, byte pId)
        {
            Name = name;
            player = pId;
            score = 0;
            lives = MAX_LIFE;
            mineCount = MAX_MINE;
            missileCount = MAX_MISSILE;
            shipColor = settings.Color;
            missileList = new List<Missile>();
        }

        public void FireMissile()
        {
            if (missileCount > 0)
            {
                missileCount--;
                Vec2 pos = ship.GetPixelPosition();
                Missile m = new Missile(new Azul.Rect(pos.X, pos.Y, 20, 5), this, shipColor);
                GameManager.AddGameObject(m);
                missileList.Add(m);
                AudioManager.PlaySoundEvent(AUDIO_EVENT.MISSILE_FIRE);
            }
        }

        public void CreateShip(PlayerManager pMgr)
        {
            PlayerSettings settings = pMgr.PlayerSettings[Name];
            ship = new Ship(this, pMgr, new Azul.Rect(settings.Spawn.X, settings.Spawn.Y, 32, 32), shipColor);
        }

        public void GiveMissile(Missile m)
        {
            if (missileList.Remove(m))
            {
                missileCount++;
            }
        }
        public Missile GetMissileByID(int id)
        {
            Missile m = missileList.Find(mis => mis.getID() == id);
            return m;
        }

        public void LayMine()
        {
            if (mineCount > 0)
            {
                mineCount--;
                Vec2 pos = ship.GetPixelPosition();

                Mine m = new Mine(new Azul.Rect(pos.X, pos.Y, 20, 20), this, shipColor);
                GameManager.AddGameObject(m);
                AudioManager.PlaySoundEvent(AUDIO_EVENT.MINE_LAYED);
            }
        }

        public void GiveMine()
        {
            if (mineCount < MAX_MINE)
            {
                mineCount++;
            }
        }

        public void AddScore()
        {
            score++;
        }

        public void SubtractLife()
        {
            lives--;
        }

        public void ResetLives()
        {
            lives = MAX_LIFE;
        }

        public static string Stringify(PlayerName name)
        {
            switch (name)
            {
                case PlayerName.Player1:    return "Player_1";
                case PlayerName.Player2:    return "Player_2";
                case PlayerName.ALL:        return "All";
                default:                    return null;
            }
        }
    }
}
