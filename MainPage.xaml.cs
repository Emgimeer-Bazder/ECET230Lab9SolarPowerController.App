using System.IO.Ports;
using System.Text;

namespace ECET230Lab9SolarPowerController;

public partial class MainPage : ContentPage
{
	private bool bPortOpen = false; //used for port control toggle button
	private string newPacket = "";  //newest recievd packet
    private int chkSumError = 0;    //tally of recieved packets with different check sums

    SerialPort serialport = new SerialPort();
    SolarClass solarclass = new SolarClass();

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
        labelRecievedPacket.Text = newPacket;   //display recieved packet in debug menu
        //The following are a series of checks to validate packet data has not been corrupted
        if(newPacket.Length > 37)   //is it the right quantity of things?
        {
            if(newPacket.Substring(0, 3) == "###") //Are the first three things the header?
            {
                //calcuate a local copy of the check sum for comparison
                int calChkSum = 0;
                for (int i = 3; i < 34; i++)                                     //packet sum but only consider items after the header, ie dynamic data
                {
                    calChkSum += (byte)newPacket[i];
                }
                calChkSum %= 1000;												//mod 1000 the packet to remove the reduent 1 prefix ie truncation
                int recChkSum = Convert.ToInt32(newPacket.Substring(34, 3));    //extract recieved packet and convert to an integer for comparison
                //complete final packet validation test
                if (recChkSum == calChkSum)
                {
                    //packet is entierly trustworthy, now pass parsed data to Solar Class
                    solarclass.parsePacket(newPacket);  //pass solar class packet so it will do its thing
                    solarclass.VoltageDisplay(labelSolarPanelVoltage, labelBatteryVoltage);   //display the project voltages
                    solarclass.CurrentDisplay(labelLoad1Current, labelLoad2Current, labelBatteryCurrent);
                }
                else
                {
                    //packet is untrusted
                    chkSumError++;
                }
            }
        }
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

    private void switchLoad1CurrentSink_Toggled(object sender, ToggledEventArgs e)
    {
        if(switchLoad1CurrentSink.IsToggled)
        {
            PacketSend(solarclass.controlPacketUpdate(io0: 0));
        }
        if(!switchLoad1CurrentSink.IsToggled)
        {
            PacketSend(solarclass.controlPacketUpdate(io0: 1));
        }
    }
    public void PacketSend(string UserSentPacket)
    {
        try
        {
            string messageOut = UserSentPacket;                         //capture user inputed packet
            messageOut += "\r\n";                                       //appends carriage return line feed
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageOut);   //ensures item is actually a byte
            serialport.Write(messageBytes, 0, messageBytes.Length);     //send our stuff over serial port
        }
        catch (Exception ex)
        {
            DisplayAlert("Alert", ex.Message, "Ok");
        }
    }

    private void switchLoad2CurrentSink_Toggled(object sender, ToggledEventArgs e)
    {
        if (switchLoad2CurrentSinkName.IsToggled)
        {
            PacketSend(solarclass.controlPacketUpdate(io1: 0));
        }
        if (!switchLoad2CurrentSinkName.IsToggled)
        {
            PacketSend(solarclass.controlPacketUpdate(io1: 1));
        }
    }

    private void chkBoxEnaSerDebug_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (chkBoxEnaSerDebug.IsChecked)
        {
            DebugDataTopLevelStack.IsVisible = true;
        }
        else
        {
            DebugDataTopLevelStack.IsVisible = false;
        }
    }
}




