using PowerPlantChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PowerPlantChallenge.Tests.Models
{
    public class PowerplantShould
    {
        [Theory]
        public void CreateEnitity()
        {
            var powerplant = Powerplant.Create()
        }
    }
}
