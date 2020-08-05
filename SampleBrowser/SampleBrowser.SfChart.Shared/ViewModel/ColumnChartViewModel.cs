using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class ColumnChartViewModel
    {
        public ColumnChartViewModel()
        {
            this.Data = new List<Model>();
            Data.Add(new Model() { Country = "Uruguay", Count = 72 });
            Data.Add(new Model() { Country = "Argentina", Count = 58 });
            Data.Add(new Model() { Country = "USA", Count = 80 });
            Data.Add(new Model() { Country = "Germany", Count = 56 });
            Data.Add(new Model() { Country = "Netherlands", Count = 34 });

            this.PercentageData = new List<Model>();
            PercentageData.Add(new Model() { Year = "2005", Count = 100 });
            PercentageData.Add(new Model() { Year = "2006", Count = 100 });
            PercentageData.Add(new Model() { Year = "2007", Count = 100 });
            PercentageData.Add(new Model() { Year = "2008", Count = 100 });
            PercentageData.Add(new Model() { Year = "2009", Count = 100 });

            this.PriceData = new List<Model>();
            PriceData.Add(new Model() { Year = "2005", Count = 77 });
            PriceData.Add(new Model() { Year = "2006", Count = 69 });
            PriceData.Add(new Model() { Year = "2007", Count = 65 });
            PriceData.Add(new Model() { Year = "2008", Count = 56 });
            PriceData.Add(new Model() { Year = "2009", Count = 44 });
        }

        public IList<Model> Data
        {
            get;
            set;
        }

        public IList<Model> PercentageData
        {
            get;
            set;
        }

        public IList<Model> PriceData
        {
            get;
            set;
        }
    }
}
