using System.Collections.Specialized;

namespace glitcher.core.Servers
{

    /// <summary>(Structure) Route</summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.16 - June 16, 2024
    /// </remarks>
    public struct Route<T>
    {
        public Func<NameValueCollection, T> callback;
        public string mimeType;
    }
}