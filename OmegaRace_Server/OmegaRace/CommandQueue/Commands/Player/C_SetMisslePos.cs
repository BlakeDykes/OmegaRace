using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Box2DX.Common;

namespace OmegaRace
{
    public class C_SetMissilePos : Command
    {
        private Vec2 _Pos;
        private float _Time;
        private float _Angle;
        private int _Id;

        public int Id { get => _Id; set => _Id = value; }

        public C_SetMissilePos()
            : base(COMMAND_TYPE.C_SetMissilePos, QUEUE_NAME.SCENE_PLAY_IN, QUEUE_NAME.SCENE_PLAY_OUT)
        { }

        public void SetID(int id)
        {
            _Id = id;
        }

        public override void Execute(ref NetworkEnv env)
        {
            Missile missile = (Missile)GameSceneCollection.ScenePlay.PlayerMgr[Subject].GetMissileByID(_Id);

            if(missile != null)
            {
                missile.SetPosAndAngle(_Time, _Pos.X, _Pos.Y, _Angle);
            }
            else
            {
                base.Print("~~~~~~ Failed");
            }
        }

        public override bool Initialize(NetworkEnv env)
        {
            BaseInitialize(env);
            Missile missile = (Missile)GameSceneCollection.ScenePlay.PlayerMgr[Subject].GetMissileByID(_Id);

            if(missile != null)
            {
                _Pos = missile.GetPixelPosition();
                _Time = TimeManager.GetCurrentTime();
                _Angle = missile.GetAngle_Deg();

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
            _Id = reader.ReadInt32();
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            BaseSerialize(ref writer);

            writer.Write(_Pos.X);
            writer.Write(_Pos.Y);
            writer.Write(_Time);
            writer.Write(_Angle);
            writer.Write(_Id);
        }

        public void Set(byte dest, byte subject, int missileId)
        {
            SetDestAndSubject(dest, subject);
            _Id = missileId;
        }

        public override void Copy(Command c)
        {
            base.Copy(c);
            C_SetMissilePos com = (C_SetMissilePos)c;
            this._Pos.X = com._Pos.X;
            this._Pos.Y = com._Pos.Y;
            this._Time = com._Time;
            this._Angle = com._Angle;
            this._Id = com._Id;
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
            _Id = 0;
        }
    }
}
