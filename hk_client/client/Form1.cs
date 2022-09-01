using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using INIRW;

namespace client
{
    public partial class Form1 : Form
    {
        string IP;
        string Port;
        string FileName;
        string FilePath;
        TcpClient tcpClient;
        public Form1()
        {
            InitializeComponent();
        }
        public void SendMsg(string msg, TcpClient tmpTcpClient)
        {
            NetworkStream ns = tmpTcpClient.GetStream();
            if (ns.CanWrite)
            {
                byte[] msgByte = Encoding.Default.GetBytes(msg);
                ns.Write(msgByte, 0, msgByte.Length);
            }
        }
        public string ReceiveMsg(TcpClient tmpTcpClient)
        {
            string receiveMsg = string.Empty;
            byte[] receiveBytes = new byte[tmpTcpClient.ReceiveBufferSize];
            int numberOfBytesRead = 0;
            NetworkStream ns = tmpTcpClient.GetStream();

            if (ns.CanRead)
            {
                do
                {
                    numberOfBytesRead = ns.Read(receiveBytes, 0, tmpTcpClient.ReceiveBufferSize);
                    receiveMsg = Encoding.Default.GetString(receiveBytes, 0, numberOfBytesRead);
                }
                while (ns.DataAvailable);
            }
            return receiveMsg;
        }
        public void Receivefile(TcpClient tmpTcpClient)
        {
            string receiveMsg = string.Empty;
            byte[] receiveBytes = new byte[tmpTcpClient.ReceiveBufferSize];
            int numberOfBytesRead = 0;
            NetworkStream ns = tmpTcpClient.GetStream();

            if (ns.CanRead)
            {
                do
                {
                    numberOfBytesRead = ns.Read(receiveBytes, 0, tmpTcpClient.ReceiveBufferSize);
                    
                }
                while (ns.DataAvailable);
            }
            /*SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "*.*|*.*";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                fs.Write(receiveBytes, 0, receiveBytes.Length);
                fs.Close();
            }*/
            string strSavePath = FilePath + FileName; 
            FileStream fs = new FileStream(strSavePath, FileMode.Create);
            fs.Write(receiveBytes, 0, receiveBytes.Length);
            fs.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //button1.Enabled = false;
                if (tcpClient==null)
                {
                    //預設主機IP
                    string hostIP = IP;

                    //先建立IPAddress物件,IP為欲連線主機之IP
                    IPAddress ipa = IPAddress.Parse(hostIP);

                    //建立IPEndPoint
                    IPEndPoint ipe = new IPEndPoint(ipa, Int32.Parse(Port));

                    //先建立一個TcpClient;
                    //TcpClient tcpClient = new TcpClient();
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ipe);
                }
                

                if (tcpClient.Connected)
                {
                    string strFile = FileName;

                    SendMsg(strFile, tcpClient);
                    string str = ReceiveMsg(tcpClient);
                    if (str == "@file found")
                    {
                        Receivefile(tcpClient);
                        textBox5.Text = "successful";
                    }
                    else
                    {
                        textBox5.Text = "error";
                    }
                    //textBox1.Text = str;
                    //CommunicationBase cb = new CommunicationBase();
                    //cb.SendMsg("這是客戶端傳給主機的訊息", tcpClient);

                }
            }
            catch(Exception ex)
            {
                button1.Enabled = true;
                MessageBox.Show(ex.Message);
            }
           

        }



        private void Form1_Shown(object sender, EventArgs e)
        {
            var MyIni = new IniFile("Settings.ini");
            IP = MyIni.Read("IP");
            Port = MyIni.Read("PORT");
            FileName = MyIni.Read("FILE_NAME");
            FilePath = MyIni.Read("FILE_PATH");
            textBox1.Text = IP;
            textBox2.Text = Port;
            textBox3.Text = FileName;
            textBox4.Text = FilePath;
        }
    }
}
