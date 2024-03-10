using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DuckGame;

namespace DuckGame.BBMod
{
    [EditorGroup("Basketball Mod|custom")]
    class CustomHoop : Block
    {
        public EditorProperty<bool> isSolid = new EditorProperty<bool>(false, null, 0f, 1f, 0.1f, null, false, false);
        public EditorProperty<ImpactedFrom> Direction;
        public CustomHoop(float xpos, float ypos) : base(xpos, ypos)
        {
            Direction = new EditorProperty<ImpactedFrom>(ImpactedFrom.Top, this, 0, 3, 1);
            this.graphic = new Sprite(GetPath("customHoop"));
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this._editorName = "Custom Hoop";
            this._canFlip = false;
            this._solid = true;
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
            this._solid = this.isSolid;
            if (!this._solid)
            {
                this.ballsInside = Level.CheckRectAll<BasketballEx>(base.topLeft, base.bottomRight).ToList<BasketballEx>();
                if (this.ballsInside.Count > 0)
                {
                    Win(ballsInside[0] as BasketballEx);
                }
                this.particlesAbove = Level.CheckRectAll<PhysicsParticle>(base.topLeft + new Vec2(-1f, -4f), base.bottomRight + new Vec2(1f, -12f)).ToList<PhysicsParticle>();
                foreach (PhysicsParticle p in particlesAbove)
                {
                    if (p.isServerForObject)
                    {
                        typeof(PhysicsParticle).GetField("_grounded", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(p, false);
                    }
                }
            }
            base.Update();
        }

        protected List<BasketballEx> ballsInside = new List<BasketballEx>();
        protected List<PhysicsParticle> particlesAbove = new List<PhysicsParticle>();

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            bool _goalCollide = with is BasketballEx && Direction.value == from;
            if (!BasketMode.hasBasket && with != null && _goalCollide)
            {
                BasketballEx b = with as BasketballEx;
                this.Win(b);
            }
            base.OnSoftImpact(with, from);
        }

        public void Win(BasketballEx with)
        {
            if (!BasketMode.hasBasket)
            {
                BasketballEx b = with as BasketballEx;
                if (b != null && b._lastOwner != null)
                {
                    BasketMode.basketDuck = b._lastOwner;
                    BasketMode.hasBasket = true;
                    this.GoalEffects();
                }
                foreach (BasketDuckManager bdm in Level.current.things[typeof(BasketDuckManager)])
                {
                    bdm.respawnTime = 0;
                }
            }
        }

        public void GoalEffects()
        {
            if (isServerForObject)
            {
                float h = 1f;
                float v = 1f;
                for (h = 1f; h >= -1f; h--)
                {
                    for (v = 1f; v >= -1f; v--)
                    {
                        if (!(v == 0 && h == 0))
                        {
                            GoalParticle g = new GoalParticle(0, 0);
                            g.hsp = h * 2f;
                            g.vsp = v * 2f;
                            g.x = this.topLeft.x + 8f;
                            g.y = this.topLeft.y;
                            Level.Add(g);
                            base.Fondle(g);
                        }
                    }
                }
            }
        }
    }
}
