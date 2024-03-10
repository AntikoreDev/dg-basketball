using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DuckGame;

namespace DuckGame.BBMod
{
    class BasketDuckManager : Thing
    {
        public int respawnTime;
        public Duck myDuck;
        private bool myDuckFell;
        public int framesWhileAlive;
        public int _fucked;
        public BasketDuckManager(float xpos, float ypos) : base(xpos, ypos)
		{
            this.respawnTime = 0;
            this.visible = false;
        }

        public override void Update()
        {
            //If the duck this manager holds isn't null
            if (this.myDuck != null)
            {
                //If this level hasn't already put the ball into the hoop
                if (!BasketMode.hasBasket)
                {
                    this._fucked++;
                    Duck d = this.myDuck;
                    if (!d.dead)
                    {
                        this.framesWhileAlive++;
                        if (this.framesWhileAlive > 30)
                        {
                            this._fucked = 0;
                        }
                        if (this.framesWhileAlive <= 60)
                        {
                            if (d.onFire)
                            {
                                if (base.isServerForObject)
                                {
                                    for (int i = 0; i < 25; i++)
                                    {
                                        ExtinguisherSmoke extinguisherSmoke = new ExtinguisherSmoke(Rando.Float(d.left, d.right), Rando.Float(d.top, d.bottom), false);
                                        Level.Add(extinguisherSmoke);
                                    }
                                }
                            }
                        }
                    }
                    
                    this.CheckForDrop();
                    if (d != null && d.isServerForObject)
                    { 
                        if (d.y >= Level.activeLevel.lowestPoint + 90f || (d.ragdoll != null && d.ragdoll.y >= Level.activeLevel.lowestPoint + 90f))
                        {
                            if (d.holdObject is RagdollPart)
                            {   
                                d.ThrowItem();
                            }
                            if (d.ragdoll != null)
                            {
                                d.ragdoll.Unragdoll();
                            }
                            ForceResurrect();
                            this.ReturnToLand(d);
                            this.RemoveInventory(d);
                        }

                        //Respawn when killed by fire
                        /*
                        if (d.onFire && d.burnt >= 0.990f)
                        {
                            d.Ressurect();
                        }
                        */

                        //When the duck is dead
                        if (d.dead)
                        {
                            this.framesWhileAlive = 0;
                            if (this._fucked > 300)
                            {
                                this.ForceResurrect();
                                this.ReturnToLand(d);
                            }
                            this.respawnTime++;
                            if (this.respawnTime >= 90)
                            {
                                if (d._cooked != null)
                                {
                                    d.visible = true;
                                    ReturnToLand(d);
                                }
                                d.Ressurect();
                                this.respawnTime = 0;
                            }
                        }
                    }
                }
            }
        }

        public void FireRespawn(Duck d)
        {
            /*
            if (Network.isActive)
            {
                this._netDeath.Play();
                this._netPierce.Play();
                this._netIgnite.Play(1f, -0.3f + Rando.Float(0.3f));
            }
            SFX.Play("death", 1f, 0f, 0f, false);
            SFX.Play("pierce", 1f, 0f, 0f, false);
            SFX.Play("ignite", 1f, -0.3f + Rando.Float(0.3f), 0f, false);
            d.onFire = false;
            d.burnt = 0f;
            if (d.ragdoll != null)
            {
                d.ragdoll.Unragdoll();
            }
            CookedDuck cd = new CookedDuck(d.x, d.y);
            cd.hSpeed = d.hSpeed;
            cd.vSpeed = d.vSpeed;
            cd.vSpeed -= 3f;
            if (isServerForObject)
            { 
                Level.Add(cd);
            }
            this.RemoveInventory(d);
            this.ReturnToLand(d);
            */
            Profile p = d.profile;
            if (d != null)
            {
                p.duck = null;
                Level.Remove(d);
            }
            p.duck = new Duck(0, 0, p);
            d = p.duck;
            Level.Add(d);
            this.myDuck = d;
            this.ReturnToLand(d);
            d.Ressurect();
        }
        
        public void RemoveInventory(Duck d)
        {
            if (d.holdObject != null)
            {
                Holdable h = d.holdObject as Holdable;
                d.ThrowItem();
                Level.Remove(h);
            }
            this.DropEquipment(d);
        }

        public void DropEquipment(Duck d)
        {
            foreach (Equipment e in d._equipment)
            {
                if (e != null)
                {
                    e.UnEquip();
                    Level.Remove(e);
                }
            }
        }

        public void ReturnToLand(Duck d)
        {
            this.ReturnSmoke(d.x, d.y);
            List<FreeSpawn> spawns = new List<FreeSpawn>();
            foreach (Thing thing in Level.current.things[typeof(FreeSpawn)])
            {
                FreeSpawn item = thing as FreeSpawn;
                spawns.Add(item);
            }
            d.vSpeed = 0f;
            d.hSpeed = 0f;
            if (d.ragdoll != null)
            {
                d.ragdoll.Unragdoll();
            }
            if (spawns.Count > 0)
            {
                FreeSpawn spawnpoint = spawns[Rando.Int(spawns.Count - 1)];
                d.position = spawnpoint.position;
                d.y -= 8f;
            };
            this.ReturnSmoke(d.x, d.y);
        }

        public void ReturnSmoke(float xx, float yy)
        {
            Level.Add(SmallSmoke.New(xx, yy));
            Level.Add(SmallSmoke.New(xx + 4f, yy));
            Level.Add(SmallSmoke.New(xx - 4f, yy));
            Level.Add(SmallSmoke.New(xx, yy + 4f));
            Level.Add(SmallSmoke.New(xx, yy - 4f));
        }

        public void ForceResurrect()
        {
            Duck d = this.myDuck;
            d.Ressurect();
            this.respawnTime = 0;
        }

        public void CheckForDrop()
        {
            Duck d = this.myDuck;
            if (d.inputProfile.Pressed("jump", false) && d.ragdoll != null && (d.ragdoll.part1.owner != null || d.ragdoll.part2.owner != null || d.ragdoll.part3.owner != null) && d.ragdoll.owner is Duck && !d.dead)
            {
                bool flag = Rando.Int(1, 5) == 5;
                if (flag)
                {
                    (d.ragdoll.owner as Duck).ThrowItem();
                    d.ragdoll.Unragdoll();    
                }
            }
        }

        public StateBinding _sfxBinding_death = new NetSoundBinding("_netDeath");
        public StateBinding _sfxBinding_pierce = new NetSoundBinding("_netPierce");
        public StateBinding _sfxBinding_ignite = new NetSoundBinding("_netIgnite");
        public NetSoundEffect _netDeath = new NetSoundEffect(new string[]
        {
            "death"
        });
        public NetSoundEffect _netPierce = new NetSoundEffect(new string[]
        {
            "pierce"
        });
        public NetSoundEffect _netIgnite = new NetSoundEffect(new string[]
        {
            "ignite"
        });
        public StateBinding _myDuckBinding = new StateBinding("myDuck", -1, false, false);
        public StateBinding _respawnTimeBinding = new StateBinding("respawnTime", -1, false, false);
        public StateBinding _fuckedBinding = new StateBinding("_fucked",-1, false, false);
        public StateBinding _framesWhileAliveBinding = new StateBinding("framesWhileAlive", -1, false, false);
    }
}
