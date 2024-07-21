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
            AlivateLocalAdminGroup("test_local_admin");
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
    }
}


