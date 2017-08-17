﻿using System;
using System.Linq;
using System.Web.Http.Filters;

namespace RichardLawley.WebApi.OrderedFilters
{
    /// <summary>
    /// Our version of FilterInfo, with the ability to sort by an Order attribute.  This cannot simply inherit from
    /// FilterInfo in the Web API class because it's sealed :(
    /// </summary>
    public class OrderedFilterInfo : IComparable
    {
        public OrderedFilterInfo(IFilter instance, FilterScope scope)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            Scope = scope;
        }

        /// <summary>
        /// Filter this instance is about
        /// </summary>
        public IFilter Instance { get; private set; }

        /// <summary>
        /// Scope of this filter
        /// </summary>
        public FilterScope Scope { get; private set; }

        /// <summary>
        /// Allows controlled ordering of filters
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is OrderedFilterInfo)
            {
                var otherfilterInfo = obj as OrderedFilterInfo;

                // Global filters should be executed before Controller and Action Filters.  We don't strictly have to
                // do this, since it's done again in the framework, but it's a little more consistent for testing!
                if (this.Scope == FilterScope.Global && otherfilterInfo.Scope != FilterScope.Global)
                {
                    return -10;
                }
                else if (this.Scope != FilterScope.Global && otherfilterInfo.Scope == FilterScope.Global)
                {
                    return 10;
                }

                var thisAttribute = Instance as IOrderedFilterAttribute;
                var otherAttribute = otherfilterInfo.Instance as IOrderedFilterAttribute;
                var thisNonOrderedAttribute = this.Instance as IFilter;
                var otherNonOrderedAttribute = otherfilterInfo.Instance as IFilter;

                if (thisAttribute != null && otherAttribute != null)
                {
                    int value = thisAttribute.Order.CompareTo(otherAttribute.Order);
                    if (value == 0)
                    {
                        // If they both have the same order, sort by name instead
                        value = string.Compare(thisAttribute.GetType().FullName, otherAttribute.GetType().FullName, StringComparison.Ordinal);
                    }

                    return value;
                }
                else if (thisNonOrderedAttribute != null && otherAttribute != null)
                {
                    return 1;
                }
                else if (thisAttribute != null && otherNonOrderedAttribute != null)
                {
                    return -1;
                }
                {
                    return string.Compare(thisNonOrderedAttribute.GetType().FullName, otherNonOrderedAttribute.GetType().FullName, StringComparison.Ordinal);
                }
            }
            else
            {
                throw new ArgumentException("Object is of the wrong type");
            }
        }

        /// <summary>
        /// Converts this to a FilterInfo (because FilterInfo is sealed, and we can't extend it. /sigh
        /// </summary>
        /// <returns></returns>
        public FilterInfo ConvertToFilterInfo()
        {
            return new FilterInfo(Instance, Scope);
        }
    }
}