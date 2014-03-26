using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using PigLib;
using System.ServiceModel;

namespace PigService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                // Endpoint Address
                ServiceHost servHost = new ServiceHost(typeof(Pig));

                // Start the service
                servHost.Open();

                // Keep the service running until <Enter> is pressed
                Console.WriteLine("Pig Game service is activated, Press <Enter> to quit.");
                Console.ReadKey();

                // Shut down the service
                servHost.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
