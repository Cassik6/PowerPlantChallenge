using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PowerPlantChallenge.Models
{
    [DataContract]
    public class FuelPrices
    {
        
        
        public double Gas { get; set; }
        
        public double Kerosine { get; set; }
        
        public double CO2 { get; set; }
        
        public double Wind { get; set; }

    }
}