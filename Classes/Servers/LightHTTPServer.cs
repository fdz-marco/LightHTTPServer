using System.Net;
using System.Collections.Specialized;
using glitcher.core.Utils;

namespace glitcher.core.Servers
{
    /// <summary>
    /// (Class) Light HTTP Server
    /// <br/>
    /// Class to execute a HTTP Server on local.<br/>
    /// The class allow to serve local files, embededd resources, and custom responses.<br/><br/>
    /// **Important**<br/>
    /// To add complete project folder as embedded resources, 
    /// please add the folder as embedded resource in the *.csproj definition:<br/>
    /// <code>&lt;ItemGroup&gt; &lt;EmbeddedResource Include = "Html\*"&gt; &lt;ItemGroup&gt;</code>
    /// </summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.16 - June 16, 2024
    /// </remarks>
    public class LightHTTPServer : LightHTTPServerUtils
    {

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Properties (Private / Public / Getters|Setters / Events)

        private HttpListener? _httpServerListener = null;
        private Dictionary<string, Route<string>> _RoutesList = new Dictionary<string, Route<string>>();
        private Dictionary<string, Route<Task<string>>> _RoutesListAsync = new Dictionary<string, Route<Task<string>>>();
        private List<string>? _endpoints { get => new List<string>(GetAllLocalIPv4().Select(x => $"http://{x}:{this.port}/")); }

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        public CancellationTokenSource? cToken { get; set; } = null;
        public int port { get; set; } = 8080;
        public int maxConnections { get; set; } = 10;
        public List<string>? endpoints { get; set; } = null;
        public string basePathLocal { get; set; } = "www";        
        public string basePathEmbedded { get; set; } = "Html";
        public bool allowCrossOrigin { get; set; } = false;
        public bool running { get; set; } = false;      
        public bool tryServeFirstLocal { get; set; } = false;

        public event EventHandler<ServerEvent>? ChangeOccurred;

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Constructor / Settings / Initialization Tasks

        /// <summary>Class constructor.</summary>
        /// <param name="port">HTTP Server Port (Default: 8080)</param>
        /// <param name="maxConnections">Max Number of Connections</param>
        /// <param name="basePathEmbedded">The base path to find the embedded resource files (Note: Relative to root folder of project).</param>
        /// <param name="basePathLocal">The base path to find the local files (Note: Relative to Application directory).</param>
        /// <param name="allowCrossOrigin">Allow Cross Origin (CORS)</param>
        public LightHTTPServer(int port = 8080, int maxConnections = 10, string basePathEmbedded = "Html", string basePathLocal = "www", bool allowCrossOrigin = false)
        {
            this.port = port;
            this.maxConnections = maxConnections;
            this.basePathEmbedded = basePathEmbedded;
            this.basePathLocal = basePathLocal;
            this.allowCrossOrigin = allowCrossOrigin;
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"Server created. Port: <{port}> | Max Connections: <{maxConnections}> | Base Path Embeded: <{basePathEmbedded}> | Base Path Local: <{basePathLocal}> | Allow CrossOrigin: <{allowCrossOrigin}>.");
        }

        /// <summary>Update settings of HTTP Server</summary>
        /// <param name="port">HTTP Server Port (Default: 8080)</param>
        /// <param name="maxConnections">Max Number of Connections</param>
        /// <param name="basePathLocal">The base path to find the local files (Note: Relative to Application directory).</param>
        /// <param name="allowCrossOrigin">Allow Cross Origin (CORS)</param>
        /// <param name="restart">Restart Server on Update</param>
        /// <returns>(bool *async)</returns>
        public async Task Update(int port = 8080, int maxConnections = 10, string basePathLocal = "www", bool allowCrossOrigin = false, bool restart = true)
        {
            if (restart)
                this.Stop();
            this.cToken = cToken;
            this.port = port;
            this.maxConnections = maxConnections;
            this.basePathLocal = basePathLocal;
            this.allowCrossOrigin = allowCrossOrigin;
            if (restart)
                this.Start();
            Logger.Add(LogLevel.Info, "HTTP Server", $"Updated Settings. Port: <{port}> | Max Connections: <{maxConnections}> | Base Path Embeded: <{basePathEmbedded}> | Base Path Local: <{basePathLocal}> | Allow CrossOrigin: <{allowCrossOrigin}>.");
        }

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Methods: Start / Stop

        /// <summary>Start the HTTP Server.</summary>
        /// <returns>(void)</returns>
        public async Task Start()
        {
            // If running cancel start and return.
            if (this.running)
            {
                Logger.Add(LogLevel.Info, "HTTP Server", $"Server already running.");
                return;
            }

            // Check admin rights to check if is possible to use all endpoints or only local
            if (!AdminRights.IsRunAsAdmin())
            {
                DialogResult dialogResult = MessageBox.Show($"Application needs admin rights to listen all domains ports. " +
                    "Do you want to restart application with admin privilages?", "Administrator Privilages", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    AdminRights.RestartAsAdmin();
                    return;
                }
                else if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("Only local domains ports will be used.", "Administrator Privilages");
                    this.endpoints = new List<string>() { $"http://localhost:{this.port}/", $"http://127.0.0.1:{this.port}/" };
                }
            }
            else
            {
                this.endpoints = _endpoints;
            }

            // Verify End Points
            if (this.endpoints == null)
            {
                this.endpoints = new List<string>() { $"http://localhost:{this.port}/", $"http://127.0.0.1:{this.port}/" };
                Logger.Add(LogLevel.Warning, "HTTP Server", $"End Points List empty. Local Domain will be used.");
            }

            // Force the use of cancellation Token
            if (this.cToken == null)
            {
                this.cToken = new CancellationTokenSource();
                Logger.Add(LogLevel.Warning, "HTTP Server", $"No Cancellation Token used. Token creation forced.");
            }

            // Start Server and all listening endpoints
            _httpServerListener = new HttpListener();
            foreach (String endpoint in this.endpoints)
            {
                _httpServerListener.Prefixes.Add(endpoint);
                Logger.Add(LogLevel.Success, "HTTP Server", $"Listening connections on <{endpoint}>.");
            }
            _httpServerListener.Start();
            NotifyChange("started");

            // Manage multiple requests tasks (threads)
            var requests = new HashSet<Task>();
            for (int i = 0; i < this.maxConnections; i++)
                requests.Add(_httpServerListener.GetContextAsync());

            // Handle Requests (Continous Loop)
            while (!this.cToken.Token.IsCancellationRequested)
            {
                // Listen any of the request and remove an available task (thread) if hear something
                Logger.Add(LogLevel.Info, "HTTP Server", $"Available connections: <{requests.Count}>.");
                Task requestTask = await Task.WhenAny(requests);
                requests.Remove(requestTask);

                // Process request
                if (requestTask is Task<HttpListenerContext>)
                {
                    // Get Task Result (shuold be HTTP Listener Context) and process it
                    HttpListenerContext context = (requestTask as Task<HttpListenerContext>).Result;                    
                    await RequestContextAsync(context);
                    // Add again a request thread after serve response
                    requests.Add(_httpServerListener.GetContextAsync());
                }
            }

            // On Cancellation
            requests.Clear();
        }

        /// <summary>Handle HTTP Request.</summary>
        /// <param name="context">HTTP Context</param>
        /// <returns>(void)</returns>
        public async Task RequestContextAsync(HttpListenerContext context)
        {
            // Peel out the requests and response objects
            HttpListenerRequest? request = context.Request;
            HttpListenerResponse? response = context.Response;

            // Local variable to manage served state
            bool served = false;

            // Create a Request Unique ID (For Logging Identification)
            string requestUID = Guid.NewGuid().ToString().Substring(0, 8);

            // Print out some info about the request
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"URL:<{request.Url}>", requestUID);
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"HttpMethod:<{request.HttpMethod}>", requestUID);
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"UserHostName: <{request.UserHostName}>", requestUID);
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"UserAgent: <{request.UserAgent}>", requestUID);
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"AbsolutePath: <{request.Url.AbsolutePath}>", requestUID);
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"LocalPath: <{request.Url.LocalPath}>", requestUID);
            Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"AbsoluteUri: <{request.Url.AbsoluteUri}>", requestUID);

            // Serve the requested Path or Not found
            string requestedPath = request.Url.AbsolutePath;
            NameValueCollection requestedParams = GetRequestParams(request, requestUID);

            // Allow Cross-Origin Resource Sharing (CORS)
            if (allowCrossOrigin)
            {
                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "POST, GET");
            }

            // Response: Custom Response Synchronous (API Call)
            if (_RoutesList.ContainsKey(requestedPath))
            {
                Func<NameValueCollection, string> callback = _RoutesList[requestedPath].callback;
                string mimeType = _RoutesList[requestedPath].mimeType;
                string content = callback(requestedParams);
                served = await ServeResponseCustomContent(response, requestedPath, content, mimeType, requestUID);
            }
            // Response: Custom Response Asynchronous (API Call)
            else if (_RoutesListAsync.ContainsKey(requestedPath))
            {
                Func<NameValueCollection, Task<string>> callback = _RoutesListAsync[requestedPath].callback;
                string mimeType = _RoutesListAsync[requestedPath].mimeType;
                string content = await callback(requestedParams);
                served = await ServeResponseCustomContent(response, requestedPath,content, mimeType, requestUID);
            }
            // Response: File on Embedded Resources (or) Response: File on App Folder
            else
            {
                // Get Local File or Default Local
                string? localFilePath = GetLocalFilePath(requestedPath, this.basePathLocal, requestUID);
                if (String.IsNullOrEmpty(localFilePath))
                    localFilePath = (!requestedPath.Contains('.')) ? GetLocalDefaultFilePath(requestedPath, this.basePathLocal, requestUID) : null;

                // Get Embeded File or Default Embedded
                string? embeddedFilePath = GetEmbeddedFilePath(requestedPath, this.basePathEmbedded, requestUID);
                if (String.IsNullOrEmpty(embeddedFilePath))
                    embeddedFilePath = (!requestedPath.Contains('.')) ? GetEmbeddedDefaultFilePath(requestedPath, this.basePathEmbedded, requestUID) : null;

                // Serve: Local File (If priority first) > Embedded Resource > Local File
                if (tryServeFirstLocal && !String.IsNullOrEmpty(localFilePath))
                {
                    served = await ServeResponseLocalFile(response, localFilePath, requestUID);
                }
                else if (!String.IsNullOrEmpty(embeddedFilePath))
                {
                    served = await ServeReponseEmbeddedFile(response, embeddedFilePath, requestUID);
                }
                else
                {
                    if (!String.IsNullOrEmpty(localFilePath))
                        served = await ServeResponseLocalFile(response, localFilePath, requestUID);
                }
            }
            // If Not Served (Not Found)
            if (!served)
            {
                served = await ServeReponseErrorNotFound(requestedPath, this.basePathEmbedded, this.basePathLocal, response, requestUID);
            }
            // If Not Served (Server Error)
            if (!served)
            {
                served = await ServeReponseErrorServer(requestedPath, this.basePathEmbedded, this.basePathLocal, response, requestUID);
            }
            // Close Response
            response.Close();
        }

        /// <summary>Stop the HTTP Server.</summary>
        /// <returns>(void)</returns>
        public async void Stop()
        {
            if (!this.running || _httpServerListener == null)
            {
                Logger.Add(LogLevel.Info, "HTTP Server", $"Server not running.");
                return;
            }
            try
            {
                if (cToken != null)
                    cToken.Cancel();
                _httpServerListener?.Stop();
                _httpServerListener?.Close();
                cToken = null;
                Logger.Add(LogLevel.Info, "HTTP Server", $"Server Stopped and Closed.");
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Error, "HTTP Server", $"Error stopping HTTP Server", $"Exception: {ex.Message}.");
            }
            NotifyChange("stopped");
        }

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Methods: Add Routes of Custom Responses (API Calls)

        /// <summary>Add a Route, defining an action to be triggered on request.<br/>
        /// <example>
        /// Example:<br/>
        /// **Defining Route**<br/>
        /// AddRoute("/custom/path", FunctionToExecute);<br/><br/>
        /// **Defining Function**<br/>
        /// public string FunctionToExecute(NameValueCollection requestParams)<br/>{<br/>return requestParams.ToString();<br/>}
        /// </example>
        /// </summary>
        /// <param name="path">Requested Path</param>
        /// <param name="callback">Function name to be called. (Note: Function should have *NameValueCollection* as input variables. Returh should be of type string.)</param>
        /// <param name="mimeType">Custom Mime Type (Default: text/html)</param>
        /// <returns>(void)</returns>
        public void AddRoute(string path, Func<NameValueCollection, string>? callback = null, string mimeType = "text/html")
        {
            Route<string> response;
            response.callback = callback;
            response.mimeType = mimeType;
            _RoutesList.Add(path, response);
        }

        /// <summary>Add a Route, defining an (async) action to be triggered on request.<br/>
        /// <example>
        /// Example:<br/>
        /// **Defining Route**<br/>
        /// AddRoute("/custom/path", AsyncFunctionToExecute);<br/><br/>
        /// **Defining Function**<br/>
        /// public Task&lt;string&gt; AsyncFunctionToExecute(NameValueCollection requestParams)<br/>{<br/>return requestParams.ToString();<br/>}
        /// </example>
        /// </summary>
        /// <param name="path">Requested Path</param>
        /// <param name="callback">Function name to be called. (Note: Async Function should have *NameValueCollection* as input variables. Returh should be of type string.)</param>
        /// <param name="mimeType">Custom Mime Type (Default: text/html)</param>
        /// <returns>(void)</returns>
        public void AddRouteAsync(string path, Func<NameValueCollection, Task<string>>? callback = null, string mimeType = "text/html")
        {
            Route<Task<string>> response;
            response.callback = callback;
            response.mimeType = mimeType;
            _RoutesListAsync.Add(path, response);
        }

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Notifiers / Event Handlers

        /// <summary>Notify a change on HTTP Server.</summary>
        /// <returns>(void)</returns>
        private void NotifyChange(string eventType)
        {
            this.running = (_httpServerListener != null) ? _httpServerListener.IsListening : false;
            if (ChangeOccurred != null)
            {
                ChangeOccurred.Invoke(this, new ServerEvent(eventType));
            }
        }

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

    }
}