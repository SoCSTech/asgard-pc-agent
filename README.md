# Asgard - PC Agent

A service for Windows machines to report their state back to [Asgard](https://github.com/socstech/asgard) so we can show room occupancy on our signage.

This is a dotnet worker service that runs in the background of a lab PC and then connects to the MQTT broker, it sends 1 ping message every 1 minute with the following information to a topic of `asgard/pc/room/desk`:

```json
{
  "Name": "1A-G6-WINDOWS",
  "IPv4Address": "10.123.123.123",
  "MacAddress": "ABCDABCDABCD",
  "OS": "WINDOWS",
  "SessionTimeSeconds": 86400,
  "IdleTimeSeconds": 0,
  "MqttTopic": "asgard/pc/1A/G6"
}
```