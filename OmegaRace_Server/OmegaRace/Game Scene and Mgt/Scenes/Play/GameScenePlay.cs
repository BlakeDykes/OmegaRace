using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OmegaRace
{
    public class GameScenePlay : GameScene
    {
        public PlayerManager PlayerMgr;
        public QueueBase _InQueue { get; private set; }
        public QueueBase _OutQueue { get; private set; }

        private SceneGameLoop GameLoop;

        DisplayManager DisplayMgr;

        public GameScenePlay()
        {
            PlayerMgr = new PlayerManager();
            DisplayMgr = new DisplayManager();
        }

        public override void Enter()
        {
            LoadLevel();
        }

        public override void Update()
        {
            GameLoop.Update();

            //Queue processing goes here
            _InQueue.ProcessIn();
            _OutQueue.ProcessOut();

            /* Screen log example
            ScreenLog.Add(string.Format("Frame Time: {0:0.0}", 1 / TimeManager.GetFrameTime()));
            ScreenLog.Add(Colors.DarkKhaki, string.Format("P1 ammo: {0}", PlayerMgr.P1Data.missileCount));
            ScreenLog.Add(Colors.Orchid, string.Format("P2 ammo: {0}", PlayerMgr.P2Data.missileCount));
            //*/
        }
        public override void Draw()
        {
            GameLoop.Draw(ref DisplayMgr, ref PlayerMgr);
        }

        public override void Leave()
        {
        }

        void LoadLevel()
        {
            PlayerData player1 = PlayerMgr.GetPlayerData(PlayerName.Player1);
            Debug.Assert(player1 != null);
            player1.CreateShip(PlayerMgr);
            GameManager.AddGameObject(player1.ship);

            PlayerData player2 = PlayerMgr.GetPlayerData(PlayerName.Player2);
            Debug.Assert(player2 != null);
            player2.CreateShip(PlayerMgr);
            GameManager.AddGameObject(player2.ship);

            // Fence OutsideBox

            GameManager.AddGameObject(new Fence(new Azul.Rect(100, 5, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 5, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 5, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(700, 5, 8, 200), 90));
  
            GameManager.AddGameObject(new Fence(new Azul.Rect(100, 495, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 495, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 495, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(700, 495, 8, 200), 90));

            GameManager.AddGameObject(new Fence(new Azul.Rect(5, 125, 8, 250), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(5, 375, 8, 250), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(795, 125, 8, 250), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(795, 375, 8, 250), 0));

            // Fence InsideBox
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 170, 10, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 170, 10, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 330, 10, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 330, 10, 200), 90));

            GameManager.AddGameObject(new Fence(new Azul.Rect(200, 250, 10, 160), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(600, 250, 10, 160), 0));


            // OutsideBox
            GameManager.AddGameObject(new FencePost(new Azul.Rect(5, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(800 - 5, 5, 10, 10)));

            GameManager.AddGameObject(new FencePost(new Azul.Rect(0 + 5, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(800 - 5, 495, 10, 10)));

            GameManager.AddGameObject(new FencePost(new Azul.Rect(5, 250, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(795, 250, 10, 10)));

            // InsideBox

            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 170, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 170, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 170, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 330, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 330, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 330, 10, 10)));
        }

        public void AddPlayer(byte playerId, PlayerName name)
        {
            PlayerMgr.AddPlayer(playerId, name);
        }

        public void InitializeTwoPlayer(byte[] playerIds)
        {
            PlayerMgr.InitializeTwoPlayer(playerIds);
        }

        public void InitializeClient()
        {
            _InQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_IN, QUEUE_TYPE.NETWORK, RECORDER_TYPE.BASIC);
            _OutQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_OUT, QUEUE_TYPE.NETWORK, RECORDER_TYPE.NULL);

            NetworkEnv env = NetworkEnvManager.GetEnv();
            _InQueue.Env = env;
            _OutQueue.Env = env;

            _InQueue.StartRecord();
        }
        
        public void InitializeServer()
        {
            _InQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_IN, QUEUE_TYPE.NETWORK, RECORDER_TYPE.BASIC);
            _OutQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_OUT, QUEUE_TYPE.NETWORK, RECORDER_TYPE.NULL);

            NetworkEnv env = NetworkEnvManager.GetEnv();
            _InQueue.Env = env;
            _OutQueue.Env = env;

            _InQueue.StartRecord();
        }

        public void InitializePlayback(PlaybackHeader playbackInfo)
        {
            _InQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_IN, QUEUE_TYPE.PLAYBACK, RECORDER_TYPE.NULL);
            _OutQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_OUT, QUEUE_TYPE.PLAYBACK, RECORDER_TYPE.NULL);

            for (int i = 0; i< playbackInfo.PlayerCount; ++i)
            {
                PlaybackHeader.PlayerInfo player = playbackInfo.GetPlayerInfo(i);

                PlayerMgr.AddPlayer(player.Id, player.Name);
            }
        }

        public void StartClient()
        {
            GameLoop = new SGL_Play_Network_Player();

            CommandQueueManager.RouteOut(COMMAND_TYPE.C_TimeRequest);
        }

        public void StartServer()
        {
            GameLoop = new SGL_Play_Network_Server();
        }

        public void StartPlayback(NetworkEnv.TYPE envType)
        {
            switch (envType)
            {

                case NetworkEnv.TYPE.CLIENT:
                    GameLoop = new SGL_Play_Network_Player();
                    break;
                case NetworkEnv.TYPE.SERVER:
                    GameLoop = new SGL_Play_Network_Server();
                    break;
                case NetworkEnv.TYPE.LOCAL:
                    GameLoop = new SGL_Play_Local();
                    break;
                
                case NetworkEnv.TYPE.UNDEFINED:
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public void InitializeLocalTwoPlayer()
        {
            _InQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_IN, QUEUE_TYPE.LOCAL, RECORDER_TYPE.BASIC);
            _OutQueue = CommandQueueManager.Add(QUEUE_NAME.SCENE_PLAY_OUT, QUEUE_TYPE.LOCAL, RECORDER_TYPE.NULL);

            NetworkEnv env = NetworkEnvManager.GetEnv();
            _InQueue.Env = env;
            _OutQueue.Env = env;
  
            InputManager.AddMap(new IM_Player1(NetworkEnv.LOCAL_PLAYER_1_MESSAGE_ID));
            InputManager.AddMap(new IM_Player2(NetworkEnv.LOCAL_PLAYER_2_MESSAGE_ID));
            GameLoop = new SGL_Play_Local();

            PlayerMgr.AddPlayer(NetworkEnv.LOCAL_PLAYER_1_MESSAGE_ID, PlayerName.Player1);
            PlayerMgr.AddPlayer(NetworkEnv.LOCAL_PLAYER_2_MESSAGE_ID, PlayerName.Player2);

            _InQueue.StartRecord();
        }
    }
}
