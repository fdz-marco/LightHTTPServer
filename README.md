# LightHTTPServer

This application is to test a simple http server class for bigger projects.
The class allow to serve local files (from App Directory), embededd resources (on Project), and custom responses (for API Responses).

Use:
- Copy all the content of the folder Classes in your project.
- Add the next references of namespaces in your project: 
```cs
using glitcher.core;
using Servers = glitcher.core.Servers;
```

- To start a server create an object of the class and call the method start:
```cs
LightHTTPServer httpServer = new Servers.LightHTTPServer();
httpServer.Start();
```

- Is also possible to put server settings from object creation:
```cs
int port = 8080;
int maxConnections = 10;
string basePathEmbedded = "Html";
string basePathLocal = "www";
bool allowCrossOrigin = true;
bool autostart = false;

Servers.LightHTTPServer httpServer = new Servers.LightHTTPServer(port, maxConnections, basePathEmbedded, basePathLocal, allowCrossOrigin, autostart);
httpServer.Start();
```

- Is also possible to restart the server with new settings:
```cs
int port = 8080;
int maxConnections = 10;
string basePathLocal = "www";
bool allowCrossOrigin = true;
bool restartOnUpdate = true;

httpServer.Update(port, maxConnections, basePathLocal, allowCrossOrigin, restartOnUpdate);
```

---

### Screenshot of the HTTP Server Tester
![HTTPServerTester](readme_img_httptester.png?raw=true "HTTP Server Tester")

---

### Screenshot of the Browser Serving Embedded Content
![Embedded Content](readme_img_embeddedcontent.png?raw=true "Embedded Content")
![Browser 1](readme_img_browser01.png?raw=true "Browser 1")
![Browser 2](readme_img_browser02.png?raw=true "Browser 2")
![Browser 3](readme_img_browser03.png?raw=true "Browser 3")

---
