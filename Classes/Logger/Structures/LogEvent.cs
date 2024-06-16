namespace glitcher.core
{
    /// <summary>(Object Definition) Log Event (EventArgs)</summary>
    /// <remarks>
    /// Author: Marco Fernandez (marcofdz.com / glitcher.dev)<br/>
    /// Last modified: 2024.06.15 - June 15, 2024
    /// </remarks>
    public class LogEvent : EventArgs
    {
        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Properties (Private / Public / Getters|Setters / Events)

        public string? eventType { get; } = null;
        public dynamic? varValue { get; } = null;

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~

        #region Constructor / Settings / Initialization Tasks

        /// <summary>Class constructor.</summary>
        public LogEvent(string eventType, dynamic? varValue = null)
        {
            this.eventType = eventType;
            this.varValue = varValue;
        }

        #endregion

        // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
    }
}