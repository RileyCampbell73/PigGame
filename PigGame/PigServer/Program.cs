///File Name: Program.cs
///Authors: James Haig, Riley Campbell
///Date: 4/11/2014
///Version 1.00.00
///Purpose: This class represents the service that clients connect to.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PigLib;
using System.ServiceModel;

namespace PigServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Endpoint Address
                ServiceHost servHost = new ServiceHost(typeof(Pig));

                // Start the service
                servHost.Open();

                // Keep the service running until <Enter> is pressed
                Console.WriteLine("Pig service is activated, Press <Enter> to quit.");
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
