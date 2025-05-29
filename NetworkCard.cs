using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace asgard_pc_agent
{
    /// <summary>
    /// A Network Card is an interface on the machine this is running on.
    /// This is a helper class to make it easier for us to get v4 addresses and macs.
    /// </summary>
    internal class NetworkCard
    {
        /// <summary>
        /// IPv4 Address on the University Network 
        /// i.e. 10.5.123.123
        /// </summary>
        public string? IPv4Address { get; set; } = "No Network";
        /// <summary>
        /// MAC Address of the network card on the University Network
        /// </summary>
        public string? MacAddress { get; set; } = "No Network";

        /// <summary>
        /// The first number of an IPv4 Address
        /// For the University network all IPs begin with 10.x.x.x
        /// So we are cheating identification as that is less likely to change than our subnet space.
        /// </summary>
        private static int _universityNetworkFirstOctet = 10;

        /// <summary>
        /// Get the IPv4 Address and Mac Address of a Workstation on the University network.
        /// </summary>
        /// <returns>NetworkCard(IPv4Address (string?), MacAddress(string?))</returns>
        public static NetworkCard GetNetworkInfo()
        {
            // Get all network interfaces on the machine
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // Iterate through each network interface
            foreach (var ni in networkInterfaces)
            {
                // Filter for interfaces that are not operational (Down) and are not either Ethernet or Wireless
                if (ni.OperationalStatus == OperationalStatus.Down &&
                    !(ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    continue;
                }

                // Get IP properties for the current network interface
                var ipProperties = ni.GetIPProperties();

                // Iterate through all Unicast IP addresses for this interface
                foreach (var unicastAddress in ipProperties.UnicastAddresses)
                {
                    // Ensure the address is not null and is an IPv4 address
                    if (unicastAddress?.Address != null && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        // Get the IP address as a byte array
                        byte[] ipBytes = unicastAddress.Address.GetAddressBytes();

                        // Check if the first octet (byte) is 10
                        if (ipBytes.Length == 4 && ipBytes[0] != _universityNetworkFirstOctet)
                        {
                            continue;
                        }

                        // If a matching IP address is found, create and return NetworkDeviceInfo
                        return new NetworkCard
                        {
                            // v4 Address
                            IPv4Address = unicastAddress.Address.ToString(),
                            // Get the physical (MAC) address of the current network interface
                            MacAddress = ni.GetPhysicalAddress().ToString()
                        };
                    }
                }

            }
            
            // Default Return 'No Network'
            return new NetworkCard
            {
                IPv4Address = "No Network",
                MacAddress = "No Network"
            };
        }
    }
}