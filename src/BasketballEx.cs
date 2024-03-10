using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    [EditorGroup("Basketball Mod")]
    [BaggedProperty("canSpawn", false)]
    public class BasketballEx : Holdable
    {
        private SpriteMap _sprite;
        private int _framesInHand;
        private int _walkFrames;
        private Duck _bounceDuck;
        public Duck _lastOwner;
        public bool spawnedOnAir;
        public BasketballEx(float xpos, float ypos) : base(xpos, ypos)
        {
            scale = new Vec2(0.66f);
            tapeable = false;
            this._sprite = new SpriteMap("basketBall", 16, 16, false);
            this.graphic = this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(11f, 11f);
            base.depth = -0.5f;
            this.thickness = 1f;
            this.weight = 3f;
            this.flammable = 0.3f;
            base.collideSounds.Add("basketball");
            this.physicsMaterial = PhysicsMaterial.Rubber;
            this._bouncy = 0.8f;
            this.friction = 0.03f;
            this._impactThreshold = 0.1f;
            this.handOffset = new Vec2(0f, -0f);
            this._editorName = "Basketball";
        }

        public override void CheckIfHoldObstructed()
        {
            Duck duckOwner = this.owner as Duck;
            if (duckOwner != null)
            {
                duckOwner.holdObstructed = false;
                if (duckOwner.action)
                {
                    duckOwner.holdObstructed = true;
                }
            }
        }

        public override void OnPressAction()
        {
        }

        public override void Update()
        {
            if (this.owner == null)
            {
                this.angleDegrees += this.hSpeed * 3f;
                this.sleeping = false;
                this._walkFrames = 0;
                this._framesInHand--;
                this._framesInHand--;
                if (this._framesInHand < -60)
                {
                    this._bounceDuck = null;
                }
                if (this._bounceDuck != null)
                {
                    float dist = (this._bounceDuck.position - this.position).length;
                    if (dist < 16f)
                    {
                        this.hSpeed = this._bounceDuck.hSpeed;
                    }
                    if (this._bounceDuck.holdObject == null && this.vSpeed < 1f && this._bounceDuck.top + 8f > base.y && dist < 16f)
                    {
                        this._bounceDuck.GiveHoldable(this);
                        this._framesInHand = 0;
                    } 
                }
            }
            else
            {
                this._lastOwner = this.owner as Duck;
                if (this._framesInHand < 0)
                {
                    this._framesInHand = 0;
                }
                if (!this.owner.action && Math.Abs(this.owner.hSpeed) > 0.5f && this._framesInHand > 6)
                {
                    this._bounceDuck = base.duck;
                    float hspd = base.duck.hSpeed;
                    base.duck.ThrowItem(false);
                    this.vSpeed = 2f;
                    this.hSpeed = hspd * 1.1f;
                    this._framesInHand = 0;
                }
                else
                {
                    if (Math.Abs(this.owner.hSpeed) > 0.5f && base.duck.grounded)
                    {
                        this._walkFrames++;
                    }
                    else if (base.duck.grounded)
                    {
                        this._walkFrames--;
                    }
                    if (this._walkFrames < 0)
                    {
                        this._walkFrames = 0;
                    }
                    if (this._walkFrames > 20)
                    {
                        if (Network.isActive)
                        {
                            this._netWhistle.Play(1f, 0f);
                        }
                        SFX.Play("basketballWhistle", 1f, 0f, 0f, false);
                        base.duck.ThrowItem(false);
                        this._walkFrames = 0;
                    }
                    this._bounceDuck = null;
                    this._framesInHand++;
                }
            }
            if (this.y >= Level.activeLevel.lowestPoint + 90f)
            {
                this.Explode();
            }
            this.CheckForThings();
            base.Update();
        }

        public void Explode()
        {
            if (this.isServerForObject)
            {
                Level.Remove(this);
                Grenade g = new Grenade(x, y);
                g._pin = false;
                g._timer = 0;
                g.alpha = 0f;
                g.position = this.position;
                Level.Add(g);
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with != null && with is Saws)
            {
                this.Explode();
            }
            base.OnSoftImpact(with, from);
        }
        public override void SolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is HoopPart)
            {
                SFX.Play(Mod.GetPath<BBMod>("hoopbounce"));
                _didImpactSound = true;
            }
            base.SolidImpact(with, from);
        }

        public void CheckForThings()
        {
            this._collidedThings = Level.CheckRectAll<Thing>(base.topLeft, base.bottomRight).ToList<Thing>();
            foreach (Thing t in _collidedThings)
            {
                if (t is Window || t is FloorWindow)
                {
                    Window w = t as Window;
                    if (w != null)
                    {
                        w.Destroy();
                    }
                }
            }
        }
        public StateBinding _sfxBinding_whistle = new NetSoundBinding("_netWhistle");
        public NetSoundEffect _netWhistle = new NetSoundEffect(new string[]
        {
            "basketballWhistle"
        });
        protected List<Thing> _collidedThings = new List<Thing>();
        public StateBinding _lastOwnerBinding = new StateBinding("_lastOwner", -1, false, false);
    }
}
