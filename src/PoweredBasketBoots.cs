using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    [EditorGroup("Basketball Mod|equipment")]
    class PoweredBasketBoots : Boots
    {
        public int amountJumps = 8;
        public bool canJump;
        public PoweredBasketBoots(float xpos, float ypos) : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite(GetPath("poweredBasketBootsPickup"), 0f, 0f);
            this._sprite = new SpriteMap(GetPath("poweredBasketBoots"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 13f);
            this._equippedDepth = 1;
            this.canJump = true;
        }

        public override void Update()
        {
            if (equippedDuck != null)
            {
                Duck d = equippedDuck;
                if (d.inputProfile.Pressed("JUMP") && this.canJump && !d.grounded && d.framesSinceJump > 1)
                {
                    d.vSpeed = -4.5f;
                    if (Network.isActive)
                    {
                        this._netJumpSound.Play();
                    }
                    SFX.Play("jump", 0.5f, 0.1f);
                    this.amountJumps--;
                    if (this.amountJumps <= 0)
                    {
                        this.canJump = false;
                        this.amountJumps = 0;
                    }
                }
                if (d.grounded && !canJump)
                {
                    this.amountJumps = 8;
                    this.canJump = true;
                }
            }
            else
            {
                this.amountJumps = 8;
                this.canJump = true;
            }
            base.Update();
        }

        public StateBinding _netJumpSoundBinding = new NetSoundBinding("_netJumpSound");
        public NetSoundEffect _netJumpSound = new NetSoundEffect(new string[]
        {
             "jump"
        })
        {
            volume = 0.5f,
            pitch = 0.1f
        };
    }
}
