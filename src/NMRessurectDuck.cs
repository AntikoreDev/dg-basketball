using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DuckGame;

namespace DuckGame.BBMod
{
    class NMRessurectDuck : NMDuckNetworkEvent
    {
        public byte index;
        public NMRessurectDuck()
        {
        }

        public NMRessurectDuck(byte idx)
        {
            this.index = idx;
        }

        public override void Activate()
        {
            Profile p = DuckNetwork.profiles[(int)this.index];
            Duck d = p.duck as Duck;
            if (d != null)
            {
                d.killedByProfile = null;
                d.framesSinceKilled = 0;
                d.dead = false;
                d.unfocus = 1f;
                d.Regenerate();
                d.immobilized = false;
                d.isGhost = false;
                typeof(Duck).GetField("_killed", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(d, false);
                typeof(Duck).GetField("forceDead", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(d, false);
                d.crouch = false;
                d.sliding = false;
                d.active = true;
                if (Level.current.camera is FollowCam)
                {
                    (Level.current.camera as FollowCam).Add(d);
                }
            }
            base.Activate();
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            base.OnDeserialize(msg);
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
        }
    }
}
