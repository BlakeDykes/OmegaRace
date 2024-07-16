using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmegaRace
{
    class GL_Network_Server : GameLoop
    {
        public override void Draw(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM)
        {
            gameSceneFSM.Draw();

            for (int i = 0; i < gameObjList.Count; i++)
            {
                gameObjList[i].Draw();

                //Show GamObjectID on screen                 
                //ScreenLog.GetSpriteFont().Render("" + gameObjList[i].getID(), (int)gameObjList[i].GetPixelPosition().X, (int)gameObjList[i].GetPixelPosition().Y);
                //*/
            }
        }

        public override void Update(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM)
        {
            gameSceneFSM.TransitionState();
            gameSceneFSM.Update();

            for (int i = gameObjList.Count - 1; i >= 0; i--)
            {
                gameObjList[i].Update();
            }
        }
    }

    class GL_Network_Client : GameLoop
    {
        public override void Draw(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM)
        {
            gameSceneFSM.Draw();

            for (int i = 0; i < gameObjList.Count; i++)
            {
                gameObjList[i].Draw();

                //Show GamObjectID on screen                 
                //ScreenLog.GetSpriteFont().Render("" + gameObjList[i].getID(), (int)gameObjList[i].GetPixelPosition().X, (int)gameObjList[i].GetPixelPosition().Y);
                //*/
            }
        }

        public override void Update(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM)
        {
            gameSceneFSM.TransitionState();
            gameSceneFSM.Update();

            for (int i = gameObjList.Count - 1; i >= 0; i--)
            {
                gameObjList[i].Update();
            }
        }
    }

}
