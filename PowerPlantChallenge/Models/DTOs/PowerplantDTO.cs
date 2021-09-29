using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Models.DTOs
{
    public class PowerplantDTO : AbstractValidator<PowerplantDTO>
    {
        
        public string Name { get; set; }        
        public string Type { get; set; }
        public double Efficiency { get; set; }
        public double PMax { get; set; }
        public double PMin { get; set; }

        public PowerplantDTO()
        {
            RuleFor(pow => pow.Type).IsEnumName(typeof(PowerplantType), caseSensitive: false);
        }
        
    }
}
