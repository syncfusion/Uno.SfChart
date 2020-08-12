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

            Accidents.Add(new Accidents() { Month = mth.AddMonths(6), Bus = 18, Car = 24, Truck = 33 });
            Accidents.Add(new Accidents() { Month = mth.AddMonths(7), Bus = 16, Car = 22, Truck = 30 });
            Accidents.Add(new Accidents() { Month = mth.AddMonths(8), Bus = 17, Car = 26, Truck = 34});
            Accidents.Add(new Accidents() { Month = mth.AddMonths(9), Bus = 16, Car = 22, Truck = 29 });
            Accidents.Add(new Accidents() { Month = mth.AddMonths(10), Bus = 15, Car = 25, Truck =31 });
            Accidents.Add(new Accidents() { Month = mth.AddMonths(11), Bus = 16, Car = 23, Truck = 32 });


        }

        public List<StackedModel> MedalDetails { get; set; }

        public List<Accidents> Accidents
        {
            get;
            set;
        }
    }
}
