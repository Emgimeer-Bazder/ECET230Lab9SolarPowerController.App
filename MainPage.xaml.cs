using System.IO.Ports;

namespace ECET230Lab9SolarPowerController;

public partial class MainPage : ContentPage
{
	private bool bPortOpen = false; //used for port control toggle button
	private string newPacket = "";  //newest recievd packet
    private int chkSumError = 0;    //tally of recieved packets with different check sums

    SerialPort serialport = new SerialPort();
    public MainPage()
	{
		InitializeComponent();
        string[] ports = SerialPort.GetPortNames();     //An array of the names of all SerialPorts. This is a static method so refers to class, not instance serialPorts
        pickerAvailablePorts.ItemsSource = ports;                 //assigns maui gui picker to have contents of serial port names
        pickerAvailablePorts.SelectedIndex = ports.Length;        //Makes the default selected serial port in the picker the last serial port... I think connected?
        Loaded += MainPage_Loaded;                                 //a event handler, runs when an event occurs, that runs when page says its done loading
    }

    private void MainPage_Loaded(object sender, EventArgs e)
    {
        //only runs when maui page is fully loaded
        serialport.BaudRate = 115200;                       //sets baud rate, needs to match firmware
        serialport.ReceivedBytesThreshold = 1;              //when one byte recieved, triggers event handler related to serial port
        serialport.DataReceived += Serialport_DataReceived; //Event handler that runs when Received Bytes threshold is exceeded.
    }

    private void Serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        newPacket = serialport.ReadLine();  //I did not pay attention, but prescribed protocol has carriage return in packet, so this will read until carriage return. will consume line feed
                                            //display this packet in gui will break because all on same thread.
        MainThread.BeginInvokeOnMainThread(MyMainThreadCode);	//makes a new thread so gui data can be displayed
    }

    private void MyMainThreadCode()
    {
        labelRecievedPacket.Text = newPacket;
    }

    private void BtnOpenPort_Clicked(object sender, EventArgs e)
    {
        if (!bPortOpen)
        {
            serialport.PortName = pickerAvailablePorts.SelectedItem.ToString();   //grabs port name from picker 
            serialport.Open();                                          //opens that port name
            BtnOpenPort.Text = "Close";                                //This changes button text, so this can be a toggle button
            bPortOpen = true;                                           //store current state for purpose of toggle functionality
        }
        else
        {
            //Reflection of above toggle code but for closeing port.
            serialport.Close();
            BtnOpenPort.Text = "Open";
            bPortOpen = false;
        }
    }
}

