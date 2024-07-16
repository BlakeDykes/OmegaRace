namespace OmegaRace
{
    public class GameSceneTitlePage : GameScene
    {
        public const Azul.AZUL_KEY StartLocalKey = Azul.AZUL_KEY.KEY_SPACE;
        public const Azul.AZUL_KEY StartServerKey = Azul.AZUL_KEY.KEY_0;
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
            ScreenLog.Add(Colors.Green, "-- Press 0 to start Server");
            
            ScreenLog.Add(" ");

            ScreenLog.Add(Colors.Coral, "-- Start Playback Controls --------------------------");
            ScreenLog.Add(Colors.Coral, "------------------------------------------------------");
            ScreenLog.Add(Colors.Coral, "-- Press p + space button to playback Local Game");
            ScreenLog.Add(Colors.Coral, "-- Press p + 0 to playback Server");


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
            else if (InputManager.GetButtonDown(StartServerKey))
            {
                if (InputManager.GetButton(PlaybackKey_1))
                {
                    GameManager.StartPlayback(NetworkEnv.TYPE.SERVER);
                }
                else
                {
                    GameManager.InitializeServer();
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
