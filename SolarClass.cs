using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECET230Lab9SolarPowerController
{
    internal class SolarClass
    {
        public decimal outputVoltage { get; set; }  //voltage at A1
        public decimal solarPanelVotlage { get; set; }
        public decimal batteryVoltage { get; set; }
        public decimal batteryCurrent { get; set; }
        public decimal loadOneCurrent { get; set; }
        public decimal loadTwoCurrent { get; set; }
        private int resistorOhms { get; set; }
        private int loadOneVoltage { get; set; }
        private int loadTwoVoltage { get; set; }

        public void parsePacket(string validPacket)
        {
            //takes in a serial packet of the ECET230 serial protocol and stores in the class variables
            solarPanelVotlage = (Convert.ToDecimal(validPacket.Substring(6, 4)) /1000);
            batteryVoltage = (Convert.ToDecimal(validPacket.Substring(14, 4)) / 1000);
            outputVoltage = (Convert.ToDecimal(validPacket.Substring(10, 4)) / 1000);

            loadOneVoltage = (Convert.ToInt32(validPacket.Substring(22, 4)) / 1000);
            loadTwoVoltage = (Convert.ToInt32(validPacket.Substring(18, 4)) / 1000);
        }

        public void VoltageDisplay(Label labelSolarVolt, Label batteryVolt)
        {
            //protocol ranges ADC0 value to four characters, ranged [0000, 3300] assuming that is A.BCD [V]
            //This is display a parsed solar packet at the specified gui location
            labelSolarVolt.Text = $"{Convert.ToString(solarPanelVotlage)}" + " [v]"; //use string interpolation to display the voltage value to the provided label
            batteryVolt.Text = $"{Convert.ToString(batteryVoltage)}" + " [v]";
        }
        public void CurrentDisplay(Label labelLoadOneAmp, Label labelLoadTwoAmp, Label labelBatteryAmp)
        {
            resistorOhms = 330;
            //battery current is derived from ohms law, (A2 - A1)/R1
            labelBatteryAmp.Text = Convert.ToString(((outputVoltage - batteryVoltage) / resistorOhms) * 1000).Remove(6) + " [mA]";
            //load one current is dervied from ohms law, (A1-A4)/R1
            labelLoadOneAmp.Text = Convert.ToString((outputVoltage - loadOneVoltage) / resistorOhms).Remove(5) + " [A]";
            //load two current is dervied from ohms law, (A1-A4)/R1
            labelLoadTwoAmp.Text = Convert.ToString((outputVoltage - loadTwoVoltage) / resistorOhms).Remove(5) + " [A]";

        }
    }
}
