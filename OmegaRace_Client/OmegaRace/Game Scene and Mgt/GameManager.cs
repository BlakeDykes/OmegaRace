using System.Collections.Generic;

namespace OmegaRace
{
    public class GameManager
    {
        private static GameManager instance = null;
        private static GameManager Instance()
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }

        List<GameObject> destroyList;
        List<GameObject> gameObjList;

        GameSceneFSM gameSceneFSM;

        GameLoop _GameLoop;

        public int frameCount = 0;
        public static int GetFrameCount() { return Instance().frameCount; }

        private GameManager()
        {
            destroyList = new List<GameObject>();
            gameObjList = new List<GameObject>();

            gameSceneFSM = new GameSceneFSM();

            _GameLoop = new GL_Local();

        }
        public static void SetMode()
        {
           Instance().gameSceneFSM.ChangeTo(GameSceneCollection.SceneTitlePage);
        }

        public static void ChangeState(GameScene st)
        {
            Instance().gameSceneFSM.ChangeTo(st);
        }

        public static void Update()
        {
            Instance().pUpdate();
        }

        private void pUpdate()
        {
            ScreenLog.Add(string.Format("Frame: {0}", frameCount));
            ScreenLog.Add("");

            _GameLoop.Update(ref gameObjList, ref gameSceneFSM);

            CleanUp();

            frameCount++;
        }

        public static void Draw()
        {
            Instance().pDraw();
        }

        private void pDraw()
        {
            _GameLoop.Draw(ref gameObjList, ref gameSceneFSM);
        }

        public static GameObject Find(int id)
        {
            GameObject toReturn = null;

            foreach (GameObject obj in Instance().gameObjList)
            {
                if (obj.getID() == id)
                {
                    toReturn = obj;
                    break;
                }
            }

            return toReturn;
        }

        public static void AddGameObject(GameObject obj)
        {
            Instance().gameObjList.Add(obj);
        }

        public static void CleanUp()
        {
            foreach (GameObject obj in Instance().destroyList)
            {
                Instance().gameObjList.Remove(obj);
                obj.Destroy();
            }

            Instance().destroyList.Clear();
        }

        public static void DestroyObject(GameObject obj)
        {
            if (obj.isAlive()) // Make sure to add once only
            {
                obj.setAlive(false);
                Instance().destroyList.Add(obj);
            }
        }

        public static void AddPlayer(byte playerId, PlayerName name)
        {
            GameSceneCollection.ScenePlay.AddPlayer(playerId, name);
        }

        public static void AddPlayer(byte playerId)
        {
            GameSceneCollection.SceneConnecting.SetInputMapId(playerId);

            ClientManager client = (ClientManager)NetworkEnvManager.GetEnv();

            GameSceneCollection.ScenePlay.AddPlayer(playerId, client.PlayerName);
        }

        /// <summary>
        /// Initializes and starts a local game
        /// </summary>
        public static void StartLocal()
        {
            GameSceneCollection.ScenePlay.InitializeLocalTwoPlayer();
            NetworkEnvManager.InitializeLocalEnv();
            PredictionManager.Initialize(PREDICTION_TYPE.PosP_Null);

            GameManager.ChangeState(GameSceneCollection.ScenePlay);
        }

        /// <summary>
        /// Initializes a ClientManager network environment and a client play scene
        /// </summary>
        public static void InitializeClient(PlayerName playerName)
        {
            NetworkEnvManager.InitializeClientEnv(playerName);
            PredictionManager.Initialize(PREDICTION_TYPE.PosP_ClientSide);

            GameManager.ChangeState(GameSceneCollection.SceneConnecting);
            GameSceneCollection.ScenePlay.InitializeClient();

            Instance()._GameLoop = new GL_Connecting();
        }

        public static void StartClient(byte[] playerIds)
        {
            GameSceneCollection.ScenePlay.InitializeTwoPlayer(playerIds);
            GameManager.ChangeState(GameSceneCollection.ScenePlay);
            Instance()._GameLoop = new GL_Network_Client();

            GameSceneCollection.ScenePlay.StartClient();
        }

        public static void StartPlayback(NetworkEnv.TYPE envType, PlayerName playerName = PlayerName.ALL)
        {
            NetworkEnvManager.InitializeLocalEnv();

            switch (envType)
            {
                case NetworkEnv.TYPE.CLIENT:
                    PredictionManager.Initialize(PREDICTION_TYPE.PosP_ClientSide);
                    break;

                case NetworkEnv.TYPE.UNDEFINED:
                case NetworkEnv.TYPE.LOCAL:
                case NetworkEnv.TYPE.SERVER:
                default:
                    PredictionManager.Initialize(PREDICTION_TYPE.PosP_Null);
                    break;
            }

            GameManager.ChangeState(GameSceneCollection.ScenePlay);

            Instance()._GameLoop = new GL_Network_Client();

            PlaybackManager.StartPlayback(envType, playerName);

            GameSceneCollection.ScenePlay.StartPlayback(envType);
        }
    }
}
