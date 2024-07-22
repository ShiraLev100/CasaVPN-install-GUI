using System;
using System.DirectoryServices.AccountManagement;
using System.Management;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.DirectoryServices;
using System.Reflection;
using System.IO;

namespace CasaVPN_install_GUI
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("start here");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UserCredentialsApp.Form1());

            // After closing the form, execute the computer name changing logic
            //ExecuteComputerNameChanger(args);
        }

        public static void ExecuteComputerNameChanger(string domainUser, string domainPassword)
        {
            string domain = "casa.net";
            string baseDn = "DC=casa,DC=net";

            string newComputerName = GeneratePcName();

            while (IsInDomain(domain, baseDn, newComputerName, domainUser, domainPassword))
            {
                newComputerName = GeneratePcName();
            }
            Console.WriteLine(newComputerName);
            ChangeComputerName(newComputerName);

            // Uncomment the next line to enable auto-restart after name change
            //RestartComputer();
        }

        static string GeneratePcName()
        {
            string prefix = "vpnpc";
            Random random = new Random();
            string suffix = random.Next(100, 999).ToString();
            return $"{prefix}{suffix}";
        }

        

        static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo info;

            do
            {
                info = Console.ReadKey(true);
                if (info.Key != ConsoleKey.Enter)
                {
                    password += info.KeyChar;
                    Console.Write("*");
                }
            } while (info.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        static bool IsInDomain(string domain, string baseDn, string computerName, string domainUser, string domainPassword)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, domain, domainUser, domainPassword))
                {
                    using (var searcher = new PrincipalSearcher(new ComputerPrincipal(context) { Name = computerName }))
                    {
                        return searcher.FindOne() != null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return false;
            }
        }

        static void ChangeComputerName(string newName)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine;

                string activeComputerName = "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName";
                RegistryKey activeCmpName = key.CreateSubKey(activeComputerName);
                activeCmpName.SetValue("ComputerName", newName);
                activeCmpName.Close();
                string computerName = "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName";
                RegistryKey cmpName = key.CreateSubKey(computerName);
                cmpName.SetValue("ComputerName", newName);
                cmpName.Close();
                string _hostName = "SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\";
                RegistryKey hostName = key.CreateSubKey(_hostName);
                hostName.SetValue("Hostname", newName);
                hostName.SetValue("NV Hostname", newName);
                hostName.Close();
                Console.WriteLine("Name changed to {", newName, "} seccesfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        /* 
         This function saves the username and password for later on local admin permissions to be given.
         */
        public static void AlivateLocalAdmin(string username, string password)
        {
            string[] lines = { "username: " + username, "password: " + password };



            // Set a variable to the Documents path.
            string docPath = @"C:\";

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "usernameData.txt")))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }

        public static void AlivateLocalAdminGroup(string username)
        {
            string domain = "casa.net";
            string userPath = $"LDAP://{domain}/CN={username},OU=Basic-Users,OU=VDI-Users,DC=CASA,DC=NET"; // Adjust the LDAP path as needed
            string groupPath = $"LDAP://{domain}/CN=Local Admins,CN=Users,DC=CASA,DC=NET"; // Adjust the group LDAP path

            try
            {
                // Get the user entry
                DirectoryEntry userEntry = new DirectoryEntry(userPath);

                // Get the group entry
                DirectoryEntry groupEntry = new DirectoryEntry(groupPath);

                // Add the user to the group
                groupEntry.Invoke("Add", new object[] { userEntry.Path });
                groupEntry.CommitChanges();

                Console.WriteLine($"User {username} was added to the 'Local Admins' group successfully.");
            }
            catch (TargetInvocationException ex)
            {
                Console.WriteLine($"Invocation error: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void RestartComputer()
        {
            Process.Start(new ProcessStartInfo("shutdown", "/r /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
    }
}
