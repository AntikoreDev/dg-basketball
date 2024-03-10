using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.BBMod
{
    public class QuadLaserEx : Gun
    {
        public QuadLaserEx(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 3;
            this._ammoType = new AT9mm();
            this._type = "gun";
            this.graphic = new Sprite("quadLaser", 0f, 0f);
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(20f, 8f);
            this._fireSound = "pistolFire";
            this._kickForce = 3f;
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._holdOffset = new Vec2(2f, -2f);
            this._bio = "Stop moving...";
            this._editorName = "Quad Laser";
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                Vec2 barrel = this.Offset(base.barrelOffset);
                if (base.isServerForObject)
                {
                    QuadLaserBulletEx b = new QuadLaserBulletEx(barrel.x, barrel.y, base.barrelVector);
                    b.killThingType = base.GetType();
                    Level.Add(b);
                    if (base.duck != null)
                    {
                        base.duck.hSpeed = -base.barrelVector.x * 8f;
                        base.duck.vSpeed = -base.barrelVector.y * 4f - 2f;
                        b.responsibleProfile = base.duck.profile;
                    }
                }
                this.ammo--;
                SFX.Play("laserBlast", 1f, 0f, 0f, false);
            }
        }
    }
}
