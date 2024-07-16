using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

namespace OmegaRace
{
    public class Ship : PredictableObject
    {
        float maxSpeed;
        float maxForce;
        float rotateSpeed;

        Vec2 localFwd;
        Vec2 respawnPos;
        bool respawning;

        PlayerData Owner;
        PlayerManager PlMgr;


        public Ship(PlayerData owner, PlayerManager pMgr, Azul.Rect screenRect, Azul.Color color)
            : base (GAMEOBJECT_TYPE.SHIP, new Azul.Rect(0, 0, 32, 32), screenRect, TextureCollection.shipTexture, color)
        {
            Owner = owner;
            PlMgr = pMgr;
            localFwd = new Vec2(1, 0);

            // maxSpeed is m/s
            maxSpeed = 3;
            maxForce = .3f;
            rotateSpeed = 5.0f;

            respawnPos = GetPixelPosition();
        }

        public PlayerData GetOwner()
        {
            return Owner;
        }

        public override void Update()
        {
            if (respawning == false)
            {
                base.Update();
                LimitSpeed();
            }
            else // needed because we can't change the physical properties during collision processing
            {
                SetPrediction(TimeManager.GetCurrentTime(), 0.0f, respawnPos, new Vec2(0.0f, 0.0f));
                base.Update();
                respawning = false;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void Move(int vertInput)
        {
            if(vertInput < 0)
            {
                vertInput = 0;
            }
            Vec2 heading = GetBody().GetWorldVector(localFwd);
            ApplyForce(heading * vertInput * maxForce, GetPixelPosition());
        }

        public void Rotate(int horzInput)
        {
            SetAngle(GetAngleDegs() + (horzInput * -rotateSpeed));
        }

        public void LimitSpeed()
        {
            Vec2 shipVel = GetPhysicalVelocity();
            float magnitude = shipVel.Length();

            if(magnitude > maxSpeed)
            {
                shipVel.Normalize();
                shipVel *= maxSpeed;
                GetBody().SetLinearVelocity(shipVel);
            }
        }

        public void Respawn(Vec2 v)
        {
            respawning = true;
            respawnPos = v;
        }

        public Vec2 GetHeading()
        {
            return GetBody().GetWorldVector(localFwd);
        }

        public void OnHit(PlayerData other)
        {
            PlMgr.PlayerKilled(Owner, other);
        }

        public override void Accept(GameObject obj)
        {
            obj.VisitShip(this);
        }

        public override void VisitMissile(Missile m)
        {
            CollisionEvent.Action(m, this);
        }
        public override void VisitMine(Mine m)
        {
            CollisionEvent.Action(this, m);
        }

        public override void VisitFence(Fence f)
        {
            CollisionEvent.Action(f, this);
        }

        protected override PhysicsObject_Data GetPhysicsData()
        {
            return new PhysicsObject_Data(
                true,
                PHYSICBODY_SHAPE_TYPE.SHIP_MANIFOLD,
                false
                );
        }
    }
}
