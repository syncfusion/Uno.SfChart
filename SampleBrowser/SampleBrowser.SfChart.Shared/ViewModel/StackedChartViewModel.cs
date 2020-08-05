using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class StackingChartViewModel
    {
        public StackingChartViewModel()
        {
            this.MedalDetails = new List<StackedModel>();

            MedalDetails.Add(new StackedModel() { CountryName = "USA", GoldMedals = 39, SilverMedals = 31, BronzeMedals = 29 });
            MedalDetails.Add(new StackedModel() { CountryName = "Germany", GoldMedals = 24, SilverMedals = 28, BronzeMedals = 32 });
            MedalDetails.Add(new StackedModel() { CountryName = "Britain", GoldMedals = 20, SilverMedals = 25, BronzeMedals = 25 });
            MedalDetails.Add(new StackedModel() { CountryName = "France", GoldMedals = 19, SilverMedals = 21, BronzeMedals = 23 });
            MedalDetails.Add(new StackedModel() { CountryName = "Italy", GoldMedals = 19, SilverMedals = 15, BronzeMedals = 17 });

            this.Accidents = new List<Accidents>();
            DateTime mth = new DateTime(2011, 1, 1);

            Accidents.Add(new Accidents() { Month = mth.AddMonths(6), Bus = 3, Car = 4, Truck = 5 });
            Accidents.Add(new Accidents() { Month = mth.AddMonths(7), Bus = 4, Car = 5, Truck = 5 });
            Accidents.Add(new Accidents() { Month = mth.AddMonths(8), Bus = 3, Car = 4, Truck = 5 });
            Accidents.Add(new Accidents() { Month = mth.AddMonths(9), Bus = 4, Car = 5, Truck = 5 });
        }

        public List<StackedModel> MedalDetails { get; set; }

        public List<Accidents> Accidents
        {
            get;
            set;
        }
    }
}
