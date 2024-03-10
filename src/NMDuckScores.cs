using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.BBMod
{
    class NMDuckScores : NMDuckNetworkEvent
    {
        public byte index;
        public NMDuckScores()
        {
        }

        public NMDuckScores(byte idx)
        {
            this.index = idx;
        }

        public override void Activate()
        {
            Profile p = DuckNetwork.profiles[(int)this.index];
            BasketMode.basketDuck = p.duck;
            BasketMode.hasBasket = true;
            base.Activate();
        }
    }
}
