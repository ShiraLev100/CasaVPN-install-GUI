using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SMBClientApp
{
    public class SMBClient
    {
        private string _username;
        private string _password;
        private int _port = 445;
        private string _shareName;
        private string _serverName;
        private string _server;

        public SMBClient(string username, string password, string servername, string shareName)
        {
            _username = username;
            _password = password;
            _serverName = servername;
            _shareName = shareName;
            _server = "";
        }

        private void Connect()
        {
            // Connect and authenticate to the SMB share.
            throw new NotImplementedException("Connection logic needs to be implemented in C#.");
        }

        private void DownloadDir(string path, string destDir)
        {
            // Download files from the remote share.
            Directory.CreateDirectory(destDir);

            // Logic for listing and downloading files needs implementation.
            throw new NotImplementedException("Download logic needs to be implemented in C#.");
        }

        private void DownloadFile(string path, string fileName, string destPath)
        {
            // Download files from the remote share.
            Directory.CreateDirectory(destPath);

            // Logic for downloading a specific file needs implementation.
            throw new NotImplementedException("Download file logic needs to be implemented in C#.");
        }
    }

    public static class VPNCommands
    {
        public static (string stdout, string stderr) RunVpncmdCommand(string vpncmdPath, string command)
        {
            // Construct the full command to run
            string fullCommand = $"\"{vpncmdPath}\" {command}";

            // Run vpncmd command
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {fullCommand}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        string stdout = process.StandardOutput.ReadToEnd();
                        string stderr = process.StandardError.ReadToEnd();
                        Console.WriteLine("Command output: " + stdout);
                        Console.WriteLine("Command errors: " + stderr);
                        return (stdout, stderr);
                    }
                    else
                    {
                        Console.WriteLine("Failed to start process.");
                        return (null, null);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error executing command: " + e.Message);
                return (null, null);
            }
        }

        public static void ConfigureSoftEtherVPN(string vpnServer, string vpnHub, string username, string password, string connectionName)
        {
            string vpncmdPath = @"C:\Program Files\SoftEther VPN Client\vpncmd.exe";
            string disaccountCommand = $"localhost /CLIENT /CMD AccountDisconnect {connectionName}";
            string deleteAccountCommand = $"localhost /CLIENT /CMD AccountDelete {connectionName}";
            string deleteNicCommand = $"localhost /CLIENT /CMD NicDelete {connectionName}";
            string addConnectionCommand = $"localhost /CLIENT /CMD AccountCreate {connectionName} /SERVER:{vpnServer} /HUB:{vpnHub} /USERNAME:{username} /NICNAME:vpn";
            string startupConnectionCommand = $"localhost /CLIENT /CMD AccountStartupSet {connectionName}";
            string setPasswordCommand = $"localhost /CLIENT /CMD AccountPasswordSet {connectionName} /PASSWORD:{password} /TYPE:radius";
            string connectCommand = $"localhost /CLIENT /CMD NicCreate {connectionName}";
            string connectAccountCommand = $"localhost /CLIENT localhost /CMD AccountConnect {connectionName}";

            try
            {
                // Check if the VPN connection exists and disconnect if connected
                var (stdout, stderr) = RunVpncmdCommand(vpncmdPath, $"localhost /CLIENT /CMD AccountStatusGet {connectionName}");
                if (stdout.Contains("Status: Connected"))
                {
                    RunVpncmdCommand(vpncmdPath, disaccountCommand);
                }

                // Delete VPN connection settings
                RunVpncmdCommand(vpncmdPath, deleteAccountCommand);

                // Delete NIC
                RunVpncmdCommand(vpncmdPath, deleteNicCommand);

                // Create new VPN connection settings
                try
                {
                    // Attempt to create the Virtual Network Adapter
                    RunVpncmdCommand(vpncmdPath, addConnectionCommand);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error creating Virtual Network Adapter: {e.Message}");
                    Console.WriteLine("Please choose a different name for the adapter.");
                    return;
                }

                RunVpncmdCommand(vpncmdPath, setPasswordCommand);
                RunVpncmdCommand(vpncmdPath, connectCommand);
                RunVpncmdCommand(vpncmdPath, connectAccountCommand);
                RunVpncmdCommand(vpncmdPath, startupConnectionCommand);

                Console.WriteLine("SoftEther VPN connection configured successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error configuring SoftEther VPN: {e.Message}");
            }
        }
    }

    public class SMBClientGUI
    {
        private readonly Form _master;
        private PictureBox _pictureBox;
        private Label _usernameLabel;
        private TextBox _usernameEntry;
        private Label _passwordLabel;
        private TextBox _passwordEntry;
        private Button _connectButtonWin;

        public SMBClientGUI(Form master)
        {
            _master = master;
            master.Text = "SMB Client GUI";
            master.Size = new Size(400, 600);

            // Configure the custom font

            // Make PictureBox from base64 string
            string base64String = ""; // Insert your base64 string here
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes);
            Image backgroundImg = Image.FromStream(ms);
            _pictureBox = new PictureBox
            {
                Image = backgroundImg,
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            master.Controls.Add(_pictureBox);

            // Configure the La Casa de Papel theme
            ConfigureCasaDePapelTheme();

            _usernameLabel = new Label
            {
                Text = "Enter Casa Username:",
                Location = new Point(10, 170),
                Font = new Font("Times New Roman", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(242, 246, 249),
                BackColor = Color.FromArgb(105, 2, 41)
            };
            master.Controls.Add(_usernameLabel);

            _usernameEntry = new TextBox
            {
                Location = new Point(10, 200),
                Font = new Font("Helvetica", 14),
                Size = new Size(300, 30)
            };
            master.Controls.Add(_usernameEntry);

            _passwordLabel = new Label
            {
                Text = "Enter Password:",
                Location = new Point(10, 240),
                Font = new Font("Times New Roman", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(242, 246, 249),
                BackColor = Color.FromArgb(105, 2, 41)
            };
            master.Controls.Add(_passwordLabel);

            _passwordEntry = new TextBox
            {
                Location = new Point(10, 270),
                Font = new Font("Helvetica", 14),
                Size = new Size(300, 30),
                UseSystemPasswordChar = true
            };
            master.Controls.Add(_passwordEntry);

            _connectButtonWin = new Button
            {
                Text = "Connect to WIN",
                Location = new Point(10, 310),
                Font = new Font("Times New Roman", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(9, 31, 44),
                ForeColor = Color.FromArgb(105, 2, 41)
            };
            _connectButtonWin.Click += ConnectSoftether;
            master.Controls.Add(_connectButtonWin);
        }

        private void ConfigureCasaDePapelTheme()
        {
            // Set the font and background color for labels and buttons
            // Note: You may need to adjust colors and fonts as per your requirements.
            _master.BackColor = Color.FromArgb(105, 2, 41);
            _usernameLabel.BackColor = Color.FromArgb(105, 2, 41);
            _usernameLabel.ForeColor = Color.FromArgb(242, 246, 249);
            _passwordLabel.BackColor = Color.FromArgb(105, 2, 41);
            _passwordLabel.ForeColor = Color.FromArgb(242, 246, 249);
            _connectButtonWin.BackColor = Color.FromArgb(9, 31, 44);
            _connectButtonWin.ForeColor = Color.FromArgb(105, 2, 41);
            _usernameLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
            _passwordLabel.Font = new Font("Times New Roman", 16, FontStyle.Bold);
            _connectButtonWin.Font = new Font("Times New Roman", 16, FontStyle.Bold);
        }

        private void InstallSoftEtherVPNClient(string vpnClientInstallerPath)
        {
            Console.WriteLine("Downloading SoftEther VPN Client installer...");
            if (File.Exists(vpnClientInstallerPath))
            {
                Console.WriteLine("SoftEther VPN Client installer downloaded successfully.");
                Console.WriteLine("Starting SoftEther VPN Client installer...");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = vpnClientInstallerPath,
                    Arguments = "/S",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                    }
                    else
                    {
                        Console.WriteLine("Failed to start SoftEther VPN Client installer.");
                        Environment.Exit(1);
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to download SoftEther VPN Client installer.");
                Environment.Exit(1);
            }
        }

        private void ConnectSoftether(object sender, EventArgs e)
        {
            // Define software folder path
            string softwarePath = @"C:\Software";

            // Define VPN server details
            string vpnUsername = _usernameEntry.Text;
            string vpnPassword = _passwordEntry.Text;

            string vpnClientInstallerPath = Path.Combine(softwarePath, "softether-vpnclient.exe");

            // Check if SoftEther VPN Client is already installed
            if (File.Exists(@"C:\Program Files\SoftEther VPN Client\vpnclient.exe"))
            {
                Console.WriteLine("SoftEther VPN Client is already installed.");
            }
            else
            {
                InstallSoftEtherVPNClient(vpnClientInstallerPath);
            }

            string vpnServer = "10.40.90.101:443"; // Replace with your VPN server address and port
            string vpnHub = "VPN"; // Replace with your VPN hub name
            string connectionName = "VPN"; // Replace with your desired connection name

            try
            {
                VPNCommands.RunVpncmdCommand("vpnclient", "start");

                VPNCommands.ConfigureSoftEtherVPN(vpnServer, vpnHub, vpnUsername, vpnPassword, connectionName);

                // Log that the VPN client has started
                Console.WriteLine("VPN client started successfully.");

                _master.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SoftEther VPN: {ex.Message}");
            }
        }
    }

    class Program
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern bool IsUserAnAdmin();

        static void Main(string[] args)
        {
            if (IsUserAnAdmin())
            {
                Console.WriteLine("Already running as admin.");
                using (Form master = new Form())
                {
                    SMBClientGUI app = new SMBClientGUI(master);
                    master.ShowDialog();
                }
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Not running as admin. Attempting to elevate privileges.");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Environment.GetCommandLineArgs()[0],
                    Verb = "runas",
                    UseShellExecute = true
                };

                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error elevating privileges: {ex.Message}");
                }
            }
        }
    }
}
