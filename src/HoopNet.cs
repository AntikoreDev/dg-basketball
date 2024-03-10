using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.BBMod
{
    public class NetPoint
    {
        public Vec2 pos;
        public Vec2 prevpos;
        public bool locked;
        public Vec2 velocity = Vec2.Zero;
        public Vec2 acceleration = Vec2.Zero;
        public float gravity = 1f;

        public NetPoint(Vec2 position, bool lck = false)
        {
            pos = position;
            prevpos = pos;
            locked = lck;
        }

        public void Update()
        {
            velocity = Lerp.Vec2(velocity, Vec2.Zero, 0.5f);
            foreach (PhysicsObject o in Level.CheckCircleAll<PhysicsObject>(pos, 0.1f))
            {
                if (velocity.length < 0.5f)
                ApplyForce(-(o.position - pos).normalized);
            }
        }

        public void ApplyForce(Vec2 force)
        {
            velocity += force;
        }
    }

    public class NetStick
    {
        public NetPoint A;
        public NetPoint B;
        public float length;

        public NetStick(NetPoint a, NetPoint b, float len = 4)
        {
            A = a; B = b; length = len;
        }

        public void Draw()
        {
            Graphics.DrawLine(A.pos, B.pos, Color.White, 0.66f, 2);
        }
    }

    public class HoopNet
    {
        public List<NetStick> sticks = new List<NetStick>();
        public List<NetPoint> points;
        public NetPoint leftanchor;
        public NetPoint rightanchor;

        public HoopNet(Vec2 leftanch, Vec2 rightanch, float angle, float grav)
        {
            float x = ((rightanch - leftanch) / 6).x * (grav > 0 ? 1 : -1);
            Vec2 off = Utils.Rotate(new Vec2(x, 0), Vec2.Zero, angle);
            Vec2 yoff = Utils.Rotate(Vec2.Unity * 4, Vec2.Zero, angle);
            leftanchor = new NetPoint(leftanch, true);
            rightanchor = new NetPoint(rightanch, true);
            points = new List<NetPoint>()
            {
                leftanchor,
                new NetPoint(leftanch + off),
                new NetPoint(leftanch + off * 2),
                new NetPoint(leftanch + off * 3),
                new NetPoint(leftanch + off * 4),
                rightanchor,
                new NetPoint(leftanch + yoff + off * 1.5f),
                new NetPoint(leftanch + yoff + off * 2.5f),
                new NetPoint(leftanch + yoff + off * 3.5f),
                new NetPoint(leftanch + yoff + off * 4.5f),
                new NetPoint(leftanch + yoff + off * 5.5f),
                new NetPoint(leftanch + yoff * 2 + off * 1.5f),
                new NetPoint(leftanch + yoff * 2 + off * 2.5f),
                new NetPoint(leftanch + yoff * 2 + off * 3.5f),
                new NetPoint(leftanch + yoff * 2 + off * 4.5f),
                new NetPoint(leftanch + yoff * 3 + off * 1.5f),
                new NetPoint(leftanch + yoff * 3 + off * 2.5f),
                new NetPoint(leftanch + yoff * 3 + off * 3.5f),
                new NetPoint(leftanch + yoff * 3 + off * 4.5f)
            };
            foreach (NetPoint p in points)
            {
                p.gravity = grav;
            }
            sticks = new List<NetStick>()
            {
                new NetStick(points[0], points[1]),
                new NetStick(points[1], points[2]),
                new NetStick(points[2], points[3]),
                new NetStick(points[3], points[4]),
                new NetStick(points[4], points[5]),
                new NetStick(points[0], points[6]),
                new NetStick(points[1], points[6]),
                new NetStick(points[1], points[7]),
                new NetStick(points[2], points[7]),
                new NetStick(points[2], points[8]),
                new NetStick(points[3], points[8]),
                new NetStick(points[3], points[9]),
                new NetStick(points[4], points[9]),
                new NetStick(points[4], points[10]),
                new NetStick(points[5], points[10]),
                new NetStick(points[6], points[7]),
                new NetStick(points[7], points[8]),
                new NetStick(points[8], points[9]),
                new NetStick(points[9], points[10]),
                new NetStick(points[6], points[11]),
                new NetStick(points[7], points[11]),
                new NetStick(points[7], points[12]),
                new NetStick(points[8], points[12]),
                new NetStick(points[8], points[13]),
                new NetStick(points[9], points[13]),
                new NetStick(points[9], points[14]),
                new NetStick(points[10], points[14]),
                new NetStick(points[11], points[12]),
                new NetStick(points[12], points[13]),
                new NetStick(points[13], points[14]),
                new NetStick(points[11], points[15]),
                new NetStick(points[11], points[16]),
                new NetStick(points[12], points[16]),
                new NetStick(points[13], points[16]),
                new NetStick(points[13], points[17]),
                new NetStick(points[13], points[18]),
                new NetStick(points[14], points[18]),
                new NetStick(points[15], points[16]),
                new NetStick(points[16], points[17]),
                new NetStick(points[17], points[18]),
            };
            for (int i = 0; i < 50; i++)
            {
                foreach (NetStick stick in sticks)
                {
                    Vec2 center = (stick.A.pos + stick.B.pos) / 2;
                    Vec2 direction = (stick.A.pos - stick.B.pos).normalized;
                    Vec2 st = stick.B.pos - stick.A.pos;
                    if (!stick.B.locked && st.length > stick.length)
                    {
                        stick.B.pos = center - ((direction * stick.length) / 2);
                    }
                    if (!stick.A.locked && st.length > stick.length)
                    {
                        stick.A.pos = center + ((direction * stick.length) / 2);
                    }
                }
            }
            for (int i = 1; i < 5; i++)
            {
                points[i].locked = true;
            }
        }

        public void Update()
        {
            foreach (NetPoint p in points)
            {
                if (!p.locked)
                {
                    Vec2 temp = p.pos;
                    p.pos += p.pos - p.prevpos;
                    p.pos += Vec2.Unity * p.gravity + p.velocity;
                    p.prevpos = temp;
                }
                p.Update();
            }
            for (int i = 0; i < 1; i++)
            {
                foreach (NetStick stick in sticks)
                {
                    Vec2 center = (stick.A.pos + stick.B.pos) / 2;
                    Vec2 direction = (stick.A.pos - stick.B.pos).normalized;
                    Vec2 st = stick.B.pos - stick.A.pos;
                    if (!stick.A.locked && st.length > stick.length)
                    {
                        stick.A.pos = center + ((direction * stick.length) / 2);
                    }
                    st = stick.A.pos - stick.B.pos;
                    if (!stick.B.locked && st.length > stick.length)
                    {
                        stick.B.pos = center - ((direction * stick.length) / 2);
                    }
                }
            }
        }

        public void Draw()
        {
            foreach (NetStick s in sticks)
            {
                s.Draw();
            }
        }
    }
}
