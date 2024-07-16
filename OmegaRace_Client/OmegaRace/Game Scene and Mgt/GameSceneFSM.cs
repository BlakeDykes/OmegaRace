using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace
{
    public static class GameSceneCollection
    {
        public static readonly GameSceneNull SceneNull = new GameSceneNull();
        public static readonly GameSceneTitlePage SceneTitlePage = new GameSceneTitlePage();
        public static readonly GameSceneConnecting SceneConnecting = new GameSceneConnecting();
        public static readonly GameScenePlay ScenePlay = new GameScenePlay();
    }

    class GameSceneFSM
    {
        GameScene currentState;
        GameScene nextState;

        public GameSceneFSM()
        {
            currentState = GameSceneCollection.SceneNull;
            nextState = GameSceneCollection.SceneNull;
        }

        public void ChangeTo( GameScene st )
        {
            nextState = st;
        }

        public void TransitionState()
        {
            if (nextState != null)
            {
                currentState.Leave();
                currentState = nextState;
                currentState.Enter();
                nextState = null;
            }
        }

        public void Update()
        {
            currentState.Update();
        }

        public void Draw()
        {
            currentState.Draw();
        }

    }
}
