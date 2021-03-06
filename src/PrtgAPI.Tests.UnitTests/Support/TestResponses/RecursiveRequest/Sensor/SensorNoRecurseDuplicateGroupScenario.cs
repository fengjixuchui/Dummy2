using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrtgAPI.Tests.UnitTests.TreeNodes;

namespace PrtgAPI.Tests.UnitTests.Support.TestResponses
{
    class SensorNoRecurseDuplicateGroupScenario : GroupScenario
    {
        public SensorNoRecurseDuplicateGroupScenario()
        {
            probe = new ProbeNode("Local Probe",
                new GroupNode("Servers",
                    new DeviceNode("dc-1",
                        new SensorNode("Ping"),
                        new SensorNode("CPU Load")
                    ),
                    new DeviceNode("dc-2",
                        new SensorNode("Ping"),
                        new SensorNode("CPU Load")
                    ),
                    new GroupNode("Linux Servers",
                        new DeviceNode("arch-1",
                            new SensorNode("Ping"),
                            new SensorNode("HTTP")
                        )
                    )
                ),
                new GroupNode("Servers",
                    new DeviceNode("exch-1",
                        new SensorNode("Ping")
                    )
                )
            );
        }

        protected override IWebResponse GetResponse(string address, Content content)
        {
            switch (requestNum)
            {
                case 1: //Get all groups. We say there is only one group, named "Servers"
                    Assert.AreEqual(Content.Groups, content);
                    return new GroupResponse(probe.Groups.First(g => g.Name == "Servers").GetTestItem());

                case 2: //Check whether any other groups exist with the specified name
                    Assert.AreEqual(Content.Groups, content);
                    Assert.IsTrue(address.Contains("filter_name=Servers"));
                    return new GroupResponse(probe.Groups.Where(g => g.Name == "Servers").Select(g => g.GetTestItem()).ToArray());

                case 3: //Get all devices under the original group
                    Assert.AreEqual(Content.Devices, content);
                    Assert.IsTrue(address.Contains("filter_parentid=2000"));
                    return new DeviceResponse(probe.Groups.First(g => g.Name == "Servers").Devices.Select(d => d.GetTestItem()).ToArray());

                case 4: //Get all sensors under the devices of the original group
                    Assert.AreEqual(Content.Sensors, content);
                    Assert.IsTrue(address.Contains("filter_name=@sub()&filter_parentid=3000&filter_parentid=3001"));
                    return new SensorResponse(probe.Groups.First(g => g.Name == "Servers").Devices.SelectMany(d => d.Sensors).Select(s => s.GetTestItem()).ToArray());

                default:
                    throw UnknownRequest(address);
            }
        }
    }
}
