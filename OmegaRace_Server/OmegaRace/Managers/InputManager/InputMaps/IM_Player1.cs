using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OmegaRace
{
    public class IM_Player1 : InputMap
    {
        public IM_Player1(byte pId = NetworkEnv.UNINITIALIZED_MESSAGE_ID)
            : base(pId)
        {
            InitializeKeys();
        }

        protected override void InitializeKeys()
        {
            HorizontalAxis = new Azul.AZUL_KEY[]
            {
                Azul.AZUL_KEY.KEY_D,
                Azul.AZUL_KEY.KEY_A
            };  

            VerticalAxis = new Azul.AZUL_KEY[]
            {
                Azul.AZUL_KEY.KEY_W,
                Azul.AZUL_KEY.KEY_S
            };

            FireKey = Azul.AZUL_KEY.KEY_F;
            MineKey = Azul.AZUL_KEY.KEY_C;
        }
    }
}
