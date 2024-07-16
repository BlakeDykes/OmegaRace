using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace
{
    public abstract class GameScene
    {
        public abstract void Enter();
        public abstract void Update();
        public abstract void Draw();
        public abstract void Leave();
    }
}
