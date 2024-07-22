using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Management;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.DirectoryServices;
using System.IO;


namespace AlivateLocalAdmin2._0
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("start here");
            AlivateLocalAdminGroup(getUsername());
        }
        public static string getUsername()
        {
            String line;
            Dictionary<string, string> userdata = new Dictionary<string, string>();
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("C:\\usernameData.txt");
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the line to console window
                    Console.WriteLine(line);
                    //Read the next line
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        userdata[key] = value;
                        Console.WriteLine($"Key: {key}, Value:{value}");
                    }
                    else
                    {
                        Console.WriteLine("Input string format is incorrect.");
                    }


                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
            return userdata["username"];
        }

        public static void AlivateLocalAdminGroup(string username)
        {
            string domain = "casa.net";
            //string userPath = $"LDAP://{domain}/CN={username},OU=Basic-Users,OU=VDI-Users,DC=CASA,DC=NET"; // Adjust the LDAP path as needed
            //string groupPath = $"LDAP://{domain}/CN=Local Admins,CN=Users,DC=CASA,DC=NET"; // Adjust the group LDAP path

            try
            {
                // Get the local Administrators group
                string localAdminGroupName = "Administrators";

                using (PrincipalContext localContext = new PrincipalContext(ContextType.Machine))
                using (GroupPrincipal localAdminGroup = GroupPrincipal.FindByIdentity(localContext, localAdminGroupName))
                {
                    if (localAdminGroup == null)
                    {
                        Console.WriteLine($"Local group '{localAdminGroupName}' not found.");
                        return;
                    }

                    // Find the domain user
                    using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, domain))
                    using (UserPrincipal domainUser = UserPrincipal.FindByIdentity(domainContext, username))
                    {
                        if (domainUser == null)
                        {
                            Console.WriteLine($"User '{username}' not found in domain '{domain}'.");
                            return;
                        }

                        // Add the domain user to the local Administrators group
                        localAdminGroup.Members.Add(domainUser);
                        localAdminGroup.Save();

                        Console.WriteLine($"User '{username}' was added to the local '{localAdminGroupName}' group successfully.");
                    }
                }
            }
            catch (PrincipalOperationException ex)
            {
                Console.WriteLine($"Principal operation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
    }
    }
}


