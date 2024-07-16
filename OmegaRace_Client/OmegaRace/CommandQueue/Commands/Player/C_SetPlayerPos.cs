using System;
using System.Diagnostics;
using System.IO;
using Box2DX.Common;

namespace OmegaRace
{
    public class C_SetPlayerPos : Command
    {
        protected Vec2 _Pos;
        private float _Time;
        protected float _Angle;

        public C_SetPlayerPos()
            : base(COMMAND_TYPE.C_SetPlayerPos, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        { }

        public override void Execute(ref NetworkEnv env)
        {
            Ship player = (Ship)GameSceneCollection.ScenePlay.PlayerMgr[Subject].ship;

            if (player != null)
            {
                player.SetPosAndAngle(_Time, _Pos.X, _Pos.Y, _Angle);
            }
            else
            {
                base.Print("~~~~~~ Failed");
            }
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);
            Ship player = (Ship)GameSceneCollection.ScenePlay.PlayerMgr[Subject].ship;

            if(player != null)
            {
                _Pos = player.GetPixelPosition();
                _Time = TimeManager.GetCurrentTime();
                _Angle = player.GetAngle_Deg();

                return (_Subject != NetworkEnv.UNINITIALIZED_MESSAGE_ID && _Subject != NetworkEnv.SERVER_MESSAGE_ID);
            }

            base.Print("~~~~~~ Failed");
            return false;
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            BaseDeserialize(ref reader);
            _Pos.X = reader.ReadSingle();
            _Pos.Y = reader.ReadSingle();
            _Time = reader.ReadSingle();
            _Angle = reader.ReadSingle();
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);
            writer.Write(_Pos.X);
            writer.Write(_Pos.Y);
            writer.Write(_Time);
            writer.Write(_Angle);
        }
        public override void Copy(Command c)
        {
            base.Copy(c);
            C_SetPlayerPos com = (C_SetPlayerPos)c;
            this._Pos.X = com._Pos.X;
            this._Pos.Y = com._Pos.Y;
            this._Time = com._Time;
            this._Angle = com._Angle;
        }

        public override void Print(string prependString)
        {
        }

        public override void Clear()
        {
            base.Clear();
            _Pos.SetZero();
            _Time = 0.0f;
            _Angle = 0.0f;
        }
    }
}
