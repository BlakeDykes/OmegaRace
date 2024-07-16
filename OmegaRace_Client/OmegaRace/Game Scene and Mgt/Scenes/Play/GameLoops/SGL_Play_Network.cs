using System;
using System.Diagnostics;
using System.Linq;

namespace OmegaRace
{
    public class SGL_Play_Network_Player : SceneGameLoop
    {
        public override void Draw(ref DisplayManager displayMgr, ref PlayerManager playerMgr)
        {
            displayMgr.DisplayHUD(playerMgr.PlayerData.Values.ToArray());
        }

        public override void Update()
        {
        }
    }

    public class SGL_Play_Network_Server : SceneGameLoop
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
