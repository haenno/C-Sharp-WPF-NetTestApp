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
using System.Net; // ping
using System.Net.NetworkInformation; //ping
using System.IO;
using System.Threading;

namespace haenno_net_WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonHTMLDo_Click(object sender, RoutedEventArgs e)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(textBoxHTMLaddr.Text);
            string webData = System.Text.Encoding.UTF8.GetString(raw);
            textBoxHTMLcode.Text = webData;
        }



        private void buttonPingDo_Click(object sender, RoutedEventArgs e)
        {

            textBoxPingOutput.Text = "";

            for (int i = 1; i < (Convert.ToInt32(textBoxPingTimes.Text)+1) ; i++)
            {
                textBoxPingOutput.Text += i + ":  ";
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true; // Use the default Ttl value which is 128, but change the fragmentation behavior.

                PingReply reply = pingSender.Send(textBoxIpAdress.Text, 120, Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"), options);

                if (reply.Status == IPStatus.Success)
                {
                    textBoxPingOutput.Text += "Address: " + reply.Address.ToString() + ", ";
                    textBoxPingOutput.Text += "RoundTrip time: " + reply.RoundtripTime + ", ";
                    textBoxPingOutput.Text += "Time to live: "+ reply.Options.Ttl + ", ";
                    textBoxPingOutput.Text += "Don't fragment: "+ reply.Options.DontFragment + ", ";
                    textBoxPingOutput.Text += "Buffer size: "+ reply.Buffer.Length + "\n";
                }
                else
                {
                    textBoxPingOutput.Text += "Ping erfolglos.\n";
                }
     
                textBoxPingOutput.ScrollToEnd();

                Thread.Sleep(Convert.ToInt32(textBoxPingPause.Text));

            }


        }


        private void button_Click(object sender, RoutedEventArgs e)
        {


            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            textBoxNicDaten.Text += "Interface information for    " + computerProperties.HostName + " . " + computerProperties.DomainName + "\n";
            if (nics == null || nics.Length < 1)
            {
                textBoxNicDaten.Text += "  No network interfaces found.\n";
                return;
            }

            textBoxNicDaten.Text += "  Number of interfaces .................... : " + nics.Length + "\n";
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                textBoxNicDaten.Text += "\n";
                textBoxNicDaten.Text += adapter.Description + "\n";
                textBoxNicDaten.Text += String.Empty.PadLeft(adapter.Description.Length, '=') + "\n";
                textBoxNicDaten.Text += "  Interface type .......................... : " + adapter.NetworkInterfaceType + "\n";
                textBoxNicDaten.Text += "  Physical Address ........................ : " + adapter.GetPhysicalAddress().ToString() + "\n";
                textBoxNicDaten.Text += "  Operational status ...................... : " + adapter.OperationalStatus + "\n";
                string versions = "";

                // Create a display string for the supported IP versions.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                textBoxNicDaten.Text += "  IP version .............................. : " + versions + "\n";
                //ShowIPAddresses(properties);

                // The following information is not useful for loopback adapters.
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                textBoxNicDaten.Text += "  DNS suffix .............................. : " + properties.DnsSuffix + "\n";

                string label;
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    textBoxNicDaten.Text += "  MTU...................................... : " + ipv4.Mtu + "\n";
                    if (ipv4.UsesWins)
                    {

                        IPAddressCollection winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            label = "  WINS Servers ............................ :";
                            //ShowIPAddresses(label, winsServers);
                        }
                    }
                }

                textBoxNicDaten.Text += "  DNS enabled ............................. : " + properties.IsDnsEnabled + "\n";
                textBoxNicDaten.Text += "  Dynamically configured DNS .............. : " + properties.IsDynamicDnsEnabled + "\n";
                textBoxNicDaten.Text += "  Receive Only ............................ : " + adapter.IsReceiveOnly + "\n";
                textBoxNicDaten.Text += "  Multicast ............................... : " + adapter.SupportsMulticast + "\n";
                //ShowInterfaceStatistics(adapter);


            }
        }
    }
}
