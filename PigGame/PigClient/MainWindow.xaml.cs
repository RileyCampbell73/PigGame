using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ServiceModel;
using PigLib;

namespace PigClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
        UseSynchronizationContext = false)]
    public partial class MainWindow : Window, ICallback
    {
        private IPig pig = null;
        private int callbackId = 0;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Configure the Endpoint details
                DuplexChannelFactory<IPig> channel = new DuplexChannelFactory<IPig>(this, "Pig");

                // Activate a remote Shoe object
                pig = channel.CreateChannel();

                // Regsister this client for the callbacks service
                callbackId = pig.RegisterForCallbacks();


                //updateCardCounts();

                this.Title = "Hello Player " + callbackId;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (callbackId != 0 && pig != null)
                pig.UnregisterForCallbacks(callbackId);
        }

        public void UpdateGui(CallBackInfo info)
        {

        }
    }
}
