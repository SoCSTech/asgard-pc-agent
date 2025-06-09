using MQTTnet;

namespace asgard_pc_agent
{
    public class Worker : BackgroundService
    {
        // Change these options for different use cases!
        private readonly string MQTT_BROKER_URL = "mqtt.socstech.support";
        private readonly int TIME_MS_BETWEEN_PINGS = 6000; // 60,000 = 1 min

        private readonly ILogger<Worker> _logger;
        private IMqttClient _mqttClient;
        private IWorkstation _workstation;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _workstation = new Workstation(_logger);
            
            // Create MQTT Client
            MqttClientFactory mqttFactory = new MqttClientFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
        }

        /// <summary>
        /// Connects to the MQTT Broker and saves it inside of the worker class.
        /// </summary>
        private async Task ConnectToMqtt()
        {

            MqttClientOptions mqttOptions = new MqttClientOptionsBuilder()
               .WithTcpServer(MQTT_BROKER_URL)
               .Build();

            while (!_mqttClient.IsConnected)
            {
                try
                {
                    await _mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Can't connect to MQTT broker " + MQTT_BROKER_URL);
                    _logger.LogError(ex.Message);
                    // Wait before trying again
                    await Task.Delay(TIME_MS_BETWEEN_PINGS, CancellationToken.None);
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Connect to the MQTT Broker
            _logger.LogInformation("Connecting to MQTT Broker: " + MQTT_BROKER_URL);
            await ConnectToMqtt();

            // Main Loop!
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var applicationMessage = new MqttApplicationMessageBuilder()
                           .WithTopic(_workstation.MqttTopic)
                           .WithPayload(_workstation.ToJson())
                           .WithRetainFlag()
                           .Build();

                    await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                    _logger.LogInformation("Sent Ping via MQTT");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    // Reconnect to MQTT on failure
                    await _mqttClient.DisconnectAsync();
                    await ConnectToMqtt();
                }

                // Wait between each ping
                await Task.Delay(TIME_MS_BETWEEN_PINGS, stoppingToken);
            }

            // Disconnect MQTT Client when we are stopping
            await _mqttClient.DisconnectAsync();
        }
    }
}
