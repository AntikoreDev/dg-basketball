using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    class GoalParticle : Thing
    {
        private int _timer;
        public float hsp;
        public float vsp;
        public GoalParticle(float xpos, float ypos) : base(xpos, ypos, null)
        {
            this.graphic = new Sprite("dizzyStar");
            this.alpha = 1f;
        }

        public override void Update()
        {
            this.x += hsp;
            this.y += vsp;
            this._timer++;
            if (_timer > 15)
            {
                this.alpha -= 0.04f;
                if (this.alpha <= 0f)
                {
                    Level.Remove(this);
                }
            }
            base.Update();
        }
    }
}
