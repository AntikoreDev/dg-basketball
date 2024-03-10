using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    public class QuadLaserBulletEx : Thing, ITeleport
    {
        public Vec2 travel
        {
            get
            {
                return this._travel;
            }
            set
            {
                this._travel = value;
            }
        }

        public QuadLaserBulletEx(float xpos, float ypos, Vec2 travel) : base(xpos, ypos, null)
        {
            this._travel = travel;
            this.collisionOffset = new Vec2(-1f, -1f);
            this._collisionSize = new Vec2(2f, 2f);
        }

        public override void Update()
        {
            this.timeAlive += 0.016f;
            this.position += this._travel * 0.5f;
            if (base.isServerForObject && (base.x > Level.current.bottomRight.x + 200f || base.x < Level.current.topLeft.x - 200f))
            {
                Level.Remove(this);
            }
            IEnumerable<MaterialThing> things = Level.CheckRectAll<MaterialThing>(base.topLeft, base.bottomRight);
            foreach (MaterialThing t in things)
            {
                if ((this.safeFrames <= 0 || t != this.safeDuck) && t.isServerForObject)
                {
                    bool wasDestroyed = t.destroyed;
                    if (t is Duck)
                    {
                        Duck d = t as Duck;
                        this.KillDuck(d);
                    }
                    else if (t is RagdollPart)
                    {
                        RagdollPart r = t as RagdollPart;
                        Duck d = r._doll._duck;
                        r._doll.Unragdoll();
                        this.KillDuck(d);
                    }
                    else if (!(t is CookedDuck))
                    {
                        t.Destroy(new DTIncinerate(this));
                    }
                    if (t.destroyed && !wasDestroyed)
                    {
                        if (Recorder.currentRecording != null)
                        {
                            Recorder.currentRecording.LogAction(2);
                        }
                        if (t is Duck && !(t as Duck).dead)
                        {
                            Recorder.currentRecording.LogBonus();
                        }
                    }
                }
            }
            if (this.safeFrames > 0)
            {
                this.safeFrames--;
            }
            base.Update();
        }

        public void KillDuck(Duck d)
        {
            BasketDuckManager _bdm = null;
            foreach (BasketDuckManager bdm in Level.current.things[typeof(BasketDuckManager)])
            {
                if (bdm.myDuck == d)
                {
                    _bdm = bdm;
                }
            }
            if (_bdm != null)
            {
                _bdm.FireRespawn(d);
            }
        }

        public override void Draw()
        {
            Graphics.DrawRect(this.position + new Vec2(-4f, -4f), this.position + new Vec2(4f, 4f), new Color(255 - (int)(this._wave.normalized * 90f), 137 + (int)(this._wave.normalized * 50f), 31 + (int)(this._wave.normalized * 30f)), base.depth, true, 1f);
            Graphics.DrawRect(this.position + new Vec2(-4f, -4f), this.position + new Vec2(4f, 4f), new Color(255, 224 - (int)(this._wave2.normalized * 150f), 90 + (int)(this._wave2.normalized * 50f)), base.depth + 1, false, 1f);
            base.Draw();
        }

        public StateBinding _positionBinding = new CompressedVec2Binding("position", int.MaxValue, false, true);
        public StateBinding _travelBinding = new CompressedVec2Binding("travel", 20);
        private Vec2 _travel;
        private SinWave _wave = 0.5f;
        private SinWave _wave2 = 1f;
        public int safeFrames;
        public Duck safeDuck;
        public float timeAlive;
    }
}
