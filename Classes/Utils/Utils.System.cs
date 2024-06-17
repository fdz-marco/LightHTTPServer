using System.Diagnostics;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace glitcher.core
{
    /// <summary>
    /// (Class: Static~Global) **Utilities - System**<br/>
    /// </summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.17 - June 17, 2024
    /// </remarks>
    public static partial class Utils
    {

        /// <summary>
        /// Open the App Folder on Explorer
        /// </summary>
        /// <returns>(void)</returns>
        public static void OpenAppFolder()
        {
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            string appFolder = Path.GetDirectoryName(appPath);
            try
            {
                Process.Start("explorer", appFolder);
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Fatal, "Utilities", $"Error opening App Folder: {ex.Message}.");
            }
        }

        /// <summary>
        /// Open a URL on the Web Browser
        /// </summary>
        /// <param name="url">URL. Example: http:// localhost:8081 </param>
        /// <returns>(void)</returns>
        public static void OpenWebBrowser(string url)
        {
            if (!IsValidUrl(url))
            {
                Logger.Add(LogLevel.Error, "Utilities", $"Invalid URL: {url}.");
                return;
            }
            try
            {
                Process.Start("explorer", url);
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Error, "Utilities", $"Error opening URL: {url}. Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Validate the URL with RegEx
        /// </summary>
        /// <param name="url">URL. Example: http:// localhost:8081 </param>
        /// <returns>(void)</returns>
        public static bool IsValidUrl(string url)
        {
            string pattern = @"^(https?|ftp):\/\/" +                    // Scheme
                             @"(([a-zA-Z0-9\-_]+\.)+[a-zA-Z]{2,}|" +    // Domain name
                             @"localhost|" +                            // Localhost
                             @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})" +   // OR IPv4
                             @"(:\d+)?(\/.*)?$";                        // Optional port and path
            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Check if application has admin rights.
        /// </summary>
        /// <returns>(bool) True if application has been open using admin rights.</returns>
        public static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Force restart application with admin rights if needed.
        /// </summary>
        /// <returns>(void)</returns>
        public static void RestartAsAdmin()
        {
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Process.GetCurrentProcess().MainModule.FileName;

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Logger.Add(LogLevel.Info, "Utilities", $"Error trying to open the application as an administrator!. Error: {ex.Message}.");
                }
            }
        }

    }
}
