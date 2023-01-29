using System;
using System.Collections.Generic;
using TradingPlatform.BusinessLayer;

namespace MultiTF3_Strategy1
{
    internal class Cloud : IEquatable<Cloud>
    {
        public enum TimeFrame
        {
            Fast,
            Mid,
            slow
        }
        public struct Bases
        {
            public int Lenght;
            public int Id { get; set; }
            public double Value { get; set; }
            public int LineSeries { get; set; }

            public Bases(int id, int lineseries, double value)
            {
                this.LineSeries = lineseries;
                this.Id = id;
                this.Value = value;
            }
        }
        public enum CloudShape
        {
            Top,
            Bottom,
            Range,
            Unknown
        }

        public enum CloudColor
        {
            green = 1,
            red = -1,
            unknown = -3
        }

        public TimeFrame TF { get; set; }
        public int Id { get; set; }
        public CloudColor Color { get; set; }
        public Indicator Indicator { get; set; }
        public int FastSeries { get; set; }
        public int SlowSeries { get; set; }
        public int Length { get; set; }  //== > da ichimokumt
        public double Thickness { get; set; } //== > da ichimokumt
        public double AverageThickness { get; set; } // ==> da ichimokumt
        public CloudShape Shape { get; set; } //== > viene assegnata qui?
        public List<Bases> RoofList { get; set; } //== > creo una nuova classe
        public List<Bases> BasesList { get; set; } //== > da interprete? 
        public List<double> Maxima { get; set; }
        public List<double> Minima { get; set; }
        public double Momentum { get; set; }
        public double OriginPrice { get; set; }
        public double EndPrice { get; set; }
        public bool IsOpen { get; set; }
        // Gestione Cross
        public event EventHandler<CrossEventArgs> cross;

        public Cloud(int id, int fastseries, int slowseries, Indicator indi, CloudColor clr, double OriginPrice, TimeFrame tF)
        {
            this.Id = id;
            this.FastSeries = fastseries;
            this.SlowSeries = slowseries;
            this.Indicator = indi;
            this.Color = clr;
            this.OriginPrice = OriginPrice;
            this.IsOpen = true;

            RoofList = new List<Bases>();
            BasesList = new List<Bases>();
            Minima = new List<double>();
            Maxima = new List<double>();
            this.TF = tF;
        }

        public void UpdateCloud()
        {
            double fastValue = this.Indicator.GetValue(lineIndex: FastSeries);
            double slowvalue = this.Indicator.GetValue(lineIndex: SlowSeries);
            double fastValueShift = this.Indicator.GetValue(lineIndex: FastSeries, offset: 1);
            double slowvalueShift = this.Indicator.GetValue(lineIndex: SlowSeries, offset: 1);

            bool goldenCross = (slowvalueShift >= fastValueShift) && (fastValue > slowvalue) ? true : false;
            bool deadCross = (slowvalueShift <= fastValueShift) && (fastValue < slowvalue) ? true : false;

            if (goldenCross || deadCross)
            {
                CrossEventArgs.EventCrosArg x = CalculateArg(goldenCross);
                OnCross(x);
            }

            if (IsOpen)
            {
                this.Length++;
                this.Thickness = Math.Abs(slowvalue - fastValue);
                this.AverageThickness = GetAvarage();
                this.Momentum = CalculateMomentum(fastValue, slowvalue);
                UpdateMinMax(this.FastSeries);
                UpdateMinMax(this.SlowSeries);
                UpdateBases(this.SlowSeries, BasesList);
                UpdateBases(this.FastSeries, RoofList);
            }
            else return;
        }

        private void UpdateBases(int lineseries, List<Bases> roof_basesList)
        {
            if (IsOpen)
            {
                //bases
                if (roof_basesList.Count == 0)
                {
                    Bases prima = new Bases(roof_basesList.Count, lineseries, this.OriginPrice);
                    prima.Lenght = 1;
                    roof_basesList.Add(prima);
                }
                else
                {
                    Bases variabile = roof_basesList[roof_basesList.Count - 1];
                    double ind = this.Indicator.GetValue(lineIndex: lineseries);
                    double indshifted = this.Indicator.GetValue(lineIndex: lineseries, offset: 1);

                    if (ind == variabile.Value)
                    {
                        variabile.Lenght++;
                        roof_basesList[roof_basesList.Count - 1] = variabile;
                    }
                    else if (ind != variabile.Value && ind == indshifted)
                    {
                        Bases secondo = new Bases(roof_basesList.Count, lineseries, ind);
                        roof_basesList.Add(secondo);
                    }
                    else return;
                }
            }
        }


        private void UpdateMinMax(int lineseries)
        {
            if (IsOpen)
            {
                // minimi
                if (Minima.Count == 0)
                {
                    Minima.Add(this.OriginPrice);
                }
                else
                {
                    if (this.Indicator.GetValue(lineIndex: lineseries) < Minima[Minima.Count - 1])
                    {
                        Minima.Add(this.Indicator.GetValue(lineIndex: lineseries));
                    }
                }

                //massimi
                if (Maxima.Count == 0)
                {
                    Maxima.Add(this.OriginPrice);
                }
                else
                {
                    if (this.Indicator.GetValue(lineIndex: lineseries) < Maxima[Maxima.Count - 1])
                    {
                        Maxima.Add(this.Indicator.GetValue(lineIndex: lineseries));
                    }
                }
            }
        }

        private double GetAvarage()
        {
            double somma = 0;
            if (this.Length < 2) { return 0; }
            else if (this.Length >= 2)
            {
                for (int i = 0; i < this.Length; i++)
                {
                    double valore = this.Indicator.GetValue(i, this.FastSeries) - this.Indicator.GetValue(i, this.SlowSeries);
                    somma += Math.Abs(valore);
                }
            }
            return somma / this.Length;
        }

        public void CloudIsClosed(double ClosePrice)
        {
            this.EndPrice = ClosePrice;
            this.IsOpen = false;
        }

        private double CalculateMomentum(double fast, double slow)
        {
            double value;

            if (OriginPrice > 0)
            {
                value = Math.Abs(OriginPrice + (fast - slow)) / this.Length;
            }
            else value = 0;
            return value;
        }

        protected void OnCross(CrossEventArgs.EventCrosArg newValue)
        {
            CloudIsClosed(this.Indicator.GetValue(lineIndex: this.SlowSeries));
            cross?.Invoke(this, new CrossEventArgs(newValue));
        }

        private CrossEventArgs.EventCrosArg CalculateArg(bool isGolden)
        {
            CrossEventArgs.EventCrosArg x = CrossEventArgs.EventCrosArg.Unkown;

            switch (this.TF)
            {
                case TimeFrame.Fast:
                    x = isGolden == true ? CrossEventArgs.EventCrosArg.Gold_fast : CrossEventArgs.EventCrosArg.Dead_fast;
                    break;

                case TimeFrame.Mid:
                    x = isGolden == true ? CrossEventArgs.EventCrosArg.Gold_midt : CrossEventArgs.EventCrosArg.Dead_mid;
                    break;

                case TimeFrame.slow:
                    x = isGolden == true ? CrossEventArgs.EventCrosArg.Gold_slow : CrossEventArgs.EventCrosArg.Dead_slow;
                    break;
            }
            return x;
        }

        public bool Equals(Cloud other)
        {
            if (other == null) return false;
            return (this.Id == other.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
