# Internet Connection Monitoring

This program test the connection to internet performing ping on several servers. 

Every 5 seconds the program pings several servers and report the performance in the console. If none of the server answers it means the internet connection is down.
While the internet connection is down, the program continue to ping the servers waiting at least one server response, meaning the connection is back.
When the connection is back, the program trace the duration of the disconnection and continue the check of the connection. 

![Console](/assets/consoleExample.png)

# How to use

* download the repo
* run the program by `dotnet run`
* `ctrl+C` to stop the program

> to log the result in a file just pipe like this: `dotnet run > myfile.txt`

# Configuration file

You can configure the frequency and the server you would like to ping using the file `appsetting.json` 

Here is the default configuration:
```json
{
    "ping" : {
        "frequency": "5000",
        "servers" : ["8.8.8.8", "4.2.2.2", "208.67.222.222"]
    }
}
```

# To do
- [x] Initialize program, log in the console
- [x] Calculate disconnection duration and show in the log 
- [x] Configuration file 
- [ ] Provide GUI
- [ ] Export log file
- [ ] Play sound when disconnecting, connecting again

