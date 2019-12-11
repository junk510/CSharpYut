using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _16_NetworkStreamClientForm
{
    public partial class Form1 : Form
    {
        Socket client;
        IPEndPoint ipep;

        NetworkStream ns;
        StreamWriter sw;

        Thread tRecv;
        bool isRecv = true;

        delegate void AddMsgData(string data);
        AddMsgData addMsgData = null;

        Rectangle[] rect = new Rectangle[39];
        Random rand = new Random();
        // rectangle 만들기
        int panX = 750;
        int panY = 750;
        int malX = 50;
        int malY = 50;
        int a = 1;
        int b = 1;
        int c = 1;

        int num;

        // 빨간 말
        int w = 0;
        int x = 0;
        // 파란 말
        int y = 0;
        int z = 0;

        SoundPlayer bitmusic = new SoundPlayer();
        SoundPlayer bitDo = new SoundPlayer();
        SoundPlayer bitgae = new SoundPlayer();
        SoundPlayer bitgrl = new SoundPlayer();
        SoundPlayer bityut = new SoundPlayer();
        SoundPlayer bitmo = new SoundPlayer();

        //이미지 gif
        Image firstPicture, Do, gae, grl, yut, mo, image;
        Rectangle rects;

        Image aniImage = null;
        System.Windows.Forms.Timer changeImage = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();
            this.Paint += Form1_Paint;
            this.DoubleBuffered = true;
            this.FormClosed += Form1_FormClosed;
            firstPicture = _16_NetworkStreamClientForm.Properties.Resources.윷놀이첫화면;
            Do = _16_NetworkStreamClientForm.Properties.Resources.도;
            gae = _16_NetworkStreamClientForm.Properties.Resources.개;
            grl = _16_NetworkStreamClientForm.Properties.Resources.걸;
            yut = _16_NetworkStreamClientForm.Properties.Resources.윷;
            mo = _16_NetworkStreamClientForm.Properties.Resources.모;
            image = _16_NetworkStreamClientForm.Properties.Resources.윷놀이GIF;
            rects = new Rectangle(890, 15, 250, 250);
            makeMap();
        }

        void makeMap()
        {
            for (int i = 1; i < 6; i++)
            {
                rect[i] = new Rectangle(panX + 30, panY - (malY * 3) * i + 30, malX, malY);
            }
            for (int i = 6; i < 11; i++)
            {
                rect[i] = new Rectangle(panX - (malX * 3) * a + 30, 0 + 30, malX, malY);
                a++;
            }
            for (int i = 11; i < 16; i++)
            {
                rect[i] = new Rectangle(0 + 30, malY * 3 * b + 30, malX, malY);
                b++;
            }
            for (int i = 16; i < 20; i++)
            {
                rect[i] = new Rectangle(malX * 3 * c + 30, panY + 30, malX, malY);
                c++;
            }
            rect[20] = new Rectangle(625 + 30, 125 + 30, 50, 50);
            rect[21] = new Rectangle(500 + 30, 250 + 30, 50, 50);
            rect[22] = new Rectangle(375 + 30, 375 + 30, 50, 50);
            rect[23] = new Rectangle(250 + 30, 500 + 30, 50, 50);
            rect[24] = new Rectangle(125 + 30, 625 + 30, 50, 50);

            rect[25] = new Rectangle(125 + 30, 125 + 30, 50, 50);
            rect[26] = new Rectangle(250 + 30, 250 + 30, 50, 50);
            rect[27] = new Rectangle(500 + 30, 500 + 30, 50, 50);
            rect[28] = new Rectangle(625 + 30, 625 + 30, 50, 50);
            rect[29] = new Rectangle(750 + 30, 750 + 30, 50, 50);

            //rect[0] = new Rectangle(1300,1300,1,1);

            //rect[31] = new Rectangle(910 + 30, 110 + 30, 50, 50);
            //rect[32] = new Rectangle(910 + 30, 170 + 30, 50, 50);
            //rect[33]= new Rectangle(970 + 30, 110 + 30, 50, 50);
            //rect[34] = new Rectangle(970 + 30, 170 + 30, 50, 50);

            rect[35] = new Rectangle(910 + 30, 650 + 30, 50, 50);
            rect[36] = new Rectangle(910 + 30, 710 + 30, 50, 50);
            rect[37] = new Rectangle(970 + 30, 650 + 30, 50, 50);
            rect[38] = new Rectangle(970 + 30, 710 + 30, 50, 50);
        }

        
        protected override void OnLoad(EventArgs e)
        {
            ImageAnimator.Animate(image, new EventHandler(this.OnFrameChanged));
            //ImageAnimator.StopAnimate(image, new EventHandler(this.OnFrameChanged));
            base.OnLoad(e);
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                isRecv = false;
                if (client != null && client.Connected)
                    client.Close();
            }
            catch (Exception ex)
            {
                AddRecvListBox("Exception : " + ex.Message);
            }
        }

        void ProcessPacket(string recvData)
        {
            string[] datas = recvData.Split(new char[] { '|' });

            switch(datas[0]){
                case "C":
                    AddRecvListBox("<채팅수신> : " + recvData);
                    AddRecvListBox(datas[1]);
                    break;
                case "P":
                    num = Int32.Parse(datas[1]);
                    break;
                case "W":
                    w = Int32.Parse(datas[1]);
                    break;
                case "X":
                    x = Int32.Parse(datas[1]);
                    break;
                case "Y":
                    y = Int32.Parse(datas[1]);
                    break;
                case "Z":
                    z = Int32.Parse(datas[1]);
                    break;
            }
            Invalidate();
        }
        void ThreadRecv()
        {
            StreamReader sr = new StreamReader(ns);
            while (isRecv)
            {
                try
                {
                    string data = sr.ReadLine();
                    ProcessPacket(data);
                    
                }
                catch(Exception ex)
                {
                    AddRecvListBox("Exception : " + ex.Message);
                    btnClientConnect.Enabled = true;
                    btnClientStop.Enabled = false;
                    break;
                }
            }
        }
        

        private void BtnClientConnect_Click(object sender, EventArgs e)
        {
            isRecv = true;
            client = 
                new Socket(AddressFamily.InterNetwork,
                           SocketType.Stream,
                           ProtocolType.Tcp);
            ipep =
                new IPEndPoint(IPAddress.Parse(tbIp.Text),
                                Int32.Parse(tbPort.Text));
            AddRecvListBox("서버 접속 시도...");
            client.Connect(ipep);
            AddRecvListBox("서버 접속 완료");

            ns = new NetworkStream(client);
            sw = new StreamWriter(ns);

            tRecv = new Thread(new ThreadStart(ThreadRecv));
            tRecv.Start();

            btnClientConnect.Enabled = false;
            btnClientStop.Enabled = true;
        }

        private void BtnClientStop_Click(object sender, EventArgs e)
        {
            try
            {
                isRecv = false;
                if (client != null && client.Connected)
                    client.Close();
            }catch(Exception ex)
            {
                AddRecvListBox("Exception : " + ex.Message);
            }
            finally
            {
                btnClientConnect.Enabled = true;
                btnClientStop.Enabled = false;
            }
        }

        private void TbSendData_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    string data = "C|"+tbSendData.Text;
                    sw.WriteLine(data);
                    sw.Flush();         // 즉시 보낸다.
                    AddRecvListBox("<송신> : " + data);
                    tbSendData.Clear();
                    break;
            }
        }

        void AddRecvListBox(string data)
        {
            if (lbRecvData.InvokeRequired)
            {
                // 사용중일 때는 .NET에 등록하게 처리를 맡긴다
                Invoke(addMsgData, new object[] { data });
            }
            else
            {
                lbRecvData.Items.Add(data);
                lbRecvData.SelectedIndex = lbRecvData.Items.Count - 1;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addMsgData = AddRecvListBox;
        }

        private void BtnEraseListBox_Click(object sender, EventArgs e)
        {
            lbRecvData.Items.Clear();
        }

        private void BtnFlip_Click(object sender, EventArgs e)
        {
            int rN = rand.Next(16) + 1;
            if (rN == 1 || rN == 2 || rN == 3 || rN == 4)
            {
                num = 1;
                this.aniImage = this.image;

                changeImage.Interval = 5000;
                changeImage.Enabled = true;
                changeImage.Tick += ChangeImage_Tick;
            }
            else if (rN == 5 || rN == 6 || rN == 7 || rN == 8 || rN == 9 || rN == 10)
            {
                num = 2;
                this.aniImage = this.image;

                changeImage.Interval = 5000;
                changeImage.Enabled = true;
                changeImage.Tick += ChangeImage_Tick;
            }
            else if (rN == 11 || rN == 12 || rN == 13 || rN == 14)
            {
                num = 3;
                this.aniImage = this.image;

                changeImage.Interval = 5000;
                changeImage.Enabled = true;
                changeImage.Tick += ChangeImage_Tick;
            }
            else if (rN == 15)
            {
                num = 4;
                this.aniImage = this.image;

                changeImage.Interval = 5000;
                changeImage.Enabled = true;
                changeImage.Tick += ChangeImage_Tick;
            }
            else if (rN == 16)
            {
                num = 5;
                this.aniImage = this.image;

                changeImage.Interval = 5000;
                changeImage.Enabled = true;
                changeImage.Tick += ChangeImage_Tick;
            }

        }

        private void BtnRed_Click(object sender, EventArgs e)
        {
            if (w < 5)
            {
                w += num;
                if (w == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                if (w == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
                
            }
            else if (w == 5)
            {
                w += (14 + num);
                if (w == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                else if (w == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
            }
            else if (w > 5 && w < 10)
            {
                w += num;
                if (w == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                else if (w == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
            }
            else if (w == 10)
            {
                if (num < 3)
                {
                    w += (num + 14);
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 3)
                {
                    w = 22;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 3)
                {
                    w += (13 + num);
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (w > 10 && w < 15)
            {
                w += num;
                if (w == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                else if (w == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
            }
            else if (w == 15)
            {
                if (num < 5)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    w = 29;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (w == 16)
            {
                if (num < 4)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 4)
                {
                    w = 29;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 17)
            {
                if (num < 3)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 3)
                {
                    w = 29;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 3)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 18)
            {
                if (num == 1)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 2)
                {
                    w = 29;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 2)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 19)
            {
                if (num == 1)
                {
                    w = 29;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 1)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 20)
            {
                if (num < 5)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    w = 15;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (w == 21)
            {
                if (num < 4)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num >= 4)
                {
                    w += (-10 + num);
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (w == 22)
            {
                if (num < 4)
                {
                    w += (4 + num);
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                if (num >= 4)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 23)
            {
                if (num == 1)
                {
                    w += num;
                    if (w == y)
                        if (w == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (w == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
                else if (num >= 2)
                {
                    w += (-10 + num);
                    if (w == y)
                        if (w == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (w == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
            }
            else if (w == 24)
            {
                w += (-10 + num);
                if (w == y)
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
            }
            else if (w == 25)
            {
                if (num == 1)
                {
                    w += num;
                    if (w == y)
                        if (w == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (w == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
                else if (num == 2)
                {
                    w = 22;
                    if (w == y)
                        if (w == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (w == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
                else if (num > 2)
                {
                    w += (-1 + num);
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (w == 26)
            {
                if (num == 1)
                {
                    w = 22;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num < 5)
                {
                    w += (-1 + num);
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 27)
            {
                if (num < 3)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num >= 3)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 28)
            {
                if (num == 1)
                {
                    w += num;
                    if (w == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (w == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 1)
                {
                    w = 35;
                    MessageBox.Show("빨강 1번말 goal");
                }
            }
            else if (w == 29)
            {
                w = 35;
                MessageBox.Show("빨강 1번말 goal");
            }
            if (sw != null && client.Connected)
            {
                string pack = String.Format("W|{0}", w.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("X|{0}", x.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Y|{0}", y.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Z|{0}", z.ToString());
                sw.WriteLine(pack);
                sw.Flush();
            }


            Invalidate();
        }

        private void BtnRed2_Click(object sender, EventArgs e)
        {
            if (x < 5)
            {
                x += num;
                if (x == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                else if (x == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
                else if (x == w)
                {

                }
            }
            else if (x == 5)
            {
                x += (14 + num);
                if (x == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                else if (x == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
            }
            else if (x > 5 && x < 10)
            {
                x += num;
                if (x == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                else if (x == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
            }
            else if (x == 10)
            {
                if (num < 3)
                {
                    x += (num + 14);
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 3)
                {
                    x = 22;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 3)
                {
                    x += (13 + num);
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (x > 10 && x < 15)
            {
                x += num;
                if (x == y)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    y = 0;
                }
                else if (x == z)
                {
                    MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                    z = 0;
                }
            }
            else if (x == 15)
            {
                if (num < 5)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    x = 29;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (x == 16)
            {
                if (num < 4)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 4)
                {
                    x = 29;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 17)
            {
                if (num < 3)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 3)
                {
                    x = 29;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 3)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 18)
            {
                if (num == 1)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 2)
                {
                    x = 29;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 2)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 19)
            {
                if (num == 1)
                {
                    x = 29;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 1)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 20)
            {
                if (num < 5)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    x = 15;
                    if (x == y)
                        if (x == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (x == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
            }
            else if (x == 21)
            {
                if (num < 4)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num >= 4)
                {
                    x += (-10 + num);
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (x == 22)
            {
                if (num < 4)
                {
                    x += (4 + num);
                    if (x == y)
                        if (w == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (x == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
                if (num >= 4)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 23)
            {
                if (num == 1)
                {
                    x += num;
                    if (x == y)
                        if (x == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (x == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
                else if (num >= 2)
                {
                    x += (-10 + num);
                    if (x == y)
                        if (x == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (x == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
            }
            else if (x == 24)
            {
                x += (-10 + num);
                if (x == y)
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
            }
            else if (x == 25)
            {
                if (num == 1)
                {
                    x += num;
                    if (x == y)
                        if (x == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (x == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
                else if (num == 2)
                {
                    x = 22;
                    if (x == y)
                        if (x == y)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            y = 0;
                        }
                        else if (x == z)
                        {
                            MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                            z = 0;
                        }
                }
                else if (num > 2)
                {
                    x += (-1 + num);
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
            }
            else if (x == 26)
            {
                if (num == 1)
                {
                    x = 22;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num < 5)
                {
                    x += (-1 + num);
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num == 5)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 27)
            {
                if (num < 3)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num >= 3)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 28)
            {
                if (num == 1)
                {
                    x += num;
                    if (x == y)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        y = 0;
                    }
                    else if (x == z)
                    {
                        MessageBox.Show("파랑 말이 잡혔습니다\r\n\r\n한번더~!");
                        z = 0;
                    }
                }
                else if (num > 1)
                {
                    x = 36;
                    MessageBox.Show("빨강 2번말 goal");
                }
            }
            else if (x == 29)
            {
                x = 36;
                MessageBox.Show("빨강 2번말 goal");
            }


            if (sw != null && client.Connected)
            {
                string pack = String.Format("W|{0}", w.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("X|{0}", x.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Y|{0}", y.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Z|{0}", z.ToString());
                sw.WriteLine(pack);
                sw.Flush();
            }
            Invalidate();
        }

        private void BtnBlue_Click(object sender, EventArgs e)
        {
            if (y < 5)
            {
                y += num;
                if (y == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (y == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (y == 5)
            {
                y += (14 + num);
                if (y == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (y == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (y > 5 && y < 10)
            {
                y += num;
                if (y == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (y == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (y == 10)
            {
                if (num < 3)
                {
                    y += (num + 14);
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 3)
                {
                    y = 22;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 3)
                {
                    y += (13 + num);
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (y > 10 && y < 15)
            {
                y += num;
                if (y == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (y == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (y == 15)
            {
                if (num < 5)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    y = 29;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (y == 16)
            {
                if (num < 4)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 4)
                {
                    y = 29;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    y = 37;
                    MessageBox.Show("파란 1번말 goal");
                }
            }
            else if (y == 17)
            {
                if (num < 3)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 3)
                {
                    y = 29;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 3)
                {
                    y = 37;
                    MessageBox.Show("파랑 1번말 goal");
                }
            }
            else if (y == 18)
            {
                if (num == 1)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 2)
                {
                    y = 29;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 2)
                {
                    y = 37;
                    MessageBox.Show("파랑 1번말 goal");
                }
            }
            else if (y == 19)
            {
                if (num == 1)
                {
                    y = 29;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 1)
                {
                    y = 37;
                    MessageBox.Show("파랑 1번말 goal");
                }
            }
            else if (y == 20)
            {
                if (num < 5)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    y = 15;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (y == 21)
            {
                if (num < 4)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num >= 4)
                {
                    y += (-10 + num);
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (y == 22)
            {
                if (num < 4)
                {
                    y += (4 + num);
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                if (num >= 4)
                {
                    y = 37;
                    MessageBox.Show("파랑 1번말 goal");
                }
            }
            else if (y == 23)
            {
                if (num == 1)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num >= 2)
                {
                    y += (-10 + num);
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (y == 24)
            {
                y += (-10 + num);
                if (y == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (y == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (y == 25)
            {
                if (num == 1)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 2)
                {
                    y = 22;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 2)
                {
                    y += (-1 + num);
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (y == 26)
            {
                if (num == 1)
                {
                    y = 22;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num < 5)
                {
                    y += (-1 + num);
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    y = 37;
                    MessageBox.Show("파랑 1번말 goal");
                }
            }
            else if (y == 27)
            {
                if (num < 3)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num >= 3)
                {
                    y = 37;
                    MessageBox.Show("파랑 1번말 goal");
                }
            }
            else if (y == 28)
            {
                if (num == 1)
                {
                    y += num;
                    if (y == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (y == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 1)
                {
                    y = 37;
                    MessageBox.Show("파랑 1번말 goal");
                }
            }
            else if (y == 29)
            {
                y = 37;
                MessageBox.Show("파랑 1번말 goal");
            }
            if (sw != null && client.Connected)
            {
                string pack = String.Format("W|{0}", w.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("X|{0}", x.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Y|{0}", y.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Z|{0}", z.ToString());
                sw.WriteLine(pack);
                sw.Flush();
            }
            Invalidate();
        }

        private void BtnBlue2_Click(object sender, EventArgs e)
        {
            if (z < 5)
            {
                z += num;
                if (z == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (z == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (z == 5)
            {
                z += (14 + num);
                if (z == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (z == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (z > 5 && z < 10)
            {
                z += num;
                if (z == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (z == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (z == 10)
            {
                if (num < 3)
                {
                    z += (num + 14);
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 3)
                {
                    z = 22;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 3)
                {
                    z += (13 + num);
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (z > 10 && z < 15)
            {
                z += num;
                if (z == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (z == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (z == 15)
            {
                if (num < 5)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    z = 29;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (z == 16)
            {
                if (num < 4)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 4)
                {
                    z = 29;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    z = 38;
                    MessageBox.Show("파란 2번말 goal");
                }
            }
            else if (z == 17)
            {
                if (num < 3)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 3)
                {
                    z = 29;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 3)
                {
                    z = 38;
                    MessageBox.Show("파랑 2번말 goal");
                }
            }
            else if (z == 18)
            {
                if (num == 1)
                {
                    y += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 2)
                {
                    z = 29;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 2)
                {
                    z = 38;
                    MessageBox.Show("파랑 2번말 goal");
                }
            }
            else if (z == 19)
            {
                if (num == 1)
                {
                    z = 29;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 1)
                {
                    z = 38;
                    MessageBox.Show("파랑 2번말 goal");
                }
            }
            else if (z == 20)
            {
                if (num < 5)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    z = 15;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (z == 21)
            {
                if (num < 4)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num >= 4)
                {
                    z += (-10 + num);
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (z == 22)
            {
                if (num < 4)
                {
                    z += (4 + num);
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                if (num >= 4)
                {
                    z = 38;
                    MessageBox.Show("파랑 2번말 goal");
                }
            }
            else if (z == 23)
            {
                if (num == 1)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num >= 2)
                {
                    z += (-10 + num);
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (z == 24)
            {
                z += (-10 + num);
                if (z == w)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    w = 0;
                }
                else if (z == x)
                {
                    MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                    x = 0;
                }
            }
            else if (z == 25)
            {
                if (num == 1)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 2)
                {
                    z = 22;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 2)
                {
                    z += (-1 + num);
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
            }
            else if (z == 26)
            {
                if (num == 1)
                {
                    z = 22;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num < 5)
                {
                    z += (-1 + num);
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num == 5)
                {
                    z = 38;
                    MessageBox.Show("파랑 2번말 goal");
                }
            }
            else if (z == 27)
            {
                if (num < 3)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num >= 3)
                {
                    z = 38;
                    MessageBox.Show("파랑 2번말 goal");
                }
            }
            else if (z == 28)
            {
                if (num == 1)
                {
                    z += num;
                    if (z == w)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        w = 0;
                    }
                    else if (z == x)
                    {
                        MessageBox.Show("빨강 말이 잡혔습니다\r\n\r\n한번더~!");
                        x = 0;
                    }
                }
                else if (num > 1)
                {
                    z = 38;
                    MessageBox.Show("파랑 2번말 goal");
                }
            }
            else if (z == 29)
            {
                z = 38;
                MessageBox.Show("파랑 2번말 goal");
            }
            if (sw != null && client.Connected)
            {
                string pack = String.Format("W|{0}", w.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("X|{0}", x.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Y|{0}", y.ToString());
                sw.WriteLine(pack);
                sw.Flush();
                pack = String.Format("Z|{0}", z.ToString());
                sw.WriteLine(pack);
                sw.Flush();
            }
            Invalidate();
        }
        private void ChangeImage_Tick(object sender, EventArgs e)
        {
            bitDo.SoundLocation = "도.wav";
            bitgae.SoundLocation = "개.wav";
            bitgrl.SoundLocation = "걸.wav";
            bityut.SoundLocation = "윷.wav";
            bitmo.SoundLocation = "모.wav";
            if (num == 1)
            {
                bitDo.Play();
                this.aniImage = this.Do;
                changeImage.Enabled = false;
                Invalidate();
            }

            if (num == 2)
            {
                bitgae.Play();
                this.aniImage = this.gae;
                changeImage.Enabled = false;
                Invalidate();
            }

            if (num == 3)
            {
                bitgrl.Play();
                this.aniImage = this.grl;
                changeImage.Enabled = false;
                Invalidate();
            }

            if (num == 4)
            {
                bityut.Play();
                this.aniImage = this.yut;
                changeImage.Enabled = false;
                Invalidate();
            }

            if (num == 5)
            {
                bitmo.Play();
                this.aniImage = this.mo;
                changeImage.Enabled = false;
                Invalidate();
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            ImageAnimator.UpdateFrames();
            Pen penGray = new Pen(Brushes.Gray, 5);
            e.Graphics.DrawLine(penGray, 55, 55, 805, 55);
            e.Graphics.DrawLine(penGray, 55, 55, 55, 805);
            e.Graphics.DrawLine(penGray, 805, 55, 805, 805);
            e.Graphics.DrawLine(penGray, 55, 805, 805, 805);
            e.Graphics.DrawLine(penGray, 55, 55, 805, 805);
            e.Graphics.DrawLine(penGray, 805, 55, 55, 805);


            e.Graphics.FillEllipse(Brushes.Gray, 15, 15, 80, 80);
            e.Graphics.FillEllipse(Brushes.Gray, 15, 765, 80, 80);
            e.Graphics.FillEllipse(Brushes.Gray, 765, 15, 80, 80);
            e.Graphics.FillEllipse(Brushes.Gray, 765, 765, 80, 80);
            e.Graphics.FillEllipse(Brushes.Gray, 390, 390, 80, 80);

            Pen pen = new Pen(Brushes.Black, 3);
            for (int i = 1; i < 39; i++)
            {
                e.Graphics.FillEllipse(Brushes.LightGray, rect[i]);
            }

            // 빨간 말 1번 움직임
            e.Graphics.FillEllipse(Brushes.Red, rect[w]);
            // 빨간 말 2번 움직임
            e.Graphics.FillEllipse(Brushes.Crimson, rect[x]);
            // 파란 말 1번 움직임
            e.Graphics.FillEllipse(Brushes.Blue, rect[y]);
            // 파란 말 2번 움직임
            e.Graphics.FillEllipse(Brushes.DarkBlue, rect[z]);

            e.Graphics.DrawImage(this.firstPicture, rects);
            //pictureBox.Enabled = false; GIF를 중지하고 
            //GIF pictureBox.Enabled = true;를 실행하려는 경우에 사용할 수 있습니다 .
            if (num == 1)
            {
                e.Graphics.DrawImage(this.aniImage, rects); //gif - 10초
            }

            if (num == 2)
            {
                e.Graphics.DrawImage(this.aniImage, rects); //gif - 10초
            }

            if (num == 3)
            {
                e.Graphics.DrawImage(this.aniImage, rects); //gif - 10초
            }

            if (num == 4)
            {
                e.Graphics.DrawImage(this.aniImage, rects); //gif - 10초
            }

            if (num == 5)
            {
                e.Graphics.DrawImage(this.aniImage, rects); //gif - 10초
            }
        }
    }

}
