using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCLStorage
{
    /// <summary>
    /// Specifies whether a file should be opened for write access or not
    /// </summary>
    public enum FileAccess
    {
        /// <summary>
        /// Specifies that a file should be opened for read-only access
        /// </summary>
        Read,
        /// <summary>
        /// Specifies that a file should be opened for read/write access
        /// </summary>
        ReadAndWrite
    }

    /// <summary>
    /// Represents a file
    /// </summary>
    public interface IFile: IFileSystemItem
    {
        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="destFolderPath">The full new path of the file.</param>
        /// <param name="desiredNewName">The desired new name of file</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task MoveAsync(string destFolderPath, string desiredNewName = "", NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a file.
        /// </summary>
        /// <param name="destFolderPath">The full new path of the file.</param>
        /// <param name="desiredNewName">The desired new name of file</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task CopyAsync(string destFolderPath, string desiredNewName = "", NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Appends text to file.
        /// </summary>
        /// <param name="text">The text which should be appended</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the text is appended.</returns>
        Task AppendTextAsync(string text, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Reads the contents of the specified file and returns a buffer.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>When this method completes, it returns byte array that represents the contents of the file.</returns>
        Task<byte[]> ReadBufferAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
