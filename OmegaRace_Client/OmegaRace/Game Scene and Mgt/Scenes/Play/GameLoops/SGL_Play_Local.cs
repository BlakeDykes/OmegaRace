using System;
using System.Diagnostics;
using System.Linq;

namespace OmegaRace
{
    class SGL_Play_Local : SceneGameLoop
    {
        public override void Draw(ref DisplayManager displayMgr, ref PlayerManager playerMgr)
        {
            displayMgr.DisplayHUD(playerMgr.PlayerData.Values.ToArray());
        }

        public override void Update()
        {
            PhysicWorld.Update();
        }
    }
}
