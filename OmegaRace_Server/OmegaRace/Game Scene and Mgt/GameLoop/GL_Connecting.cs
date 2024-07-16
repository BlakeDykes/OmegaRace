using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmegaRace
{
    class GL_Connecting : GameLoop
    {
        public override void Draw(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM)
        {
        }

        public override void Update(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM)
        {
            gameSceneFSM.Update();
            gameSceneFSM.TransitionState();
        }
    }
}
