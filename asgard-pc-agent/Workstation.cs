using Newtonsoft.Json;

namespace asgard_pc_agent
{
    /// <summary>
    /// Generic Computer interface
    /// </summary>
    internal interface IWorkstation
    {
        /// <summary>
        /// The full hostname of the workstation.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The IP of the workstation that is on the University network.
        /// </summary>
        string IPv4Address { get; }
        /// <summary>
        /// The MAC of the main network port, this is the defacto ID.
        /// </summary>
        string MacAddress { get; }
        /// <summary>
        /// What OS is the computer running, the value should only ever really be "WINDOWS" in this application, 
        /// but we need it to be there so we can do stuff like detect linux/windows usage later!
        /// </summary>
        string OS { get; }
        /// <summary>
        /// How long the workstation has been powered on in seconds. Convert for other units at your own pleasure.
        /// </summary>
        int SessionTimeSeconds { get; }
        /// <summary>
        /// Topic path on which to publish messages about the Workstation on
        /// </summary>
        string MqttTopic { get; }
        /// <summary>
        /// Makes the workstation available as a Json Object
        /// </summary>
        /// <returns>json of workstation details</returns>
        string ToJson();
    }

    /// <summary>
    /// A lab computer
    /// </summary>
    internal class Workstation : IWorkstation
    {
        public string Name => Environment.MachineName;

        private NetworkCard _networkCard => NetworkCard.GetNetworkInfo();
        public string IPv4Address => String.IsNullOrEmpty(_networkCard.IPv4Address) ? "No Network" : _networkCard.IPv4Address;

        public string MacAddress => String.IsNullOrEmpty(_networkCard.MacAddress) ? "No Network" : _networkCard.MacAddress;

        public string OS { get; } = "WINDOWS";
        public int SessionTimeSeconds => (int)Environment.TickCount / 1000;

        public string MqttTopic
        {
            get
            {
                // Is the PC named correctly?
                string[] deskLocation = this.Name.Trim().Split('-');
                if (deskLocation[1] == "MASTER")
                {
                    // asgard/pc/abcdabcd
                    return $"asgard/pc/{this.MacAddress}";
                }
                else if (deskLocation.Length >= 2)
                {
                    // asgard/pc/1a/g6
                    return $"asgard/pc/{deskLocation[0]}/{deskLocation[1]}";
                }
                else
                {
                    // asgard/pc/11WABCDABCD
                    return $"asgard/pc/{this.Name}";
                }
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
