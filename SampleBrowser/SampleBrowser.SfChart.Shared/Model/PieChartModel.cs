using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class Populations
    {
        public string Continent { get; set; }

        public string Countries { get; set; }

        public string States { get; set; }

        public double PopulationinStates { get; set; }

        public double PopulationinCountries { get; set; }

        public double PopulationinContinents { get; set; }
    }
    public class Model
    {
        public string Country { get; set; }

        public string Name { get; set; }
        public double Count { get; set; }

        public string Year { get; set; }
    }
}
