using System.Collections.Generic;
using TradingPlatform.BusinessLayer;

namespace MultiTF3_Strategy1
{
    abstract class CloudSeries
    {
        public int FastS_Fast { get; set; }
        public Indicator Indi { get; set; }
        public int SlowS_Fast { get; set; }
        public List<Cloud> Clouds { get; set; }
        public CloudTrend Trend { get; set; }
        public int FastS_Mid { get; set; }
        public int SlowS_Mid { get; set; }
        public int FastS_Slow { get; set; }
        public int SlowS_Slow { get; set; }
        public Cloud CurrentCloud { get; set; }


        public CloudSeries(int fastseries, int sloseries, Indicator indi, int slowS_Slow, int fastS_Slow, int fastS_Mid, int slowS_Mid)
        {
            this.FastS_Fast = fastseries;
            this.SlowS_Fast = sloseries;
            this.Indi = indi;
            this.SlowS_Slow = slowS_Slow;
            this.FastS_Slow = fastS_Slow;
            this.SlowS_Mid = slowS_Mid;
            this.FastS_Mid = fastS_Mid;

        }

        public abstract void GenerateCloud();
    }
}
