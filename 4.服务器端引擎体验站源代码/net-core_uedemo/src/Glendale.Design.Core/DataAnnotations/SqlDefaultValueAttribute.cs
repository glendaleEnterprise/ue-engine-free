﻿using System;

namespace Glendale.Design.DataAnnotations
{
    /// <summary>
    /// Set a default value defined on the sql server
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class SqlDefaultValueAttribute : Attribute
    {
        /// <summary>
        /// Default value to apply
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Set a default value defined on the sql server
        /// </summary>
        /// <param name="value">Default value to apply</param>
        public SqlDefaultValueAttribute(string value)
        {
            DefaultValue = value;
        }
    }
}
