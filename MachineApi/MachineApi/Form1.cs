using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace MachineApi
{
    public partial class Form1 : Form
    {
      // Define a Variable for uses.
        private Timer timer;
        private byte Num_Count = 5;
        private String Channel, Date, Time,User;
        private String TempChannel = "";
        public Form1()
        {
            InitializeComponent();

            //Set a timer Overflow every 1000 ms
            timer = new Timer();
            timer.Interval = 1000;

            //For Timer Tick
            timer.Tick += TimerTicker;
            
        }
        //Create a Timer Ticker Function.
        void TimerTicker(object sender, EventArgs e)
        {
            // Count Down for 5 Second
            Num_Count--;

            if (Num_Count == 0)
            {
                //Restart Count.
                Num_Count = 5;

                //If Count Down = 5 Second 
                //Connect To a database.
                ConnectDatabase();
            }
        }
        private void OpenSerialPort()
        {
            if (serialPort1.IsOpen == false)
            {
                // Open serial port
                serialPort1.Open();

                //Send Data.
                SendDatas();

                // Terminate a Serial port.
                serialPort1.Close();
            }

            else
            {
               // Serial Port is Ready to Send a Data.
            }

        }
        private void SendDatas()
        {
            int DataLength = Channel.Length;
            for (int i = 0; i < DataLength; i++)
            {
                if (serialPort1.IsOpen == true)
                {
                    serialPort1.Write(Channel[i].ToString());
                }

                // Delay for Microcontroller receive Data
                System.Threading.Thread.Sleep(500);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Do something 
        }

        private void OpenSerial_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void ConnectDatabase()
        {
            //using MySql.Data.MySqlClient; 
            // Connect to database and Uses a Sql Command to qeury.
            string stConnect = @"server=localhost;database=channel;userid=root;password=korn1993";

            //Server and DB Connnect.
            MySqlConnection ConnectDB = new MySqlConnection(stConnect);
            try
            {
                ConnectDB.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM channel", ConnectDB);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                // Read a data from database. 
                while (dataReader.Read())
                {
                    Channel = dataReader["channel"].ToString();
                    Date = dataReader["Date"].ToString();
                    Time = dataReader["time"].ToString();
                    User = dataReader["Userid"].ToString();
                    break;
                }


                //close Data Reader
                dataReader.Close();

                //close Connection
                ConnectDB.Close();

                /* CHECK IF DATA IS SAME AT THE TIME
                 * If First Time receive A TempChannel is No data
                 * And We push a Data (Channel) to TempChannel, So TempChannel = Channel.
                 * The next time we can Check if Data same, Donothing.
                */

                if (Channel == TempChannel)
                {
                    TempChannel = Channel;

                    //Show in label
                    label2.Text = User;

                    label3.Text = Date;
                    label4.Text = Time;
                }
                else
                {
                    
                    TempChannel = Channel;

                    // Show a channel From database.
                    textBox1.Text = Channel.ToString();

                    //Show in label
                    label2.Text = User;
                    label3.Text = Date;
                    label4.Text = Time;

                    //Open A serial port. 
                    // Ready to send a Data. 
                    OpenSerialPort();

                }
            }
            catch
            {
                // Catch Error.
                MessageBox.Show("Error Connection.");
            }

        }
    }
}
