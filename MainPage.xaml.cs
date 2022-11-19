using System.IO.Ports;

namespace ECET230Lab9SolarPowerController;

public partial class MainPage : ContentPage
{
	private bool bPortOpen = false; //used for port control toggle button
	private string newPacket = "";  //newest recievd packet
    private int chkSumError = 0;                    //tally of recieved packets with different check sums

    SerialPort serialport = new SerialPort();
    public MainPage()
	{
		InitializeComponent();
	}

}

