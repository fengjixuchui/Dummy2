/******************************************************************************************
 * This code was generated by a tool.                                                     *
 * Please do not modify this file directly - modify TreeBuilderLevel.Generated.tt instead *
 ******************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrtgAPI.Tree.Internal;

//Methods with complex logic surrounding sync/async function calls.
//For each method, two variants a generated. A synchronous method with the
//expected return type, and an async method that implicitly wraps the result
//in a Task

namespace PrtgAPI.Tree.Converters.Tree
{
    internal partial class TreeBuilderLevel
    {
        //######################################
        // GetDeviceChildren
        //######################################

        private List<PrtgOrphan> GetDeviceChildren(IDevice device)
        {
            if (device.TotalSensors > 0 && Options.Contains(TreeParseOption.Sensors))
            {
                Token.ThrowIfCancellationRequested();

                var sensors = GetOrphans(ObjectManager.Sensor);

                return sensors;
            }

            return new List<PrtgOrphan>();
        }

        private async Task<List<PrtgOrphan>> GetDeviceChildrenAsync(IDevice device)
        {
            if (device.TotalSensors > 0 && Options.Contains(TreeParseOption.Sensors))
            {
                Token.ThrowIfCancellationRequested();

                var sensors = await GetOrphansAsync(ObjectManager.Sensor).ConfigureAwait(false);

                return sensors;
            }

            return new List<PrtgOrphan>();
        }

        //######################################
        // GetContainerChildren
        //######################################

        private List<PrtgOrphan> GetContainerChildren(IGroupOrProbe parent)
        {
            List<ObjectFactory> factories = new List<ObjectFactory>();

            if (parent.Id == WellKnownId.Root && Options.Contains(TreeParseOption.Probes))
                factories.Add(ObjectManager.Probe);
            else
            {
                if (parent.TotalDevices > 0 && Options.Contains(TreeParseOption.Devices))
                    factories.Add(ObjectManager.Device);

                if (parent.TotalGroups > 0 && Options.Contains(TreeParseOption.Groups))
                    factories.Add(ObjectManager.Group);
            }

            var results = GetOrphans(factories.ToArray());

            return results;
        }

        private async Task<List<PrtgOrphan>> GetContainerChildrenAsync(IGroupOrProbe parent)
        {
            List<ObjectFactory> factories = new List<ObjectFactory>();

            if (parent.Id == WellKnownId.Root && Options.Contains(TreeParseOption.Probes))
                factories.Add(ObjectManager.Probe);
            else
            {
                if (parent.TotalDevices > 0 && Options.Contains(TreeParseOption.Devices))
                    factories.Add(ObjectManager.Device);

                if (parent.TotalGroups > 0 && Options.Contains(TreeParseOption.Groups))
                    factories.Add(ObjectManager.Group);
            }

            var results = await GetOrphansAsync(factories.ToArray()).ConfigureAwait(false);

            return results;
        }

        //######################################
        // GetTriggers
        //######################################

        private TriggerOrphanCollection GetTriggers()
        {
            var obj = Value as SensorOrDeviceOrGroupOrProbe;

            if (obj != null && obj.NotificationTypes.TotalTriggers > 0 && Options.Contains(TreeParseOption.Triggers))
            {
                Token.ThrowIfCancellationRequested();

                var triggers = ObjectManager.Trigger.Objects(Value.Id.Value);

                if (triggers.Count == 0)
                    return null;

                var orphans = triggers.Select(t => ObjectManager.Trigger.Orphan(t, null)).Cast<TriggerOrphan>();

                return PrtgOrphan.TriggerCollection(orphans);
            }

            return null;
        }

        private async Task<TriggerOrphanCollection> GetTriggersAsync()
        {
            var obj = Value as SensorOrDeviceOrGroupOrProbe;

            if (obj != null && obj.NotificationTypes.TotalTriggers > 0 && Options.Contains(TreeParseOption.Triggers))
            {
                Token.ThrowIfCancellationRequested();

                var triggers = await ObjectManager.Trigger.ObjectsAsync(Value.Id.Value, Token).ConfigureAwait(false);

                if (triggers.Count == 0)
                    return null;

                var orphans = triggers.Select(t => ObjectManager.Trigger.Orphan(t, null)).Cast<TriggerOrphan>();

                return PrtgOrphan.TriggerCollection(orphans);
            }

            return null;
        }

        //######################################
        // GetProperties
        //######################################

        private PropertyOrphanCollection GetProperties()
        {
            var obj = Value as SensorOrDeviceOrGroupOrProbe;

            if (obj != null && Options.Contains(TreeParseOption.Properties))
            {
                Token.ThrowIfCancellationRequested();

                var properties = ObjectManager.Property.Objects(obj.Id);
                var orphans = properties.Select(p => ObjectManager.Property.Orphan(p, null)).Cast<PropertyOrphan>();

                return PrtgOrphan.PropertyCollection(orphans);
            }

            return null;
        }

        private async Task<PropertyOrphanCollection> GetPropertiesAsync()
        {
            var obj = Value as SensorOrDeviceOrGroupOrProbe;

            if (obj != null && Options.Contains(TreeParseOption.Properties))
            {
                Token.ThrowIfCancellationRequested();

                var properties = await ObjectManager.Property.ObjectsAsync(obj.Id, Token).ConfigureAwait(false);
                var orphans = properties.Select(p => ObjectManager.Property.Orphan(p, null)).Cast<PropertyOrphan>();

                return PrtgOrphan.PropertyCollection(orphans);
            }

            return null;
        }

        //######################################
        // GetOrphans
        //######################################

        /// <summary>
        /// Retrieves <see cref="ITreeValue"/> values objects from a set of object factories, retrieves the children
        /// of each object from the next <see cref="TreeBuilderLevel"/> and encapsulates the object and its children in a <see cref="PrtgOrphan"/>.
        /// </summary>
        /// <param name="factories">The factories to retrieve objects from.</param>
        /// <returns>A list of <see cref="PrtgOrphan"/> objects encapsulating the values returnd from the factories and their respective children.</returns>
        private List<PrtgOrphan> GetOrphans(params ObjectFactory[] factories)
        {
            List<Tuple<ITreeValue, ObjectFactory>> results = new List<Tuple<ITreeValue, ObjectFactory>>();

            foreach (var factory in factories)
            {
                Token.ThrowIfCancellationRequested();

                var objs = factory.Objects(Value.Id.Value);

                results.AddRange(objs.Select(o => Tuple.Create(o, factory)));
            }

            ProgressManager.OnLevelWidthKnown(Value, ValueType, results.Count);

            var orphans = new List<PrtgOrphan>();

            foreach (var item in results)
            {
                ProgressManager.OnProcessValue(item.Item1);

                var level = new TreeBuilderLevel(item.Item1, item.Item2.Type, item.Item2.Orphan, builder);

                orphans.Add(level.ProcessObject());
            }

            return orphans;
        }

        /// <summary>
        /// Retrieves <see cref="ITreeValue"/> values objects from a set of object factories, retrieves the children
        /// of each object from the next <see cref="TreeBuilderLevel"/> and encapsulates the object and its children in a <see cref="PrtgOrphan"/>.
        /// </summary>
        /// <param name="factories">The factories to retrieve objects from.</param>
        /// <returns>A list of <see cref="PrtgOrphan"/> objects encapsulating the values returnd from the factories and their respective children.</returns>
        private async Task<List<PrtgOrphan>> GetOrphansAsync(params ObjectFactory[] factories)
        {
            List<Tuple<ITreeValue, ObjectFactory>> results = new List<Tuple<ITreeValue, ObjectFactory>>();

            foreach (var factory in factories)
            {
                Token.ThrowIfCancellationRequested();

                var objs = await factory.ObjectsAsync(Value.Id.Value, Token).ConfigureAwait(false);

                results.AddRange(objs.Select(o => Tuple.Create(o, factory)));
            }

            ProgressManager.OnLevelWidthKnown(Value, ValueType, results.Count);

            var orphans = new List<PrtgOrphan>();

            foreach (var item in results)
            {
                ProgressManager.OnProcessValue(item.Item1);

                var level = new TreeBuilderLevel(item.Item1, item.Item2.Type, item.Item2.Orphan, builder);

                orphans.Add(await level.ProcessObjectAsync().ConfigureAwait(false));
            }

            return orphans;
        }
    }
}
