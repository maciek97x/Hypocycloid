using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypocycloid
{
    class Point
    {
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double x;
        public double y;
    }

    enum State
    {
        STOP,
        RUNNING,
        PAUSE
    }

    class HypocycloidCurve
    {
        public double R { get; set; }
        public double r { get; set; }

        public double t { get; private set; }
        public Point alpha { get; private set; }

        public List<Point> trail { get; private set; }

        private double t_0;
        private State state;

        public HypocycloidCurve()
        {
            trail = new List<Point>();
            t = 0.0;
            state = State.STOP;
        }

        public void Reset()
        {
            trail = new List<Point>();
            t = 0.0;
            state = State.STOP;
        }

        public void Start()
        {
            if (state == State.STOP)
            {
                t_0 = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            }
            state = State.RUNNING;
        }

        public void Stop()
        {
            state = State.PAUSE;
        }

        public void CalculateStep()
        {
            alpha = new Point(
                (R - r) * Math.Cos(t) + r * Math.Cos(t * (R - r) / r),
                (R - r) * Math.Sin(t) - r * Math.Sin(t * (R - r) / r));
            trail.Add(alpha);
        }
        
    }
}
