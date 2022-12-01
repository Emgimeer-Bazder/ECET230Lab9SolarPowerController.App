using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;

namespace ECET230Lab9SolarPowerController
{
    internal class SolarClass
    {
        public Double outputVoltage { get; set; }  //voltage at A1
        public Double solarPanelVotlage { get; set; }
        public Double batteryVoltage { get; set; }
        public Double batteryCurrent { get; set; }
        public Double loadOneCurrent { get; set; }
        public Double loadTwoCurrent { get; set; }
        private Double resistorOhms { get; set; }
        private Double loadOneVoltage { get; set; }
        private Double loadTwoVoltage { get; set; }

        public string contorlPacket { get; set; }

        public SolarClass()
        {
            contorlPacket = "###0000192";
        }

        public void parsePacket(string validPacket)
        {
            //takes in a serial packet of the ECET230 serial protocol and stores in the class variables
            solarPanelVotlage = (Convert.ToDouble(validPacket.Substring(6, 4)) /1000);
            batteryVoltage = (Convert.ToDouble(validPacket.Substring(14, 4)) / 1000);
            outputVoltage = (Convert.ToDouble(validPacket.Substring(10, 4)) / 1000);
            loadOneVoltage = (Convert.ToDouble(validPacket.Substring(22, 4)) / 1000);
            loadTwoVoltage = (Convert.ToDouble(validPacket.Substring(18, 4)) / 1000);
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
            Double resistorOhms = 220.00;
            try
            {
                //battery current is derived from ohms law, (A2 - A1)/R1
                labelBatteryAmp.Text = (((batteryVoltage - outputVoltage) / resistorOhms) * -1000000).ToString("F99").TrimEnd('0').Remove(6) + " [uA]";
                //load one current is dervied from ohms law, (A1-A4)/R1
                labelLoadOneAmp.Text = (((outputVoltage - loadOneVoltage) / 220.00000) * 1000).ToString("F99").TrimEnd('0').Remove(5) + " [mA]";
                //load two current is dervied from ohms law, (A1-A4)/R1
                labelLoadTwoAmp.Text = (((outputVoltage - loadTwoVoltage) / 220.0000) * 1000).ToString("F99").TrimEnd('0').Remove(5) + " [mA]";
            }
            catch { }


        }

        public string controlPacketUpdate(string packetOverride = "", int io0 = 2, int io1 = 2, int io2 = 2, int io3 = 2)
        {
            //updates the contorl packet with any provided optional parameters
            string workingPacket = contorlPacket;
            if(packetOverride.Length > 10)
            {
                contorlPacket = packetOverride;
                return packetOverride;
            }
            if (io0 == 0 || 1 == io0)
            {
                workingPacket = $"{workingPacket.Substring(0, 3)}" + Convert.ToString(io0) + $"{workingPacket.Substring(4, 6)}";
            }
            if (io1 == 0 || 1 == io1)
            {
                workingPacket = $"{workingPacket.Substring(0, 4)}" + Convert.ToString(io1) + $"{workingPacket.Substring(5, 5)}";
            }
            if (workingPacket != contorlPacket)
            {
                //Calculates the checksum of the user entered data then replaces the old checksum.
                int calChkSum2 = 0;
                for (int i = 3; i < 7; i++)                                                             //calculate check sum by adding together all the dynamic data
                {
                    calChkSum2 += (byte)workingPacket[i];
                }
                calChkSum2 %= 1000;
                workingPacket = $"{workingPacket.Substring(0, 7),-7}" + Convert.ToString(calChkSum2);	//replace the old checksum
            }
            contorlPacket = workingPacket;
            return workingPacket;
        }
    }
}
