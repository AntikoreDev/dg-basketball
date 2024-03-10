using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.BBMod
{
    public class HoopPart : Block
    {
        public HoopPart(Hoop own) : base(own.x, own.y)
        {
            collisionOffset = new Vec2(-0.05f, -1);
            collisionSize = new Vec2(0.1f, 7);
        }

        public override void Draw()
        {
            //debug
            //Graphics.DrawRect(topLeft, bottomRight, Color.Red, 3);
            base.Draw();
        }
    }
}
