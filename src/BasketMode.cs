using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DuckGame;

namespace DuckGame.BBMod
{
    [EditorGroup("Basketball Mod")]
    public class BasketMode : Thing
    {
        public static bool ballsAvailable;
        public static Duck basketDuck;
        public static bool hasBasket;
        public static bool someWins;
        public static bool levelStarted;
        public static int matchesAtStart;
        public static bool isBasket;
        public bool imMode;
        public BasketMode(float xpos, float ypos) : base(0f, 0f, null)
        {
            this.graphic = new Sprite(GetPath("basketballMode"), 0f, 0f);
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            base.depth = 0.9f;
            base.layer = Layer.Foreground;
            this._editorName = "Basket Level";
            this._canFlip = false;
            this._canHaveChance = false;
        }

        public override void Initialize()
        {
            BasketMode.hasBasket = false;
            BasketMode.basketDuck = null;
            BasketMode.someWins = false;
            BasketMode.levelStarted = false;
            BasketMode.isBasket = true;
            BasketMode.matchesAtStart = GameMode.numMatchesPlayed;
            base.Initialize();
        }

        //Controls the start of the level. Creates the managers
        public void StartLevel()
        {
            BasketMode.levelStarted = true;
            foreach (Profile profile in Profiles.all)
            {
                if (profile != null && profile.team != null && profile.duck != null)
                {
                    Duck d = profile.duck;
                    BasketDuckManager bdm = new BasketDuckManager(0, 0);
                    bdm.myDuck = d;
                    Level.Add(bdm);
                }
            }
        }

        //Controls all the level related things. Mainly detects when balls aren't available
        public void LevelController()
        {
            int ballc = 0;
            foreach (BasketballEx b in Level.current.things[typeof(BasketballEx)])
            {
                ballc++;
            }
            BasketMode.ballsAvailable = (ballc > 0);
            foreach (Basketball b in Level.current.things[typeof(Basketball)])
            {
                BasketballEx be = new BasketballEx(0, 0);
                be.x = b.x;
                be.y = b.y;
                Level.Add(be);
                Level.Remove(b);
            }
            this.UpdateGamemode();
        }

        public override void Update()
        {
            base.Update();
            if (!Network.isServer)
            {
                return;
            }
            //If the basket mode hasn't been started at all
            if (!BasketMode.levelStarted)
            {
                this.StartLevel();
            }
            //When no one scored yet.
            if (!BasketMode.hasBasket)
            {
                this.LevelController();   
            }
            //When somebody scored but only first frame
            else if (!BasketMode.someWins)
            {
                if (Network.isActive)
                {
                    Send.Message(new NMDuckScores(BasketMode.basketDuck.profile.networkIndex));
                }
                this.PlayerScoreMassacre();
            }
            this.WeaponBanning();  
        }

        public override void Draw()
        {
            if (!(Level.current is Editor))
            {
                return;
            }
            base.Draw();
        }

        public void UpdateGamemode()
        {
            if (Level.current != null && Level.current is GameLevel)
            {
                GameMode gameMode = typeof(GameLevel).GetField("_mode", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue((Level.current as GameLevel)) as GameMode;
                if (gameMode != null)
                {
                    typeof(GameMode).GetField("_matchOver", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(gameMode, false);
                    typeof(GameMode).GetField("_roundEndWait", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(gameMode, 1f);
                    GameMode.numMatchesPlayed = BasketMode.matchesAtStart;
                }
            }
        }

        public void WeaponBanning()
        {
            /*
            if ((Network.isActive && Network.isServer) || !Network.isActive)
            { 
                
                foreach (QuadLaserBullet qlb in Level.current.things[typeof(QuadLaserBullet)])
                {
                    QuadLaserBulletEx qlbe = new QuadLaserBulletEx(qlb.x, qlb.y, qlb.travel);
                    Level.Remove(qlb);
                    Level.Add(qlbe);
                }
                foreach (QuadLaser ql in Level.current.things[typeof(QuadLaser)])
                {
                    QuadLaserEx qle = new QuadLaserEx(ql.x, ql.y);
                    qle.vSpeed = ql.vSpeed;
                    qle.hSpeed = ql.hSpeed;
                    Level.Remove(ql);
                    Level.Add(qle);
                }
                foreach (HugeLaser hl in Level.current.things[typeof(HugeLaser)])
                {
                    Sharpshot ss = new Sharpshot(hl.x, hl.y);
                    ss.vSpeed = hl.vSpeed;
                    ss.hSpeed = hl.hSpeed;
                    Level.Add(ss);
                    Level.Remove(hl);
                }
                
        }
        */
        }

        public void PlayerScoreMassacre()
        {
            foreach (BasketDuckManager bdm in Level.current.things[typeof(BasketDuckManager)])
            {
                if (bdm.myDuck != BasketMode.basketDuck && bdm.myDuck.dead)
                {
                    bdm.ForceResurrect();
                }
            }
            foreach (Profile profile in Profiles.all)
            {
                if (profile != null && profile.team != null && profile.duck != null)
                {
                    Duck d = profile.duck;
                    if (d.ragdoll != null)
                    {
                        Ragdoll r = d.ragdoll;
                        r.Unragdoll();
                    }
                    if (!d.dead && d != BasketMode.basketDuck)
                    {
                        d.Kill(new DTCrush(d));
                    }
                }
            }
            BasketMode.someWins = true;
        }
    }
}
