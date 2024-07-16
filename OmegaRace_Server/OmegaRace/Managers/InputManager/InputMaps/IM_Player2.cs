using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OmegaRace
{
    public class IM_Player2 : InputMap
    {
        public IM_Player2(byte pId = NetworkEnv.UNINITIALIZED_MESSAGE_ID)
            : base(pId)
        {
            InitializeKeys();
        }

        protected override void InitializeKeys()
        {
            HorizontalAxis = new Azul.AZUL_KEY[]
            {
                Azul.AZUL_KEY.KEY_L,
                Azul.AZUL_KEY.KEY_J
            };

            VerticalAxis = new Azul.AZUL_KEY[]
            {
                Azul.AZUL_KEY.KEY_I,
                Azul.AZUL_KEY.KEY_K
            };

            FireKey = Azul.AZUL_KEY.KEY_H;
            MineKey = Azul.AZUL_KEY.KEY_N;
        }
    }
}
