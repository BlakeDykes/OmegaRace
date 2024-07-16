using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace OmegaRace
{

    public class C_PlayerMove : Command
    {
        private int _HorzInput;
        private int _VertInput;

        public C_PlayerMove()
            : base(COMMAND_TYPE.C_PlayerMove, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        {
            _HorzInput = 0;
            _VertInput = 0;
        }

        public override void Execute(ref NetworkEnv env)
        {
            PlayerData player = GameSceneCollection.ScenePlay.PlayerMgr[Subject];

            player.ship.Move(_VertInput);
            player.ship.Rotate(_HorzInput);
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);

            writer.Write(_HorzInput);
            writer.Write(_VertInput);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);

            _HorzInput = reader.ReadInt32();
            _VertInput = reader.ReadInt32();
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);

            return (_Subject != NetworkEnv.UNINITIALIZED_MESSAGE_ID && _Subject != NetworkEnv.SERVER_MESSAGE_ID);
        }

        public void Set(int horizontalAxis, int verticalAxis, byte subject)
        {
            _Subject = subject;
            _HorzInput = horizontalAxis;
            _VertInput = verticalAxis;
        }

        public override void Copy(Command c)
        {
            base.Copy(c);
            C_PlayerMove com = (C_PlayerMove)c;
            this._HorzInput = com._HorzInput;
            this._VertInput = com._VertInput;
        }

        public override void Print(string prependString)
        {
        }

        public override void Clear()
        {
            base.Clear();
            _HorzInput = 0;
            _VertInput = 0;
        }
    }
}
