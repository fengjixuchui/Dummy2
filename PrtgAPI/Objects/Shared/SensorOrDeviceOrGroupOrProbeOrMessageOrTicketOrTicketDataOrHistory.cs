﻿using System.Xml.Serialization;
using PrtgAPI.Attributes;

namespace PrtgAPI.Objects.Shared
{
    /// <summary>
    /// Properties that apply to Sensors, Devices, Groups, Probes, Messages, Tickets, TicketData and History.
    /// </summary>
    public class SensorOrDeviceOrGroupOrProbeOrMessageOrTicketOrTicketDataOrHistory : ObjectTable
    {
        // ################################## Sensors, Devices, Groups, Probes, Messages, Tickets, TicketData, History ##################################

        /// <summary>
        /// Message or subject displayed on an object.
        /// </summary>
        [XmlElement("message_raw")]
        [PropertyParameter(nameof(Property.Message))]
        [PSVisible(true)]
        public string Message { get; set; }
    }
}