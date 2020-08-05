using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class PolarChartViewModel
    {
        public PolarChartViewModel()
        {
            this.PlantDetails = new List<PlantData>();
            this.PlantDetails.Add(new PlantData() { Direction = "North", Weed = 63, Flower = 42, Tree = 80 });
            this.PlantDetails.Add(new PlantData() { Direction = "NorthEast", Weed = 70, Flower = 40, Tree = 85 });
            this.PlantDetails.Add(new PlantData() { Direction = "East", Weed = 45, Flower = 25, Tree = 78 });
            this.PlantDetails.Add(new PlantData() { Direction = "SouthEast", Weed = 70, Flower = 40, Tree = 90 });
            this.PlantDetails.Add(new PlantData() { Direction = "South", Weed = 47, Flower = 20, Tree = 78 });
            this.PlantDetails.Add(new PlantData() { Direction = "SouthWest", Weed = 65, Flower = 45, Tree = 83 });
            this.PlantDetails.Add(new PlantData() { Direction = "West", Weed = 58, Flower = 40, Tree = 79 });
            this.PlantDetails.Add(new PlantData() { Direction = "NorthWest", Weed = 73, Flower = 28, Tree = 88 });
        }

        public List<PlantData> PlantDetails { get; set; }
    }
}
