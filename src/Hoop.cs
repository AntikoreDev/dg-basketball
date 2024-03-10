using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    [EditorGroup("Basketball Mod")]
    public class Hoop : Thing
    {
        public HoopNet net;
        public enum HoopDirection
        {
            Top,
            Right,
            Bottom,
            Left
        }
        public HoopPart leftpart;
        public HoopPart rightpart;
        public EditorProperty<HoopDirection> Direction;
        public Hoop(float xpos, float ypos) : base(xpos, ypos)
        {
            lastpos = new Vec2(xpos, ypos);
            leftpart = new HoopPart(this);
            rightpart = new HoopPart(this);
            Direction = new EditorProperty<HoopDirection>(HoopDirection.Top, this, 0, 3, 1);
            this.graphic = new Sprite(GetPath("hoop"));
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this._editorName = "Hoop";
            this._canFlip = false;
            depth = 3;
            UpdateAngle();
        }

        public Vec2 lastpos = Vec2.Zero;

        public override void Update()
        {
            if (position != lastpos)
            {
                UpdateAngle();
            }
            net.Update();
            UpdatePartPositions();
            foreach (Nubber noob in Level.CheckCircleAll<Nubber>(position, 20))
            {
                Level.Remove(noob);
            }
            if (!Level.current.things.Contains(leftpart))
            {
                Level.Add(leftpart);
            }
            if (!Level.current.things.Contains(rightpart))
            {
                Level.Add(rightpart);
            }
            BasketballEx b = Level.CheckLine<BasketballEx>(leftpart.bottomLeft, rightpart.bottomRight, this);
            if (b != null && b.x > leftpart.x && b.x < rightpart.x)
            {
                Win(b);
            }
            base.Update();
            lastpos = position;
        }

        public override void Draw()
        {
            net.Draw();
            //debug
            //Graphics.DrawLine(leftpart.bottomLeft, rightpart.bottomRight, Color.Blue, 2);
            //Graphics.DrawLine(position, Utils.Rotate(topLeft, position, angle), Color.Green, 3);
            //Graphics.DrawLine(position, Utils.Rotate(topRight, position, angle), Color.Green, 3);
            base.Draw();
        }

        public void UpdatePartPositions()
        {
            switch (dir)
            {
                case HoopDirection.Right: leftpart.position = topLeft + new Vec2(4, -1); rightpart.position = bottomRight + new Vec2(3, -8); break;
                case HoopDirection.Left: leftpart.position = bottomLeft + new Vec2(-1, -8); rightpart.position = topRight + new Vec2(-3, -1); break;
                case HoopDirection.Bottom: leftpart.position = bottomLeft + new Vec2(0, -4); rightpart.position = bottomRight + new Vec2(0, -4); break;
                case HoopDirection.Top: leftpart.position = topLeft; rightpart.position = topRight; break;
            }
        }

        public HoopDirection dir
        {
            get
            {
                return Direction.value;
            }
        }

        public override void EditorPropertyChanged(object property)
        {
            UpdateAngle();
            base.EditorPropertyChanged(property);
        }

        public void UpdateAngle()
        {
            switch (dir)
            {
                case HoopDirection.Right: angleDegrees = 45f; break;
                case HoopDirection.Left: angleDegrees = -45f; break;
                case HoopDirection.Bottom: angleDegrees = 180f; break;
                case HoopDirection.Top: angleDegrees = 0f; break;
            }
            net = new HoopNet(Utils.Rotate(topLeft + Vec2.Unitx + 4 * Vec2.Unity, position, angle), Utils.Rotate(topRight - Vec2.Unitx + 4 * Vec2.Unity, position, angle), angle, Direction.value == HoopDirection.Bottom ? -1f : 1f);
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
