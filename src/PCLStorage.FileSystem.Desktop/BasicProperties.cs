using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLStorage
{
    /// <summary>
	/// Provides access to the basic properties, like the size of the item or the date the item was last modified.
	/// </summary>
    public class BasicProperties : IBasicProperties
    {

        /// <summary>
		/// Initializes a new instance of the <see cref="T:PCLStorage.ExtraProperties"/> class.
		/// </summary>
		/// <param name="dateModified">Date modified.</param>
        /// <param name="size">Size in bytes.</param>
		public BasicProperties(DateTimeOffset dateModified, ulong size)
        {
            this.DateModified = dateModified;
            this.Size = size;
        }

        /// <summary>
        /// Gets the timestamp of the last time the file was modified.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTimeOffset DateModified { get; private set; }

        /// <summary>
		/// Gets the size of the file in bytes.
		/// </summary>
		/// <value>The size of the file in bytes.</value>
        public ulong Size { get; private set; }
    }
}
