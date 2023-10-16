using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookTool_GetTokenLD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            Task.Run(()=> {

                string adbCommand = "adb";
                string adbArguments = "shell cat /data/data/com.facebook.katana/app_light_prefs/com.facebook.katana/authentication";

                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = adbCommand,
                        Arguments = adbArguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                string outputData = process.StandardOutput.ReadToEnd();

                string pattern = "access_token\\0(.*?)\\u0005";

                Match match = Regex.Match(outputData, pattern);

                if (match.Success)
                {
                    string accessToken = match.Groups[1].Value;

                    WriteLog(accessToken.Substring(1), "SUCCESS");
                }

            });
        }

        private void WriteLog(string message, string loglevel)
        {
            string formattedMessgae = $"{loglevel}: {message}";

            if (rtxtLogs.InvokeRequired)
            {
                rtxtLogs.Invoke(new Action<string, string>(WriteLog), message, loglevel);
            }
            else
            {
                Color textColor = Color.Black;

                if (loglevel == "ERROR")
                {
                    textColor = Color.Red;
                }
                else if(loglevel == "SUCCESS")
                {
                    textColor = Color.Green;
                }
                else if(loglevel == "WARNING")
                {
                    textColor = Color.Orange;
                }

                rtxtLogs.SelectionStart = rtxtLogs.TextLength;
                rtxtLogs.SelectionLength = 0;
                rtxtLogs.SelectionColor = textColor;
                rtxtLogs.AppendText(formattedMessgae + Environment.NewLine);
                rtxtLogs.SelectionColor = rtxtLogs.ForeColor;

                rtxtLogs.SelectionStart = rtxtLogs.Text.Length;
                rtxtLogs.ScrollToCaret();
            }
        }
    }
}
