# oauth-client-tokenmanager
## Description
Library for authorizing requests and managing tokens against OAuth server using tokens.

The library:
- is fully thread-safe
- supports following requests: **HttpClient, WebClient, HttpWebRequest**
- is capable to serve more clients
- is written in **.NET Standard 2.1**

## Example of usage

Before using * *Manager* * you have to initialize it by calling:

```
    // Get instance of the token manager
    var tokenManager = Manager.GetInstance;

    // Create a list of settings
    var settings = new List<ClientSetting>();
    settings.Add(setting);
    settings.Add(setting2);

    // Initialize the manager prior to use
    tokenManager.InitializeManager(settings);
```

For the complete example, please see the [Samples section](https://github.com/tomasandrek/oauth-client-tokenmanager/tree/master/samples)
