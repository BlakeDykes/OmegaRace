using System;
using System.Diagnostics;

namespace OmegaRace
{
    public class GameSceneConnecting : GameScene
    {

        private InputMap _InputMap;

        public InputMap InputMap { get => _InputMap; set => _InputMap = value; }

        public GameSceneConnecting()
        {
        }

        public override void Draw()
        {
        }

        public override void Enter()
        {
        }

        public override void Leave()
        {
        }

        public override void Update()
        {
            ScreenLog.Add("------ Waiting for two players ------");
        }

        public void SetInputMapId(byte playerId)
        {
            _InputMap.PlayerId = playerId;
            InputManager.AddMap(_InputMap);
        }
    }
}
