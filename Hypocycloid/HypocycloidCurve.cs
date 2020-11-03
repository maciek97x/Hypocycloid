using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Hypocycloid
{
    class Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Point Copy()
        {
            return new Point(x, y);
        }
    }

    enum State
    {
        STOP,
        RUNNING,
        PAUSE,
        END
    }

    class HypocycloidCurve
    {
        public double R { get; set; }
        public Point O { get; private set; }
        public double r { get; set; }
        public Point o { get; private set; }

        public double t { get; private set; }
        public Point alpha { get; private set; }

        public List<Point> trail { get; private set; }

        public double speed { get; set; }
        
        private State state;
        private double deltaTime;
        private double time;

        public HypocycloidCurve()
        {
            trail = new List<Point>();
            alpha = new Point(0, 0);
            speed = 1.0;
            t = 0.0;
            O = new Point(0, 0);
            o = new Point(0, 0);
            time = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            state = State.STOP;
        }

        public void Reset()
        {
            trail = new List<Point>();
            t = 0.0;
            time = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            state = State.STOP;
        }

        public void Start()
        {
            double now = ((double)DateTime.Now.Ticks) / ((double)TimeSpan.TicksPerSecond);
            deltaTime = now - time;
            time = now;
            state = State.RUNNING;
        }

        public void Stop()
        {
            state = State.PAUSE;
        }

        public void Compute()
        {
            double now = ((double)DateTime.Now.Ticks) / ((double)TimeSpan.TicksPerSecond);
            deltaTime = now - time;
            time = now;
            if (state == State.RUNNING)
            {
                o.x = (R - r) * Math.Cos(t);
                o.y = (R - r) * Math.Sin(t);

                alpha.x = o.x + r * Math.Cos(t * (R - r) / r);
                alpha.y = o.y - r * Math.Sin(t * (R - r) / r);

                trail.Add(alpha.Copy());
                t += deltaTime * speed;
            }
        }
    }
}
