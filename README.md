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

## Building an MSI File Manually

You first need to install the [Microsoft Visual Studio Installer Projects 2022](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2022InstallerProjects) extension inside of Visual Studio 2022. This will let you use all the MSI tooling. 

Then you should check your build target is set to `Release` and **not** debug! 

Then press `Build` on the top bar, then `Clean Solution`... And then `Build Solution`.

Then you should have the following `.msi` file built `asgard-pc-agent\asgard-pc-agent.setup\Release\asgard-pc-agent.setup.msi`.

Create this an a release in GitHub and you can now deploy this to the Lab PCs via Deep Freeze.