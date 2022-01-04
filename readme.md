# Internet Connection Monitoring

This tool tests the connection to internet performing ping on several internet servers. The tool can be configured to ping any type of server on internet or on local network to control the availability of the server or the network.

Every 5 seconds the program pings several servers and report the performance in the console and in a file. If none of the server answers it means the communication is down (network or server). While the communication is down, the tool continues to ping the servers waiting at least one server response, meaning the communication is back.
When the communication is back, the tool traces the duration of the disconnection and restart checking of the connection. 

![Console output](/assets/ConsoleExample_.png "Console output example")

![File log](/assets/FileExample.png "File log example")

# How to use

* download the repo
* run the program by `dotnet run`
* `ctrl+C` to stop the program

> to log the result in a file just pipe like this: `dotnet run > myfile.txt`

# Output

In the output you will retrieve 1 line for each test. In each line the information are separated by `|`

The line is composed of:
* **date/time** of the test eg: `02/05/2021 16:48:24`,
* the **result of the test** eg: `OK`, `KO`, `RECONNECT`,
* depending on the result there are detailed information:
  * `OK` ... at least one ping succeeded  
    you will found the **roundtrip** of the ping for each server eg: `02/05/2021 16:45:15|OK          |061|181|061|`.  
    If one server does not answer or timeout the value is replaced by `---` eg: `02/05/2021 16:48:24|OK          |059|069|---|`
    
  *  `KO` ... not ping are succeeding  
    you will found the duration since when the connection is lost eg: `02/05/2021 16:48:31|KO          |pings are failing, disconnected since: 1 seconds`
    > during the disconnection the is only one line, the duration of the disconnection being updated at every test
    > when the communication is working again, the line is replaced by one `RECONNECT` line output
  * `RECONNECT` ... the connection is back  
    you will found the whole duration of disconnection eg:`02/05/2021 16:48:29|RECONNECTION|7|Disconnected during : 7 seconds`

# Configuration file

You can configure some behavior the tool via the `appsetting.json` file, located side to the program. 

Here is the default configuration:
```json
{
    "ping" : {
        "frequency": "5000",
        "servers" : ["8.8.8.8", "4.2.2.2", "208.67.222.222"],
        "loginfile" : true,
        "loginfiledisconnected" : true
    }
}
```
### frequency 
Duration between 2 ping tests performed. The value is defined in milliseconds.

### servers 
The list of the IP servers to ping. By default there is `Google`, `Level 3`, `OpenDNS` IP servers. You can update as you wish.

### loginfile
The tool will log the results in a file, in addition of the console output.

### loginfiledisconnected
When the system is disconnected the log are also written in the file. 
> This could be useful if you don't want to be polluted by all the log when the system is disconnected.

## log information in files

When activating the `loginfile` the output will be persisted in a file.

The file is automatically generated side to the program with the pattern `icm.yyyymmdd.csv` (yyyy for years, mm for months, dd for days ). Doing so you will have 1 file per day.

> If you `dotnet run` the tool. There is a high probability that you will found the log closed to the .dll generated in the folder `.\bin\Debug\net5.0\` 

# To do
- [x] Initialize program, log in the console
- [x] Calculate disconnection duration and show in the log 
- [x] Configuration file 
- [x] Log in file
- [ ] Provide GUI (systray?)
- [ ] Play sound when disconnecting, connecting again
- [ ] support when network card is not connected (currently there is weird error :-( ))

