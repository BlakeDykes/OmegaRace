using Box2DX.Common;

namespace OmegaRace
{
    public enum GAMEOBJECT_TYPE
    {
        NULL,
        SHIP,
        MISSILE,
        FENCEPOST,
        FENCE,
        MINE,
        PARTICLE
    }

    public class GameObject : Visitor
    {       
        // Each object has a type
        public GAMEOBJECT_TYPE type;

        // Reference for the Sprite
        protected Azul.Sprite pSprite;

        // Reference for the ScreenRect (i.e. Position and Size)
        protected Azul.Rect pScreenRect;

        // Refernce for the Color (tints the sprite)
        protected Azul.Color color;
        
        // Every GameObject has a unique ID number.
        static int IDNUM;
        int id;
        
        bool alive;
        

        public GameObject(GAMEOBJECT_TYPE _type, Azul.Rect textureRect, Azul.Rect screenRect, Azul.Texture text, Azul.Color c)
        {
            type = _type;
            color = c;
            pSprite = new Azul.Sprite(text, textureRect, screenRect, color);
            pScreenRect = screenRect;
            id = IDNUM++;
            alive = true;
        }

        public int getID()
        {
            return id;
        }

        public Azul.Rect getDestRect()
        {
            return pScreenRect;
        }

        public virtual void Update()
        {
            pSprite.x = pScreenRect.x;
            pSprite.y = pScreenRect.y;
            pSprite.Update();
        }

        public bool isAlive()
        {
            return alive;
        }
        public void setAlive(bool b)
        {
            alive = b;
        }

        public virtual void Draw()
        {           
            pSprite.Render();
        }

        public Vec2 GetPixelPosition()
        {
            return new Vec2(pSprite.x, pSprite.y);
        }

        public virtual void Destroy()
        { }
    }
}
