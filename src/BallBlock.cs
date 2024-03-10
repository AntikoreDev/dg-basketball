using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    [EditorGroup("Basketball Mod|spawners")]
    class BallBlock : Block, IPathNodeBlocker
    {
         public SpriteMap _sprite;
         public bool hit;
         public int framesFromHit;
         public bool canBounce
         {
             get
             {
                 return this._canBounce;
             }
         }
         public BallBlock(float xpos, float ypos) : base(xpos, ypos)
         {
             this._sprite = new SpriteMap(GetPath("ballSpawnerBlock"), 16, 16);
             this.graphic = _sprite;
             this.center = new Vec2(7.5f, 7.5f);
             this.collisionOffset = new Vec2(-7.5f, -7.5f);
             this.collisionSize = new Vec2(16f, 16f);
             this.hit = false;
             this._canFlip = false;
             this._editorName = "Ball Box";
         }

         public override void Update()
         {
             this._aboveList.Clear();
             if (this.startY < -9999f)
             {
                 this.startY = base.y;
             }
             this._sprite.frame = (this.hit ? 1 : 0);
             if (this.netDisarmIndex != this.localNetDisarm)
             {
                 this.localNetDisarm = this.netDisarmIndex;
                 this._aboveList = Level.CheckRectAll<PhysicsObject>(base.topLeft + new Vec2(1f, -4f), base.bottomRight + new Vec2(-1f, -12f)).ToList<PhysicsObject>();
                 foreach (PhysicsObject p in this._aboveList)
                 {
                     if (base.isServerForObject && p.owner == null)
                     {
                         base.Fondle(p);
                     }
                     if (p.isServerForObject && (p.grounded || p.vSpeed > 0f || p.vSpeed == 0f))
                     {
                         p.y -= 2f;
                         p.vSpeed = -3f;
                         Duck d = p as Duck;
                         if (d != null)
                         {
                             d.Disarm(this);
                         }
                     }
                 }
             }
             if (this.bounceAmount > 0f)
             {
                 this.bounceAmount -= 0.8f;
             }
             else
             {
                 this.bounceAmount = 0f;
             }
             base.y -= this.bounceAmount;
             if (!this._canBounce)
             {
                 if (base.y < this.startY)
                 {
                     base.y += 0.8f + Math.Abs(base.y - this.startY) * 0.4f;
                 }
                 if (base.y > this.startY)
                 {
                     base.y -= 0.8f - Math.Abs(base.y - this.startY) * 0.4f;
                 }
                 if (Math.Abs(base.y - this.startY) < 0.8f)
                 {
                     this._canBounce = true;
                     base.y = this.startY;
                 }
             }
             if (!BasketMode.ballsAvailable && this.framesFromHit == 0)
             {
                 this.hit = false;
             }
             if (this.framesFromHit > 0)
             {
                 this.framesFromHit--;
             }
             base.Update();
         }

         public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
         {
             with.Fondle(this);
             if (from == ImpactedFrom.Bottom)
             {
                 if (!hit)
                 {
                     this.hit = true;
                     if (isServerForObject)
                     {
                         BasketballEx b = new BasketballEx(0, 0);
                         b.x = base.x;
                         b.bottom = this.bottom;
                         b.y -= 12f;
                         b.vSpeed = -3.5f;
                         b.clip.Add(this);
                         Level.Add(b);
                     }
                     if (Network.isActive)
                     {
                         _netHitSound.Play();
                     }
                     SFX.Play("hitBox");
                     this.framesFromHit = 2;
                 }
                 this.Bounce();
             }
             base.OnSoftImpact(with, from);
         }

         public void Bounce()
         {
             if (this._canBounce)
             {
                 this.bounceAmount = 8f;
                 this._canBounce = false;
                 if (Network.isActive)
                 {
                     this.netDisarmIndex += 1;
                     return;
                 }
                 this._aboveList = Level.CheckRectAll<PhysicsObject>(base.topLeft + new Vec2(1f, -4f), base.bottomRight + new Vec2(-1f, -12f)).ToList<PhysicsObject>();
                 foreach (PhysicsObject p in this._aboveList)
                 {
                     if (p.grounded || p.vSpeed > 0f || p.vSpeed == 0f)
                     {
                         base.Fondle(p);
                         p.y -= 2f;
                         p.vSpeed = -3f;
                         Duck d = p as Duck;
                         if (d != null)
                         {
                             d.Disarm(this);
                         }
                     }
                 }
             }
         }
         public StateBinding _positionBinding = new StateBinding("position", -1, false);
         public StateBinding _hitBinding = new StateBinding("hit", -1 , false, false);
         public StateBinding _framesFromHitBinding = new StateBinding("framesFromHit", -1, false, false);
         public StateBinding _netHitSoundBinding = new NetSoundBinding("_netHitSound");
         public NetSoundEffect _netHitSound = new NetSoundEffect(new string[]
         {
             "hitBox"
         });
         public byte netDisarmIndex;
         public byte localNetDisarm;
         public float bounceAmount;
         public bool _canBounce = true;
         protected List<PhysicsObject> _aboveList = new List<PhysicsObject>();
         public float startY = -99999f;
    }
}

  