using Azul;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace OmegaRace
{
    public abstract class PhysicsObject : GameObject
    {
        // Reference to Physics Body
        private PhysicBody pBody;

        public struct PhysicsObject_Data 
        {
            bool _Active;
            PHYSICBODY_SHAPE_TYPE _PhysShape;
            bool _IsSensor;

            public bool Active { get => _Active; }
            public PHYSICBODY_SHAPE_TYPE PhysShape { get => _PhysShape; }
            public bool IsSensor { get => _IsSensor; }

            public PhysicsObject_Data(bool active, PHYSICBODY_SHAPE_TYPE shape, bool isSensor)
            {
                _Active = active;
                _PhysShape = shape;
                _IsSensor = isSensor;
            }
        }


        public PhysicsObject(GAMEOBJECT_TYPE _type, Rect textureRect, Rect screenRect, Texture text, Azul.Color c, bool usingPhysics = true, float angle = 0.0f) 
            : base(_type, textureRect, screenRect, text, c)
        {
            if (usingPhysics)
            {
                PhysicsObject_Data objectData = GetPhysicsData();

                PhysicBody_Data data = new PhysicBody_Data();
                data.position = new Vec2(screenRect.x, screenRect.y);
                data.size = new Vec2(screenRect.width, screenRect.height);
                data.active = objectData.Active;
                data.angle = angle;
                data.shape_type = objectData.PhysShape;
                data.isSensor = objectData.IsSensor;
                CreatePhysicBody(data);
            }
        }

        protected abstract PhysicsObject_Data GetPhysicsData();

        public override void Update()
        {
            if (pBody != null)
            {
                pushPhysics();
            }

            base.Update();
        }

        public Body GetBody()
        {
            return pBody.GetBody();
        }

        public float GetAngleDegs()
        {
            return pBody.GetAngleDegs();
        }

        public void CreatePhysicBody(PhysicBody_Data _data)
        {
            pBody = new PhysicBody(_data, this);
        }

        public virtual void SetPosAndAngle(float x, float y, float ang)
        {
            pBody.SetPhysicalPosition(new Vec2(x, y));
            pBody.SetAngle(ang);
        }

        public void SetPixelVelocity(Vec2 v)
        {
            pBody.SetPixelVelocity(v);
        }

        public Vec2 GetPixelVelocity()
        {
            return pBody.GetPhysicalVelocity() * PhysicWorld.METERSTOPIXELS;
        }

        public Vec2 GetPhysicalVelocity()
        {
            return pBody.GetPhysicalVelocity();
        }

        public void SetPhysicalPosition(Vec2 pixelPos)
        {
            pBody.SetPhysicalPosition(pixelPos);
        }

        public float GetAngle_Deg()
        {
            return pBody.GetAngleDegs();
        }
        public float GetAngle_Rad()
        {
            return pBody.GetAngleRads();
        }

        void pushPhysics()
        {
            Vec2 bodyPos = pBody.GetPixelPosition();
            pSprite.angle = pBody.GetAngleRads();

            pScreenRect.x = bodyPos.X;
            pScreenRect.y = bodyPos.Y;
        }

        public void SetAngle(float degrees)
        {
            pBody.SetAngle(degrees);
        }

        public void ApplyForce(Vec2 pixelForce, Vec2 pixelPoint)
        {
            pBody.ApplyForce(pixelForce, pixelPoint);
        }

        public override void Destroy()
        {
            //pSprite = null;

            if (pBody != null)
            {
                Body b = pBody.GetBody();
                if (b != null)
                {
                    PhysicWorld.GetWorld().DestroyBody(b);
                    //Debug.WriteLine("GameObject {0}'s physic body destroyed", id);
                }
            }
            pBody = null;

        }
    }
}
