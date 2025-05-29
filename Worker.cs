using MQTTnet;

namespace asgard_pc_agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IMqttClient _mqttClient; 

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            MqttClientFactory mqttFactory = new MqttClientFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Connect to the 
            MqttClientOptions mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("mqtt.socstech.support")
                .Build();

            await _mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);

            // Main Loop!
            while (!stoppingToken.IsCancellationRequested)
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                       .WithTopic($"josh/testing")
                       .WithPayload("Hello World!")
                       .Build();

                await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

                await Task.Delay(1000, stoppingToken);
            }

            // Disconnect MQTT Client when we are stopping
            await _mqttClient.DisconnectAsync();
        }
    }
}
