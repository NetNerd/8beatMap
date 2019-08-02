using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    // this is a huge mess to help ensure the hacky audio synchronisation stuff runs somewhat smoothly most of the time
    // I guess it works well enough

    class TickSmoothing
    {
        public Form1 form;

        private double lastTickForSmoothing = 0;
        private DateTime lastTickChange = DateTime.UtcNow;

        private double getSmoothedPlayTickTime(double rawtick)
        {
            //rawtick = getAveragedPlayTickTime(rawtick);
            Notedata.Chart chart = form.chart; // doesn't seem necessary for our use case, but if I want to eliminate warnings it should be here

            TimeSpan timeDelta = DateTime.UtcNow - lastTickChange;
            double interpTick = lastTickForSmoothing + chart.ConvertTimeToTicks(timeDelta);
            double absdiff = Math.Abs(rawtick - interpTick);

            if (!form.IsPlaying || timeDelta > TimeSpan.FromMilliseconds(1000) || absdiff > 0.75)
            {
                lastTickForSmoothing = rawtick;
                lastTickChange = DateTime.UtcNow;
                return lastTickForSmoothing;
            }


            if (absdiff > 1) absdiff = 1;
            int tenAbsdiff = (int)(absdiff * 10);

            if (absdiff > 0.2)
            {
                lastTickForSmoothing = (interpTick * (10 - tenAbsdiff) + rawtick) / (11 - tenAbsdiff);
                lastTickChange = DateTime.UtcNow;

                return lastTickForSmoothing;
            }
            else
            {
                return interpTick;
            }
        }


        private double[] prevPlayTicks = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        public double getAveragedPlayTickTime(double rawtick)
        {
            //return getSmoothedPlayTickTime(rawtick);
            rawtick = (rawtick + getSmoothedPlayTickTime(rawtick) * 2) / 3d;

            double avgTickDelta = 0;
            int numentries = 0;
            for (int i = 1; i < prevPlayTicks.Length; i++)
            {
                if (prevPlayTicks[i] >= 0)
                {
                    avgTickDelta += prevPlayTicks[i - 1] - prevPlayTicks[i];
                    numentries++;
                }
            }
            if (numentries > 0)
                avgTickDelta /= numentries;
            else
                avgTickDelta = 0;

            double averagedTick = prevPlayTicks[0] + avgTickDelta;

            if (Math.Abs(rawtick - averagedTick) > 2 | !form.IsPlaying) // averaged tick is too different to raw tick or playtimer isn't enabled -- reset all to default state
            {
                prevPlayTicks[0] = rawtick;
                for (int i = 1; i < prevPlayTicks.Length; i++)
                {
                    prevPlayTicks[i] = -1;
                }
            }
            else
            {
                for (int i = prevPlayTicks.Length - 1; i > 0; --i)
                {
                    prevPlayTicks[i] = prevPlayTicks[i - 1];
                }
                if (numentries > 0)
                    prevPlayTicks[0] = (averagedTick * 2 + rawtick) / 3; // add some portion of rawtick to help avoid drifting
                else
                    prevPlayTicks[0] = rawtick; // if there was no valid average made, it's necessary to just use the raw value provided
            }

            if (prevPlayTicks[0] < 0) prevPlayTicks[0] = 0;
            return (prevPlayTicks[0] + rawtick) / 2;
        }
    }
}
