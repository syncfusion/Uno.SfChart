using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class ViewModel
    {
        public ViewModel()
        {
            this.Data = new List<Model>();
            Data.Add(new Model() { Country = "Uruguay", Count = 2807 });
            Data.Add(new Model() { Country = "Argentina", Count = 2577 });
            Data.Add(new Model() { Country = "USA", Count = 960 });
            Data.Add(new Model() { Country = "Germany", Count = 2120 });

            this.DoughnutData = new List<Model>();
            DoughnutData.Add(new Model() { Country = "USA", Count = 2473 });
            DoughnutData.Add(new Model() { Country = "Germany", Count = 2120 });
            DoughnutData.Add(new Model() { Country = "Malta", Count = 960 });
            DoughnutData.Add(new Model() { Country = "Maldives", Count = 941 });

            this.SemiPieData = new List<Model>();
            SemiPieData.Add(new Model() { Country = "USA", Count = 473 });
            SemiPieData.Add(new Model() { Country = "Germany", Count = 1120 });
            SemiPieData.Add(new Model() { Country = "Malta", Count = 960 });
            SemiPieData.Add(new Model() { Country = "Monaco", Count = 908 });

            this.Collection = new List<Model>();
            Collection.Add(new Model() { Name = "Week1", Count = 45 });
            Collection.Add(new Model() { Name = "Week2", Count = 60 });
            Collection.Add(new Model() { Name = "Week3", Count = 70 });
            Collection.Add(new Model() { Name = "Week4", Count = 85 });

            this.Population = new List<Populations>();
            Population.Add(new Populations() { Continent = "Asia", Countries = "China", States = "Taiwan", PopulationinContinents = 50.02, PopulationinCountries = 26.02, PopulationinStates = 18.02 });
            Population.Add(new Populations() { Continent = "Africa", Countries = "India", States = "Shandong", PopulationinContinents = 20.81, PopulationinCountries = 24, PopulationinStates = 8 });
            Population.Add(new Populations() { Continent = "Europe", Countries = "Nigeria", States = "UP", PopulationinContinents = 15.37, PopulationinCountries = 12.81, PopulationinStates = 14.5 });
            Population.Add(new Populations() { Countries = "Ethiopia", States = "Maharashtra", PopulationinCountries = 8, PopulationinStates = 9.5 });
            Population.Add(new Populations() { Countries = "Germany", States = "Kano", PopulationinCountries = 8.37, PopulationinStates = 7.81 });
            Population.Add(new Populations() { Countries = "Turkey", States = "Lagos", PopulationinCountries = 7, PopulationinStates = 5 });
            Population.Add(new Populations() { States = "Oromia", PopulationinStates = 5 });
            Population.Add(new Populations() { States = "Amhara", PopulationinStates = 3 });
            Population.Add(new Populations() { States = "Hessen", PopulationinStates = 5.37 });
            Population.Add(new Populations() { States = "Bayern", PopulationinStates = 3 });
            Population.Add(new Populations() { States = "Istanbul", PopulationinStates = 4.5 });
            Population.Add(new Populations() { States = "Ankara", PopulationinStates = 2.5 });
        }

        public IList<Model> Data
        {
            get;
            set;
        }

        public IList<Model> DoughnutData
        {
            get;
            set;
        }

        public IList<Model> SemiPieData
        {
            get;
            set;
        }

        public IList<Model> Collection
        {
            get;
            set;
        }

        public IList<Populations> Population
        {
            get;
            set;
        }
    }
}
