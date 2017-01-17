using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCLStorage
{
    /// <summary>
	/// Provides access to the basic properties, like the size of the item or the date the item was last modified.
	/// </summary>
    public interface IBasicProperties
    {
        /// <summary>
		/// Gets the timestamp of the last time the file was modified.
		/// </summary>
		/// <value>The timestamp.</value>
        DateTimeOffset DateModified { get; }

        /// <summary>
		/// Gets the size of the file in bytes.
		/// </summary>
		/// <value>The size of the file in bytes.</value>
        ulong Size { get; }
    }
}
