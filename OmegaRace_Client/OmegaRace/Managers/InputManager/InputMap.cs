using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OmegaRace
{
    public abstract class InputMap
    {
        protected Azul.AZUL_KEY[] _HorizontalAxis;
        protected Azul.AZUL_KEY[] _VerticalAxis;
        protected Azul.AZUL_KEY _FireKey;
        protected Azul.AZUL_KEY _MineKey;
        protected byte _PlayerId;

        public Azul.AZUL_KEY[] HorizontalAxis { get => _HorizontalAxis; protected set => _HorizontalAxis = value; }
        public Azul.AZUL_KEY[] VerticalAxis { get => _VerticalAxis; protected set => _VerticalAxis = value; }
        public Azul.AZUL_KEY FireKey { get => _FireKey; protected set => _FireKey = value; }
        public Azul.AZUL_KEY MineKey{ get => _MineKey; protected set => _MineKey = value; }
        public byte PlayerId { get => _PlayerId; set => _PlayerId = value; }

        public InputMap(byte pId)
        {
            _PlayerId = pId;
        }

        public virtual void HandleInputs(Dictionary<Azul.AZUL_KEY, KeyState> Keys)
        {
            int horizontalAxis = 0;
            int verticalAxis = 0;

            if ((horizontalAxis = InputManager.GetAxis(HorizontalAxis)) != 0 || (verticalAxis = InputManager.GetAxis(VerticalAxis)) != 0)
            {
                C_PlayerMove moveCommand = (C_PlayerMove)CommandQueueManager.GetCommand(COMMAND_TYPE.C_PlayerMove);
                moveCommand.Set(horizontalAxis, verticalAxis, _PlayerId);

                CommandQueueManager.RouteOut(moveCommand);
            }

            if (InputManager.GetButtonDown(this.FireKey))
            {
                CommandQueueManager.RouteOut(COMMAND_TYPE.C_PlayerRequestFire, PlayerId);
            }

            if (InputManager.GetButtonDown(this.MineKey))
            {
                CommandQueueManager.RouteOut(COMMAND_TYPE.C_PlayerRequestLayMine, PlayerId);
            }
        }

        protected abstract void InitializeKeys();
    }
}
