using System.Collections.Generic;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace MultiTF3_Strategy1
{
    internal class FastCloudSeries : CloudSeries
    {
        public int CloudId { get; set; }
        //public Dictionary<Cloud,List<Cloud>> MidCloudDictionary { get; set; }
        //public Dictionary<Cloud, List<Cloud>> SlowCloudDictionary { get; set; }
        private int midId;
        //private int slowId;


        public FastCloudSeries(int fastseries, int sloseries, Indicator indi, int slowslow, int fastslow, int fastmid, int slowmid)
            : base(fastseries, sloseries, indi, slowslow, fastslow, fastmid, slowmid)
        {
            this.Clouds = new List<Cloud>();
            //this.MidCloudDictionary= new Dictionary<Cloud, List<Cloud>>();
            //this.SlowCloudDictionary = new Dictionary<Cloud, List<Cloud>>();
            this.CloudId = 0;
            this.midId = 0;
            //this.slowId= 0;
        }

        public override void GenerateCloud()
        {
            //mid
            //if (SlowS_Mid > 0 && FastS_Mid > 0)
            //{
            //    Cloud.CloudColor c_mid = GetColor(FastS_Mid, SlowS_Mid);
            //    double start_mid = (this.Indi.GetValue(lineIndex: FastS_Mid) + this.Indi.GetValue(lineIndex: SlowS_Mid)) / 2;
            //    Cloud midCloud = new Cloud(midId, FastS_Mid, SlowS_Mid, this.Indi, c_mid, start_mid, Cloud.TimeFrame.Mid);

            //    if (!MidCloudDictionary.ContainsKey(midCloud))
            //    {
            //        MidCloudDictionary.Add(midCloud, new List<Cloud>());
            //    }
            //    else if (MidCloudDictionary.ContainsKey(midCloud))
            //    {
            //        var lastKey = MidCloudDictionary.Keys.Last();
            //        lastKey.cross += this.MidKey_cross;
            //        lastKey.UpdateCloud();
            //    }

            //}

            //fast
            if (this.SlowS_Fast > 0 && this.FastS_Fast > 0)
            {
                if (Clouds.Count == 0)
                {
                    double start = (this.Indi.GetValue(lineIndex: FastS_Fast) + this.Indi.GetValue(lineIndex: SlowS_Fast)) / 2;
                    Cloud.CloudColor c = GetColor(this.FastS_Fast, this.SlowS_Fast);
                    Cloud cloud = new Cloud(this.CloudId, this.FastS_Fast, this.SlowS_Fast, this.Indi, c, start, Cloud.TimeFrame.Fast);
                    this.Clouds.Add(cloud);
                }
                else if (Clouds.Count > 0)
                {
                    this.Clouds.Last().cross += this.FastCloudSeries_cross;
                    this.Clouds.Last().UpdateCloud();
                }
            }
        }

        private void MidKey_cross(object sender, CrossEventArgs e)
        {
            this.midId++;
            Cloud c = (Cloud)sender;
            double startPrice = c.EndPrice;
            Cloud.CloudColor clr = GetColor(FastS_Mid, FastS_Mid);
            Cloud cUno = new Cloud(midId, FastS_Mid, SlowS_Mid, this.Indi, clr, startPrice, Cloud.TimeFrame.Mid);
            //MidCloudDictionary.Add(cUno, new List<Cloud>());
        }

        private void FastCloudSeries_cross(object sender, CrossEventArgs e)
        {
            this.CloudId++;
            double startPrice = 0;
            if (sender.GetType() == this.Clouds.Last().GetType())
            {
                Cloud c = (Cloud)sender;
                startPrice = c.EndPrice;
            }

            Cloud.CloudColor cl = GetColor(this.FastS_Fast, this.SlowS_Fast);
            this.CurrentCloud = new Cloud(CloudId, this.FastS_Fast, SlowS_Fast, this.Indi, cl, startPrice, Cloud.TimeFrame.Fast);
            this.Clouds.Add(this.CurrentCloud);
            //if (MidCloudDictionary.Any())
            //{
            //    Cloud id = MidCloudDictionary.Keys.Last();
            //    MidCloudDictionary[id].Add(this.CurrentCloud);
            //}
        }

        private Cloud.CloudColor GetColor(int fastseries, int slowseries)
        {
            double color = this.Indi.GetValue(lineIndex: fastseries) - this.Indi.GetValue(lineIndex: slowseries);
            Cloud.CloudColor x = Cloud.CloudColor.unknown;

            if (color > 0)
                x = Cloud.CloudColor.green;
            if (color < 0)
                x = Cloud.CloudColor.red;

            return x;
        }
    }
}
