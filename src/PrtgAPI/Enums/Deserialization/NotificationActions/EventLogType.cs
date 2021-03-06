using System.Xml.Serialization;

namespace PrtgAPI
{
    /// <summary>
    /// Specifies types of event logs that can be generated by PRTG.
    /// </summary>
    public enum EventLogType
    {
        /// <summary>
        /// Error, indicating something has gone wrong.
        /// </summary>
        [XmlEnum("error")]
        Error,

        /// <summary>
        /// Warning, indicating a potential issue.
        /// </summary>
        [XmlEnum("warning")]
        Warning,

        /// <summary>
        /// Informational log for diagnostic purposes.
        /// </summary>
        [XmlEnum("information")]
        Information
    }
}
