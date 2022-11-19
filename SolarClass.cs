using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECET230Lab9SolarPowerController
{
    internal class SolarClass
    {
        private int solarPanelVotlage { get; set; }

        public void VoltageDisplay(string parsedPacketVoltageValue, Label guiLocation)
        {
            //protocol ranges ADC0 value to four characters, ranged [0000, 3300] assuming that is A.BCD [V]
            //This is display a parsed solar packet at the specified gui location
            guiLocation.Text = $"{parsedPacketVoltageValue.Substring(0, 1)}" + "." + $"{parsedPacketVoltageValue.Substring(1, 3)}" + " [v]"; //use string interpolation to display the adc value
        }
    }
}
