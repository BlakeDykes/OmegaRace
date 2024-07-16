using System;
using System.Diagnostics;

namespace OmegaRace
{
    public abstract class SceneGameLoop
    {
        public abstract void Update();
        public abstract void Draw(ref DisplayManager displayMgr, ref PlayerManager playerMgr);
    }
}
