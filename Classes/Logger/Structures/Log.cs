namespace glitcher.core
{
    /// <summary>(Object Definition) Log (Entry)</summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.15 - June 15, 2024
    /// </remarks>
    public class Log
    {
        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Properties (Private / Public / Getters|Setters / Events)

        /// <summary>Log (Entry) Timestamp.</summary>
        public DateTime DateTime { get; set; } = DateTime.Now;
        /// <summary>Log (Entry) Severity Level.</summary>
        public LogLevel LogLevel { get; set; } = LogLevel.None;
        /// <summary>Log (Entry) Group.</summary>
        public string Group { get; set; } = string.Empty;
        /// <summary>Log (Entry) Message.</summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>Log (Entry) Whisper (More information).</summary>
        public string Whisper { get; set; } = string.Empty;
        /// <summary>Log (Entry) Function Caller.</summary>
        public string Caller { get; set; } = string.Empty;

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

    }
}