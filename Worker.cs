using MQTTnet;

namespace asgard_pc_agent
{
    public class Worker : BackgroundService
    {
        private readonly string MQTT_BROKER_URL = "mqtt.socstech.support";

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
            _logger.LogInformation("Connecting to MQTT Broker: " + MQTT_BROKER_URL);

            MqttClientOptions mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(MQTT_BROKER_URL)
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
                _logger.LogInformation("Sent Ping via MQTT");

                await Task.Delay(1000, stoppingToken);
            }

            // Disconnect MQTT Client when we are stopping
            await _mqttClient.DisconnectAsync();
        }
    }
}
