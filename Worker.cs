using MQTTnet;

namespace asgard_pc_agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IMqttClient _mqttClient;
        private IWorkstation _workstation;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            MqttClientFactory mqttFactory = new MqttClientFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
            _workstation = new Workstation();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Connect to the MQTT Broker
            Console.WriteLine("Connecting to MQTT Broker: mqtt.iamhardcodedpleaseupdate.bogon");

            MqttClientOptions mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("mqtt.socstech.support")
                .Build();

            await _mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);

            // Main Loop!
            while (!stoppingToken.IsCancellationRequested)
            {

                var applicationMessage = new MqttApplicationMessageBuilder()
                       .WithTopic(_workstation.MqttTopic)
                       .WithPayload(_workstation.ToJson())
                       .Build();

                await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                Console.WriteLine("Sent Ping to MQTT");

                await Task.Delay(1000, stoppingToken);
            }

            // Disconnect MQTT Client when we are stopping
            await _mqttClient.DisconnectAsync();
        }
    }
}
