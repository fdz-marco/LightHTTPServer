using System.Diagnostics;
using System.Text.RegularExpressions;

namespace glitcher.core.Utils
{

    /// <summary>
    /// **System Utilities**
    /// </summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.16 - June 16, 2024
    /// </remarks>
    public static class SystemUtils
    {

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        /// <summary>Open the App Folder</summary>
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
                Logger.Add(LogLevel.Fatal, "System", $"Error opening App Folder: {ex.Message}.");
            }
        }

        /// <summary>Open the Web Browser wit a defined URL</summary>
        /// <returns>(void)</returns>
        public static void OpenWebBrowser(string URL)
        {
            if (!IsValidUrl(URL))
            {
                Logger.Add(LogLevel.Error, "System", $"Invalid URL: {URL}.");
                return;
            }
            try
            {
                Process.Start("explorer", URL);
            }
            catch (Exception ex)
            {
                Logger.Add(LogLevel.Error, "System", $"Error opening URL: {URL}. Error: {ex.Message}.");
            }
        }

        /// <summary>Validate the URL with RegEx</summary>
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

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
    }
}