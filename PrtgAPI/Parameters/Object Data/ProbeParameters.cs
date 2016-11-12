﻿using System.Linq;

namespace PrtgAPI.Parameters
{
    /// <summary>
    /// Represents parameters used to construct a <see cref="T:PrtgAPI.PrtgUrl"/> for retrieving <see cref="T:PrtgAPI.Probe"/> objects.
    /// </summary>
    public class ProbeParameters : TableParameters<Probe>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PrtgAPI.Parameters.ProbeParameters"/> class.
        /// </summary>
        public ProbeParameters() : base(Content.Objects)
        {
            base.SearchFilter = DefaultSearchFilter();
        }

        private SearchFilter[] DefaultSearchFilter()
        {
            return new[] {new SearchFilter(Property.ParentId, "0")};
        }

        /// <summary>
        /// Filter objects to those with a <see cref="T:PrtgAPI.Property"/> of a certain value. Specify multiple filters to limit results further.
        /// </summary>
        public override SearchFilter[] SearchFilter
        {
            get { return base.SearchFilter; }
            set
            {
                if (!value.Any(item => item.Property == Property.ParentId && item.Operator == FilterOperator.Equals && item.Value.ToString() == "0"))
                    value = value.Concat(DefaultSearchFilter()).ToArray();

                base.SearchFilter = value;
            }
        }
    }
}