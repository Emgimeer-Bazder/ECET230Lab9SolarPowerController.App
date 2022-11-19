using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECET230Lab9SolarPowerController
{
    internal class SolarClass
    {
        public decimal solarPanelVotlage { get; set; }
        public decimal batteryVoltage { get; set; }
        public decimal batteryCurrent { get; set; }
        public decimal loadOneCurrent { get; set; }
        public decimal loadTwoCurrent { get; set; }
        private int resistorOhms { get; set; }

        public void parsePacket(string validPacket)
        {
            //takes in a serial packet of the ECET230 serial protocol and stores in the class variables
            solarPanelVotlage = (Convert.ToDecimal(validPacket.Substring(6, 4)) /1000);
        }

        public void VoltageDisplay(Label labelSolarVolt, Label batteryVolt)
        {
            //protocol ranges ADC0 value to four characters, ranged [0000, 3300] assuming that is A.BCD [V]
            //This is display a parsed solar packet at the specified gui location
            labelSolarVolt.Text = $"{Convert.ToString(solarPanelVotlage)}" + " [v]"; //use string interpolation to display the adc value
        }
    }
}
