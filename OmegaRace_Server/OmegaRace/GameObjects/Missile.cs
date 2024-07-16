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
    public class Missile : PredictableObject
    {
        PlayerData Owner;
        float MaxForce;
        AnimationParticle animPart;

        public Missile(Azul.Rect destRect, PlayerData owner, Azul.Color color)
            : base(GAMEOBJECT_TYPE.MISSILE, new Azul.Rect(0, 0, 24, 6), destRect, TextureCollection.missileTexture, color)
        {
            Owner = owner;
            MaxForce = 17;

            SetAngle(owner.ship.GetAngle_Deg());

            ApplyForce(owner.ship.GetHeading() * MaxForce, GetPixelPosition());

            animPart = ParticleSpawner.GetParticle(PARTICLE_EVENT.EXPLOSION, this);
        }

        public PlayerData GetOwner()
        {
            return Owner;
        }

        public void OnHit()
        {
            Owner.GiveMissile(this);
            GameManager.DestroyObject(this);
        }

        public override void Destroy()
        {
            AudioManager.PlaySoundEvent(AUDIO_EVENT.MISSILE_HIT);
            animPart.StartAnimation(pSprite.x, pSprite.y);
            base.Destroy();
        }

        public override void Accept(GameObject obj)
        {
            obj.VisitMissile(this);
        }

        public override void VisitFence(Fence f)
        {
            CollisionEvent.Action(f, this);
        }

        public override void VisitFencePost(FencePost fp)
        {
            CollisionEvent.Action(this, fp);
        }

        public override void VisitShip(Ship s)
        {
            CollisionEvent.Action(this, s);
        }

        protected override PhysicsObject_Data GetPhysicsData()
        {
            return new PhysicsObject_Data(
                true,
                PHYSICBODY_SHAPE_TYPE.DYNAMIC_BOX,
                true);
        }
    }
}
