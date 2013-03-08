﻿using System;
using System.Linq;

namespace RichardLawley.WebApi.OrderedFilters
{
    /// <summary>
    /// Allows ordering of filter attributes
    /// </summary>
    public interface IOrderedFilterAttribute
    {
        /// <summary>
        /// Order of execution for this filter
        /// </summary>
        int Order { get; set; }
    }
}