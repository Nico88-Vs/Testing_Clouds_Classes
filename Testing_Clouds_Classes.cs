// Copyright QUANTOWER LLC. © 2017-2022. All rights reserved.

using MultiTF3_Strategy1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace Testing_Clouds_Classes
{
    
	public class Testing_Clouds_Classes : Indicator
    {
        public Indicator ichiMt;
        public HistoricalData hd;
        private FastCloudSeries fastCloudSeries;

        
       
        public Testing_Clouds_Classes()
            : base()
        {
            Name = "Testing_Clouds_Classes";
            Description = "My indicator's annotation";

            AddLineSeries("line1", Color.CadetBlue, 1, LineStyle.Solid);
            //this.LinesSeries[0].TimeShift = 26 * 5;

            SeparateWindow = true;
        }

        protected override void OnInit()
        {
            hd = this.Symbol.GetHistory(this.HistoricalData.Period, fromTime: Core.TimeUtils.DateTimeUtcNow.AddDays(-1));
            //Indicator
            IndicatorInfo iInfo = Core.Instance.Indicators.All.First(info => info.Name == "IchiMTreTempi V.1");
            ichiMt = Core.Instance.Indicators.CreateIndicator(iInfo);

            this.ichiMt.Settings = new List<SettingItem>()
            {
               new SettingItemInteger(name: "Tenkan Sen", value: 9),
               new SettingItemInteger(name: "Kijoun Sen", value: 26),
               new SettingItemInteger(name: "SekuSpanB", value: 52),
               new SettingItemInteger(name: "Multiplaier", value: 5),
               new SettingItemInteger(name: "MultiplaierSecondo", value: 30),
            };

            this.hd.AddIndicator(ichiMt);

            //series Istance
            fastCloudSeries = new FastCloudSeries(13, 14, ichiMt, 8, 9, 3, 4);
                
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            fastCloudSeries.GenerateCloud();
            int fromIndex = hd.Count- 1;
            DrowIndicator(fromIndex);
            int value = fastCloudSeries.Clouds.Count;
            SetValue(value);
        }

        private void DrowIndicator(int fromIndex)
        {
            for (int i = fromIndex; i >= 0; i--)
            {
                double value = this.ichiMt.GetValue(lineIndex: 3, offset: i);
                if(value > 0)
                    this.SetValue(value, offset: i);
            }
        }
    }
}
