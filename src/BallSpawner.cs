using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    [EditorGroup("Basketball Mod|spawners")]
    class BallSpawner : Thing
    {
        public BallSpawner(float xpos, float ypos) : base(0f, 0f, null)
        {
            this.graphic = new Sprite(GetPath("basketSpawner"), 0f, 0f);
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            base.depth = 0.9f;
            base.layer = Layer.Foreground;
            this._editorName = "Ball Spawner";
            this._canFlip = false;
        }

        public override void Draw()
        {
            if (!(Level.current is Editor))
            {
                return;
            }
            base.Draw();
        }

        public override void Update()
        {
            if (!BasketMode.isBasket)
            {
                Level.Remove(this);
            }
            if (!BasketMode.ballsAvailable)
            {
                if ((Network.isActive && Network.isServer) || !Network.isActive)
                { 
                    BasketballEx b = new BasketballEx(0,0);
                    b.x = this.x;
                    b.y = this.y;
                    Level.Add(b);
                }
            }
            base.Update();
        }
    }
}
