using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OmegaRace
{
    public class GameSceneTitlePage : GameScene
    {
        public const Azul.AZUL_KEY StartLocalKey = Azul.AZUL_KEY.KEY_SPACE;
        public const Azul.AZUL_KEY JoinKey_P1 = Azul.AZUL_KEY.KEY_1;
        public const Azul.AZUL_KEY JoinKey_P2 = Azul.AZUL_KEY.KEY_2;
        public const Azul.AZUL_KEY PlaybackKey_1 = Azul.AZUL_KEY.KEY_P;

        Azul.Sprite titlePageSpr;

        public GameSceneTitlePage()
        {
            titlePageSpr = new Azul.Sprite( TextureCollection.titleScreenText, new Azul.Rect(0, 0, 800, 480), new Azul.Rect(400, 250, 800, 500));
            titlePageSpr.Update();
        }

        public override void Enter()
        {
            Program.AdjustWindow();

            AudioManager.PlaySoundEvent(AUDIO_EVENT.MINE_DESPAWN);
        }

        public override void Update()
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

            ScreenLog.Add(Colors.Green, "-- Start Game Controls ------------------------------");
            ScreenLog.Add(Colors.Green, "------------------------------------------------------");
            ScreenLog.Add(Colors.Green, "-- Press Space to start Local Game");
            ScreenLog.Add(Colors.Green, "-- Press 1 to start Player 1");
            ScreenLog.Add(Colors.Green, "-- Press 2 to start Player 2");
            
            ScreenLog.Add(" ");

            ScreenLog.Add(Colors.Coral, "-- Start Playback Controls --------------------------");
            ScreenLog.Add(Colors.Coral, "------------------------------------------------------");
            ScreenLog.Add(Colors.Coral, "-- Press p + space button to playback Local Game");
            ScreenLog.Add(Colors.Coral, "-- Press p + 1 to playback Player 1");
            ScreenLog.Add(Colors.Coral, "-- Press p + 2 to playback Player 2");


            if (InputManager.GetButtonDown(StartLocalKey))
            {
                if(InputManager.GetButton(PlaybackKey_1))
                {
                    GameManager.StartPlayback(NetworkEnv.TYPE.LOCAL);
                }
                else
                {
                    GameManager.StartLocal();
                }
            }
            else if (InputManager.GetButtonDown(JoinKey_P1))
            {
                if (InputManager.GetButton(PlaybackKey_1))
                {
                    GameManager.StartPlayback(NetworkEnv.TYPE.CLIENT, PlayerName.Player1);
                }
                else
                {
                    GameSceneCollection.SceneConnecting.InputMap = new IM_Player1();
                    GameManager.ChangeState(GameSceneCollection.SceneConnecting);
                    GameManager.InitializeClient(PlayerName.Player1);
                }
            }
            else if (InputManager.GetButtonDown(JoinKey_P2))
            {
                if (InputManager.GetButton(PlaybackKey_1))
                {
                    GameManager.StartPlayback(NetworkEnv.TYPE.CLIENT, PlayerName.Player2);
                }
                else
                {
                    GameSceneCollection.SceneConnecting.InputMap = new IM_Player2();
                    GameManager.ChangeState(GameSceneCollection.SceneConnecting);
                    GameManager.InitializeClient(PlayerName.Player2);
                }
            }
        }

        public override void Draw()
        {
            titlePageSpr.Render();
        }

        public override void Leave() { }
    }
}
