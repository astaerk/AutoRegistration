using System;
using System.Collections.Generic;
using Unity.Lifetime;

namespace Unity.AutoRegistration
{
    /// <summary>
    /// Registration options contract describes parameters of type registration operation
    /// </summary>
    public interface IRegistrationOptions
    {
        /// <summary>
        /// Gets or sets lifetime manager to use to register type(s).
        /// </summary>
        /// <value>Lifetime manager.</value>
        ITypeLifetimeManager LifetimeManager { get; set; }

        /// <summary>
        /// Gets or sets name to register type(s) with.
        /// </summary>
        /// <value>Name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets interfaces to register type(s) as.
        /// </summary>
        /// <value>Interfaces.</value>
        IEnumerable<Type> Interfaces { get; set; }

        /// <summary>
        /// Sets type being registered.
        /// </summary>
        /// <value>Target type.</value>
        Type Type { set; }
    }
}