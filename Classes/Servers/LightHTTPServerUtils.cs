using System.Net;
using System.Text;
using System.Reflection;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Net.NetworkInformation;

namespace glitcher.core.Servers
{
    /// <summary>(Class) Light HTTP Server Utilities</summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.16 - June 16, 2024
    /// </remarks>
    public class LightHTTPServerUtils
    {
        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        /// <summary>Get Local IPs</summary>
        /// <returns>(String[]) List of string with the IPs</returns>
        public static String[] GetAllLocalIPv4()
        {
            List<string>? ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }

            if (ipAddrList.FindIndex(x => x == "127.0.0.1") < 0)
                ipAddrList.Add("127.0.0.1");
            if (ipAddrList.FindIndex(x => x == "localhost") < 0)
                ipAddrList.Add("localhost");
            return ipAddrList.ToArray();
        }

        /// <summary>Get Mime Type by file extension.</summary>
        /// <param name="extension">File Extension</param>
        /// <returns>(string) Mime Type</returns>
        private string GetMimeType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".html": return "text/html";
                case ".htm": return "text/html";
                case ".css": return "text/css";
                case ".js": return "application/javascript";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".json": return "application/json";
                case ".txt": return "text/plain";
                case ".xml": return "application/xml";
                case ".csv": return "text/csv";
                case ".zip": return "application/zip";
                case ".mp3": return "video/mpeg";
                case ".mp4": return "video/mp4";
                case ".pdf": return "application/pdf";
                case ".php": return "application/x-httpd-php";
                case ".svg": return "image/svg+xml";
                case ".ttf": return "font/ttf";
                case ".woff": return "font/woff";
                case ".woff2": return "font/woff2";
                case ".wasm": return "application/wasm";
                case "": return "text/html";
                default: return "application/octet-stream";
            }
        }

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        /// <summary>Generates a collection of the request's parameters from a HTTP Request.</summary>
        /// <param name="request">HTTP Request received.</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(NameValueCollection) Collection of the request's parameters</returns>
        protected NameValueCollection GetRequestParams(HttpListenerRequest request, string requestUID = "Unidentified")
        {
            NameValueCollection queryParams = request.QueryString;
            string queryString = "";

            // GET (should be always executed to read values passed on URL)
            foreach (string? key in queryParams.AllKeys)
            {
                queryString += $"{key}={queryParams[key]}&";
            }

            // POST
            if (request.HttpMethod == "POST")
            {
                using (Stream body = request.InputStream)
                {
                    Encoding encoding = request.ContentEncoding;
                    StreamReader reader = new StreamReader(body, encoding);
                    string requestBody = reader.ReadToEnd();

                    Logger.Add(LogLevel.OnlyDebug, "HTTP Server", $"Request Body: <{requestBody}>.", requestUID);

                    string[] queryStringParts = requestBody.Split("&");
                    foreach (string queryStringPart in queryStringParts)
                    {
                        queryString += queryStringPart + "&";
                        string[] partKeyValue = queryStringPart.Split("=");
                        if (partKeyValue.Length == 2)
                        {
                            queryParams[partKeyValue[0]] = partKeyValue[1];
                        }
                    }
                }
            }

            if (queryString.EndsWith("&"))
                queryString = queryString.Remove(queryString.Length - 1, 1);

            Logger.Add(LogLevel.Info, "HTTP Server", $"Request Parameters: <{queryString}>.", requestUID);
            return queryParams;
        }

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        /// <summary>Get a Local File Path from Requested Path (Note: Relative to Application directory).</summary>
        /// <param name="requestedPath">Requested Path.</param>
        /// <param name="basePathLocal">The base path to find the local file (Note: Relative to Application directory).</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(string) Local File Path</returns>
        protected string? GetLocalFilePath(string requestedPath, string basePathLocal, string requestUID = "Unidentified")
        {
            // Get current directory (Application)
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            // Return Requested FilePath (No Change)
            string filePath = Path.Combine(appPath, basePathLocal, requestedPath.TrimStart('/'));
            if (File.Exists(filePath)) 
            {
                Logger.Add(LogLevel.Info, "HTTP Server", $"Found on Local. Local File Path: <{filePath}>", requestUID);
                return filePath;
            }

            // Return Requested FilePath (Not Found)
            Logger.Add(LogLevel.Warning, "HTTP Server", $"Not Found on Local. Request Path: <{requestedPath}>", requestUID);
            return null;
        }

        /// <summary>Get a Embedded File Path from Requested Path (Note: Relative to root folder of project).</summary>
        /// <param name="requestedPath">Requested Path.</param>
        /// <param name="basePathEmbedded">The base path to find the embedded resource file (Note: Relative to root folder of project).</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(string) Embedded File Path</returns>
        protected string? GetEmbeddedFilePath(string requestedPath, string basePathEmbedded, string requestUID = "Unidentified")
        {
            // Get the assembly (Application) and all the resources on assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());

            // Return Requested Embbeded Resource (No Change)
            string resourceFilePath = $"{basePathEmbedded}/{requestedPath.TrimStart('/')}";
            resourceFilePath = resourceFilePath.Replace(@"\", "/");
            resourceFilePath = resourceFilePath.Replace(@"//", "/");
            resourceFilePath = resourceFilePath.Replace(@"/", ".");
            resourceFilePath = resourceNames.FirstOrDefault(r => r.EndsWith(resourceFilePath), "");

            // Return Requested Embbeded Resource (Found)
            if (!string.IsNullOrEmpty(resourceFilePath))
            {
                Logger.Add(LogLevel.Info, "HTTP Server", $"Found on Resources. Embedded File: <{resourceFilePath}>", requestUID);
                return resourceFilePath;
            }

            // Return Requested Embbeded Resource (Not Found)
            Logger.Add(LogLevel.Warning, "HTTP Server", $"Not Found on Resources. Request Path: <{requestedPath}>", requestUID);
            return null;
        }

        /// <summary>Get a Default Local File Path from Requested Path (Note: Relative to Application directory).</summary>
        /// <param name="requestedPath">Requested Path.</param>
        /// <param name="basePathLocal">The base path to find the local file (Note: Relative to Application directory).</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(string) Local Default File Path</returns>
        protected string? GetLocalDefaultFilePath(string requestedPath, string basePathLocal, string requestUID = "Unidentified")
        {
            // Clean requested path
            requestedPath = requestedPath.Replace(@"\", "/");
            requestedPath = requestedPath.Replace(@"//", "/");
            requestedPath = requestedPath.Contains('.') ? requestedPath.Substring(0, requestedPath.LastIndexOf('/')) : requestedPath;
            basePathLocal = Path.Combine(basePathLocal, requestedPath.TrimStart('/'));

            // Return Default Local File Path
            string[] defaultFiles = { "index.html", "index.htm", "default.html", "default.htm", "index.php" };
            foreach (string indexFile in defaultFiles)
            {
                string? filePath = GetLocalFilePath(indexFile, basePathLocal, requestUID);
                if (!String.IsNullOrEmpty(filePath)) 
                {
                    Logger.Add(LogLevel.Info, "HTTP Server", $"Found Default. Local File Path: <{filePath}>", requestUID);
                    return filePath;
                }
            }

            // Return Default Local File Path (Not Found)
            Logger.Add(LogLevel.Warning, "HTTP Server", $"Not Found Default. Local File.", requestUID);
            return null;
        }

        /// <summary>Get a Defalt Embedded File Path from Requested Path (Note: Relative to root folder of project).</summary>
        /// <param name="requestedPath">Requested Path.</param>
        /// <param name="basePathEmbedded">The base path to find the embedded resource file (Note: Relative to root folder of project).</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(string) Embedded Default File Path</returns>
        protected string? GetEmbeddedDefaultFilePath(string requestedPath, string basePathEmbedded, string requestUID = "Unidentified")
        {
            // Clean requested path
            requestedPath = requestedPath.Replace(@"\", "/");
            requestedPath = requestedPath.Replace(@"//", "/");
            requestedPath = requestedPath.Contains('.') ? requestedPath.Substring(0, requestedPath.LastIndexOf('/')) : requestedPath;
            basePathEmbedded = Path.Combine(basePathEmbedded, requestedPath.TrimStart('/'));

            // Return Default Embbeded Resource 
            string[] defaultFiles = { "index.html", "index.htm", "default.html", "default.htm", "index.php" };
            foreach (string indexFile in defaultFiles)
            {
                string? resourceFilePath = GetEmbeddedFilePath(indexFile, basePathEmbedded, requestUID);
                if (!String.IsNullOrEmpty(resourceFilePath))
                {
                    Logger.Add(LogLevel.Info, "HTTP Server", $"Found Default. Embedded Path: <{resourceFilePath}>", requestUID);
                    return resourceFilePath;
                }
            }

            // Return Default Embbeded Resource  (Not Found)
            Logger.Add(LogLevel.Warning, "HTTP Server", $"Not Found Default. Embedded File.", requestUID);
            return null;
        }

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        /// <summary>Serve a Local File to the Response (Note: Relative to Application Directory).</summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="localFilePath">Local File Path (Note: Relative to App Directory).</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(bool *async) True if is served correctly.</returns>
        protected async Task<bool> ServeResponseLocalFile(HttpListenerResponse response, string localFilePath, string requestUID = "Unidentified")
        {
            try
            {
                Logger.Add(LogLevel.Info, "HTTP Server", $"Serving Local File: {localFilePath}.", requestUID);
                // Get file
                byte[] fileBytes = await File.ReadAllBytesAsync(localFilePath);
                // Add to HTTP Response
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = GetMimeType(Path.GetExtension(localFilePath));
                response.AddHeader("Date", DateTime.Now.ToString("r"));
                response.AddHeader("Last-Modified", File.GetLastWriteTime(localFilePath).ToString("r"));
                response.AddHeader("Cache-Control", "no-cache");
                response.ContentLength64 = fileBytes.Length;
                await response.OutputStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                Logger.Add(LogLevel.Success, "HTTP Server", $"Served Local File: {localFilePath}.", requestUID);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Error, "HTTP Server", $"Error serving Local File: {localFilePath}. Exception: {ex.Message}.", requestUID);
                return false;
            }
        }

        /// <summary>Serve a Embedded File to the Response (Note: Relative to root folder of project).</summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="embeddedFilePath">Embedded File Path (Note: Relative to root folder of project)</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(bool *async) True if is served correctly.</returns>
        protected async Task<bool> ServeReponseEmbeddedFile(HttpListenerResponse response, string embeddedFilePath, string requestUID = "Unidentified")
        {
            try
            {
                Logger.Add(LogLevel.Info, "HTTP Server", $"Serving Embedded File: {embeddedFilePath}.", requestUID);
                // Get the assembly (Application) and resoure file
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream? embeddedFile = assembly.GetManifestResourceStream(embeddedFilePath);
                byte[] filesBytes = new byte[embeddedFile.Length];
                MemoryStream memoryStream = new MemoryStream();
                await embeddedFile.CopyToAsync(memoryStream);
                filesBytes = memoryStream.ToArray();
                // Add to HTTP Response
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = GetMimeType(Path.GetExtension(embeddedFilePath));
                response.AddHeader("Date", DateTime.Now.ToString("r"));
                //response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(requestedPath).ToString("r"));
                response.AddHeader("Cache-Control", "no-cache");
                response.ContentLength64 = filesBytes.Length;
                await response.OutputStream.WriteAsync(filesBytes, 0, filesBytes.Length);
                Logger.Add(LogLevel.Success, "HTTP Server", $"Served Embedded File: {embeddedFilePath}.", requestUID);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Error, "HTTP Server", $"Error serving Embedded File: {embeddedFilePath}. Exception: {ex.Message}.", requestUID);
                return false;
            }
        }

        /// <summary>Serve Custom Content to the Response.</summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="requestedPath">Requested Path</param>
        /// <param name="content">Content to be in the response.</param>
        /// <param name="mimeType">Mime Type</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(bool *async) True if is served correctly.</returns>
        protected async Task<bool> ServeResponseCustomContent(HttpListenerResponse response, string requestedPath, string content, string mimeType = "text/html", string requestUID = "Unidentified")
        {
            try
            {
                Logger.Add(LogLevel.Info, "HTTP Server", $"Serving custom content: {requestedPath}.", requestUID);
                // Get content
                byte[] data = Encoding.UTF8.GetBytes(content);
                // Add to HTTP Response
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = mimeType;
                response.AddHeader("Date", DateTime.Now.ToString("r"));
                response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
                response.AddHeader("Cache-Control", "no-cache");
                response.ContentLength64 = data.LongLength;
                await response.OutputStream.WriteAsync(data, 0, data.Length);
                Logger.Add(LogLevel.Success, "HTTP Server", $"Served custom content: {requestedPath}.", requestUID);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Error, "HTTP Server", $"Error serving custom content: {requestedPath}. Exception: {ex.Message}.", requestUID);
                return false;
            }
        }

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        /// <summary>Get the Local File Path and Serve Reponse.</summary>
        /// <param name="requestedPath">Requested Path</param>
        /// <param name="basePathLocal">The base path to find the local file (Note: Relative to Application directory).</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(bool *async) True if is served correctly.</returns>
        protected async Task<bool> GetPathAndServeLocalFile(string requestedPath, string basePathLocal, HttpListenerResponse response, string requestUID = "Unidentified")
        {
            bool served = false;

            // Search local file 
            string? localFilePath = GetLocalFilePath(requestedPath, basePathLocal, requestUID);

            // Try to serve local file 
            if (!String.IsNullOrEmpty(localFilePath))
                served = await ServeResponseLocalFile(response, localFilePath, requestUID);

            // Return 
            return served;
        }

        /// <summary>Get the Embedded File and Serve Reponse.</summary>
        /// <param name="requestedPath">Requested Path</param>
        /// <param name="basePathEmbedded">The base path to find the embedded resource file (Note: Relative to root folder of project).</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(bool *async) True if is served correctly.</returns>
        protected async Task<bool> GetPathAndServeEmbeddedFile(string requestedPath, string basePathEmbedded, HttpListenerResponse response, string requestUID = "Unidentified")
        {
            bool served = false;

            // Search embedded file 
            string? embeddedFilePath = GetEmbeddedFilePath(requestedPath, basePathEmbedded, requestUID);

            // Try to serve embedded file 
            if (!String.IsNullOrEmpty(embeddedFilePath))
                served = await ServeReponseEmbeddedFile(response, embeddedFilePath, requestUID);

            // Return 
            return served;
        }

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        /// <summary>Serve Error 404 - Not Found.</summary>
        /// <param name="requestedPath">Requested Path</param>
        /// <param name="basePathEmbedded">The base path to find the embedded resource file (Note: Relative to root folder of project).</param>
        /// <param name="basePathLocal">The base path to find the local file (Note: Relative to Application directory).</param>/// 
        /// <param name="response">HTTP Response</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(bool *async) True if is served correctly.</returns>
        protected async Task<bool> ServeReponseErrorNotFound(string requestedPath, string basePathEmbedded, string basePathLocal, HttpListenerResponse response, string requestUID = "Unidentified")
        {
            try
            {
                // Try to serve embedded file 
                bool serveEmbedded = await GetPathAndServeEmbeddedFile("/error_404.html", basePathEmbedded, response, requestUID);
                if (serveEmbedded)
                    return true;
                // Try to serve local file 
                bool serveLocal = await GetPathAndServeLocalFile("/error_404.html", basePathLocal, response, requestUID);
                if (serveLocal) 
                    return true;
                // Serve plain text
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.ContentType = "text/plain";
                var errorMessage = "404 - Not Found";
                var errorBytes = Encoding.UTF8.GetBytes(errorMessage);
                response.ContentLength64 = errorBytes.Length;
                await response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                Logger.Add(LogLevel.Warning, "HTTP Server", $"Error 404. Path: <{requestedPath}>.", requestUID);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Error, "HTTP Server", $"Error on Server. Path: <{requestedPath}>. Exception: {ex.Message}.", requestUID);
                return false;
            }
        }

        /// <summary>Serve Error 505 - Server Error.</summary>
        /// <param name="requestedPath">Requested Path</param>
        /// <param name="basePathEmbedded">The base path to find the embedded resource file (Note: Relative to root folder of project).</param>
        /// <param name="basePathLocal">The base path to find the local file (Note: Relative to Application directory).</param>/// 
        /// <param name="response">HTTP Response</param>
        /// <param name="requestUID">Parameter requestUID is used for logging purposes to identify all task triggered by same request.</param>
        /// <returns>(bool *async) True if is served correctly.</returns>
        protected async Task<bool> ServeReponseErrorServer(string requestedPath, string basePathEmbedded, string basePathLocal, HttpListenerResponse response, string requestUID = "Unidentified")
        {
            try
            {
                // Try to serve embedded file 
                bool serveEmbedded = await GetPathAndServeEmbeddedFile("/error_500.html", basePathEmbedded, response, requestUID);
                if (serveEmbedded)
                    return true;
                // Try to serve local file 
                bool serveLocal = await GetPathAndServeLocalFile("/error_500.html", basePathLocal, response, requestUID);
                if (serveLocal)
                    return true;
                // Serve plain text
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "text/plain";
                var errorMessage = "500 - Internal Server Error";
                var errorBytes = Encoding.UTF8.GetBytes(errorMessage);
                response.ContentLength64 = errorBytes.Length;
                await response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                Logger.Add(LogLevel.Error, "HTTP Server", $"Error 500. Path: <{requestedPath}>.", requestUID);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Fatal, "HTTP Server", $"Error on Server. Path: <{requestedPath}>. Exception: {ex.Message}.", requestUID);
                return false;
            }
        }

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

    }
}