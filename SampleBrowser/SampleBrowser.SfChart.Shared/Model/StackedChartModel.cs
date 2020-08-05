using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class StackedModel
    {
        public string CountryName { get; set; }

        public double GoldMedals { get; set; }

        public double SilverMedals { get; set; }

        public double BronzeMedals { get; set; }

    }

    public class Accidents
    {
        public DateTime Month { get; set; }
        public double Bus { get; set; }
        public double Car { get; set; }
        public double Truck { get; set; }
    }
}
