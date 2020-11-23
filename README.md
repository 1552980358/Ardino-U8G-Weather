# Ardino-U8G-Weather

### This repo consists of two projects
1. `ArduinoSketch`
2. `HostServerDotNET`

### `ArduinoSketch`
 - It is the sketch file that will be compiled and write into flash of Arudino board
 - Tested board: Arduino Uno (Modified version from Taobao)
 - Workable board: Arduino Uno (Official)
 - Compile tool: `Arduino IDE` (Recommanded & tested), `Jetbrains CLion` (tested), or `Visual Studio`
 
### `HostServerDotNET`
 - It is the console program that runs on the host computer device, that will be launched after boot up
 - Compile tool: `Jetbrains Rider` (Recommanded & tested) or `Visual Studio`
 
### Write `ArduinoSketch` into board flash
 - Arduino IDE
  1. Download Arduino IDE
  2. Clone Project
  3. Copy U8glib into `libraries` directory of your Arduino IDE directory
  4. Open [arduino_sketch.ino](https://github.com/1552980358/Ardino-U8G-Weather/tree/master/ArduinoSketch) in IDE
  5. Select your board
  6. Upload file
  
 - Jetbrains CLion
  1. Download and set toolchain as MinGW, DON'T USE Cygwin
  2. Install `Arduino Support` within `Plugin` in CLion
  3. Download Arduino IDE
  4. Copy U8glib into `libraries` directory of your Arduino IDE directory
  5. Clone and import this repo
  6. Edit `ArduinoToolchain.cmake`, modify line 9 as `set(ARDUINO_SDK_PATH <YOUR ARDUINO IDE PATH>)`
  7. Select `ArduinoSketch-upload`
  8. Click `Build 'ArduinoSketch-upload'` (green hammer)
  
### Run `HostServerDotNET`
 - Use pre-compiled .exe file (Recommanded), can be found at assets of [Releases](https://github.com/1552980358/Ardino-U8G-Weather/releases), named starts with `HostServerDotNET`
 - Compile project with `Jetbrains Rider` (Recommanded & tested) or `Visual Studio`
 - ApiKey can be gotten from [https://openweathermap.org/](https://openweathermap.org/) for free
 - Use Command: `<PATH OF YOUR EXE FILE> "<[q={city name},{state code}] or [id={City ID}]>" "<YOUR BOARD COM NUMBER>" "<YOUR BAUDRATE, RECOMMANDED AS 9600>" "<YOUR APIKEY>"`
 - See [https://openweathermap.org/current](https://openweathermap.org/current) for detail API usage

### Photo
![](IMG_1.jpg)
 
 ### Credit
  - U8glib
  - Arduino Team
  - [openweathermap](https://openweathermap.org/)