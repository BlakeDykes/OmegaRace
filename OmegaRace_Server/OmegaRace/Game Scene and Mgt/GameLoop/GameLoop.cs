using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OmegaRace
{
    abstract class GameLoop
    {
        public abstract void Update(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM);
        public abstract void Draw(ref List<GameObject> gameObjList, ref GameSceneFSM gameSceneFSM);
    }
}
