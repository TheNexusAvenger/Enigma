# Enigma
*It's an enigma that this can project exist.*

Enigma is a Roblox library and desktop application for providing OpenVR (SteamVR)
tracker information to the Roblox client. Unlike other approaches, [such as tigerVR](https://github.com/200Tigersbloxed/tigerVR),
there is no server component required, but the approach to transfer data has
some limitations.

## Running
In order to run Enigma, the application needs to be downloaded from GitHub releases
needs to be downloaded and ran. **The first run sets up a Roblox Studion companion plugin.
It is highly recommended to restart open Studio windows after the first run.**

Once Enigma is running, it wait for OpenVR (SteamVR) to be detected, and start reading
and sending tracker data using the methods below. Data will only be sent when a Roblox
window is focused and when the application is running.

### Command Line Arguments
A couple of command line arguments can be added to change how Enigma runs.
- `--debug`: Enables debug logging (such as the rate for when data is sent).
- `--trace`: Enables trace + debug logging (such as average timings for sending data).
- `--debug-http`: Enables logging for the ASP.NET server used by the comapnion plugin.
  - Logging is disabled by default due to ASP.NET being *very* noisy during normal operation.

## Roblox Library API
The Roblox library has a few functions in the root module.
- `Enigma:Enable()` - Enables reading inputs from the desktop application.
  - While it is fairly safe to call this while not in VR, it is recommended to only
    run it when VR is active.
- `Enigma:GetUserCFrameEnabled(UserCFrame: TrackerRole, Index: number?): boolean` -
  Returns if the tracker for a role is active. In most cases, the index (defaults
  to 1) is not needed, but since SteamVR allows for multiple trackers with the
  same role, multiple can be read. For a list of `TrackerRoles`, see [TrackerRole.luau](./RobloxLibrary/src/Data/TrackerRole.luau).
- `Enigma:GetUserCFrame(UserCFrame: TrackerRole, Index: number?): CFrame?` -
  Returns the CFrame for a tracker if it is active, or `nil` otherwise. In most cases,
  the index (defaults to 1) is not needed, but since SteamVR allows for multiple
  trackers with the same role, multiple can be read. For a list of `TrackerRoles`,
  see [TrackerRole.luau](./RobloxLibrary/src/Data/TrackerRole.luau).

Below is a very simple example:
```luau
local Enigma = require(game:GetService("ReplicatedStorage"):WaitForChild("Enigma"))

Enigma:Enable()
while true do
    print(`Left foot CFrame: {Enigma:GetUserCFrame("LeftFoot")}`) --Might be nil at any time!
    task.wait()
end
```

The UserCFrames returned are meant to be mixed with the results of
`UserInputService::GetUserCFrame`.

## How It Works
### Normal Operation
Enigma sends data using a combination of the system clipboard and a `TextBox` in
Roblox. When an active Roblox window is detected, Enigma will continuously overwrite
the keyboard, then run Ctrl + A and Ctrl + V to override the data in the TextBox.

### Companion Plugin (Roblox Studio Only)
Due to blindly pasting with Roblox Studio being destructive, a companion plugin
is automatically set up when Enigma is run. The plugin will always attempt to
send heartbeat requests every 3 seconds to disable pasting to Roblox Studio, 
and will poll the latest data that would have been pushed about 60 times a second
*only while in run mode*.

## Limitations
- Contents of the clipboard are always overwritten. Anything in the clipboard
  before running will be lost.
- Keyboard inputs (including WASD movement) and `TextBox` inputs will be non-functional.
  Gamepad or mouse/touch inputs are required.
- Data delivery is unreliable. While order is guaranteed, messages being delivered
  is not. Sometimes, the clipboard will paste at the end of the `TextBox` instead
  of replacing. For custom messages, a schema that is tolerant of extra characters
  (JSON is not, for example) is strongly recommended.
- Constant keyboard inputs will disable Roblox's default gamepad navigation.
  It will need to be re-implemented.
- **Enigma might be incompatible with specific experiences, or have major performance
  issues in specific games.** Issues specific to experiences must be brought to the
  experience first. Issues specific to systems that implement it (like
  [Nexus VR Character Model](https://github.com/TheNexusAvenger/Nexus-VR-Character-Model))
  must be brought to the project first.
- Meta Quest standalone is not possible to support, and Meta Quest Link does not seem
  to be detected by OpenVR.
- Inertial Measurement Unit (IMU)-based trackers are untested. Special support might
  be required.

### macOS and Linux Support
At the moment, macOS and Linux probably do not work. It is unclear if they can be supported.
- `Enigma.Core.Shim.Window.WindowsWindowState` is Windows-specific. macOS and Linux
  versions would need to be created.
- [`InputSimulatorStandard` is Windows-specific](https://github.com/GregsStack/InputSimulatorStandard/issues/61).
- It is unclear if `OVRSharp` supports macOS or Linux.
- The Roblox client is currently blocked on Linux.

[`TextCopy`](https://github.com/CopyText/TextCopy) and ASP.NET Core are cross-platform, though.

## Building
Enigma uses [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for the
desktop application and [Rojo](https://github.com/rojo-rbx/rojo/) for the companion
plugin and Roblox library. However, using [Python](https://www.python.org/) with the
`publish.py` script is the recommended way to create new releases.

```bash
# Windows
python ./publish.py
# macOS/Linux
python3 ./publish.py
```

When creating releases, .NET will use Native AOT for building to create smaller
binaries with better performance and less memory. For macOS and Linux,
[see the prerequisites section](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=net7%2Cwindows#prerequisites)
on Microsoft's documentation.

## Custom Messsages
While Enigma's desktop application and intended use is for SteamVR trackers,
all components are set up for Enigma to be re-used for generic messages.

Below is an example of how to send a generic message using `Enigma.Core`:
```CSharp
await RobloxPlugins.CopyPluginAsync(); // Optional to set up the plugin, but only call once!
var appInstances = new AppInstances(); // Only call this once!
await appInstances.RobloxOutput.PushDataAsync("MyData");
```

And to read data on the client:
```luau
local ReplicatedStorage = game:GetService("ReplicatedStorage")

local Enigma = ReplicatedStorage:WaitForChild("Enigma")
local CombinedInput = require(Enigma:WaitForChild("Input"):WaitForChild("CombinedInput"))
local CompanionPluginInput = require(Enigma:WaitForChild("Input"):WaitForChild("CompanionPluginInput"))
local TextBoxInput = require(Enigma:WaitForChild("Input"):WaitForChild("TextBoxInput"))

local Input = CombinedInput.new(CombinedInput.new(), CompanionPluginInput.new())
print(Input:GetCurrentText()) --Remember that this can be anything (including empty strings) or 2 messages back-to-back!
```

### Custom Application
You can replicate the main parts of the application by providing the following:
1. When active, send an F13 key press when starting and every 250ms or sooner.
2. After the first F13 key press is sent, send left Ctrl down, then A press, then
   V press, and Ctrl up.
3. (Optional) For the Roblox Studio companion plugin, have the following endpoints:
    a. `GET http://localhost:52821/enigma/data`, which returns the current data
       for the client.
    b. `POST http://localhost:52821/enigma/heartbeat`, which is for the studio to
       say it is active and to disable clipboard pasting. This endpoint is required
       for the data fetching in the companion plugin.

## License
Enigma is available under the terms of the MIT  License. See [LICENSE](LICENSE)
for details.