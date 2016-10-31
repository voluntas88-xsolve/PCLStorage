using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace PCLStorage
{
    /// <summary>
    /// Represents a file in the <see cref="WinRTFileSystem"/>
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class WinRTFile : IFile
    {
        /// <summary>
        /// The HRESULT on a System.Exception thrown when a file collision occurs.
        /// </summary>
        internal const int FILE_ALREADY_EXISTS = unchecked((int)0x800700B7);

        private IStorageFile _wrappedFile;

        public bool Equals(IFileSystemItem other)
        {
            return this.Path == other.Path;
        }

        /// <summary>
        /// Creates a new <see cref="WinRTFile"/>
        /// </summary>
        /// <param name="wrappedFile">The WinRT <see cref="IStorageFile"/> to wrap</param>
        public WinRTFile(IStorageFile wrappedFile)
        {
            _wrappedFile = wrappedFile;
        }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name
        {
            get { return _wrappedFile.Name; }
        }

        /// <summary>
        /// The "full path" of the file, which should uniquely identify it within a given <see cref="IFileSystem"/>
        /// </summary>
        public string Path
        {
            get { return _wrappedFile.Path; }
        }

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            FileAccessMode fileAccessMode;
            if (fileAccess == FileAccess.Read)
            {
                fileAccessMode = FileAccessMode.Read;
            }
            else if (fileAccess == FileAccess.ReadAndWrite)
            {
                fileAccessMode = FileAccessMode.ReadWrite;
            }
            else
            {
                throw new ArgumentException("Unrecognized FileAccess value: " + fileAccess);
            }

            var wrtStream = await _wrappedFile.OpenAsync(fileAccessMode).AsTask(cancellationToken).ConfigureAwait(false);
            return wrtStream.AsStream();
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>A task which will complete after the file is deleted.</returns>
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _wrappedFile.DeleteAsync().AsTask(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Renames a file without changing its location.
        /// </summary>
        /// <param name="newName">The new leaf name of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is renamed.
        /// </returns>
        public async Task RenameAsync(string newName, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newName, "newName");

            try
            {
                await _wrappedFile.RenameAsync(newName, (Windows.Storage.NameCollisionOption)collisionOption).AsTask(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == FILE_ALREADY_EXISTS)
                {
                    throw new IOException("File already exists.", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="destFolderPath">The full new path of the file.</param>
        /// <param name="desiredNewName">The desired new name of file</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        public async Task MoveAsync(string destFolderPath, string desiredNewName = "", NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNullOrEmpty(destFolderPath, "newPath");

            var newFolder = await StorageFolder.GetFolderFromPathAsync(destFolderPath).AsTask(cancellationToken).ConfigureAwait(false);
            string newName = desiredNewName.Length > 0 ? desiredNewName : _wrappedFile.Name;

            try
            {
                await _wrappedFile.MoveAsync(newFolder, newName, (Windows.Storage.NameCollisionOption)collisionOption).AsTask(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == FILE_ALREADY_EXISTS)
                {
                    throw new IOException("File already exists.", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Copy a file.
        /// </summary>
        /// <param name="destFolderPath">The full new path of the file.</param>
        /// <param name="desiredNewName">The desired new name of file</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        public async Task CopyAsync(string destFolderPath, string desiredNewName = "", NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNullOrEmpty(destFolderPath, "newPath");

            var newFolder = await StorageFolder.GetFolderFromPathAsync(destFolderPath).AsTask(cancellationToken).ConfigureAwait(false);
            string newName = desiredNewName.Length > 0 ? desiredNewName : _wrappedFile.Name;

            try
            {
                _wrappedFile = await _wrappedFile.CopyAsync(newFolder, newName, (Windows.Storage.NameCollisionOption)collisionOption).AsTask(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == FILE_ALREADY_EXISTS)
                {
                    throw new IOException("File already exists.", ex);
                }

                throw;
            }
        }

        public async Task AppendTextAsync(string text, CancellationToken cancellationToken)
        {
            using (var stream = await OpenAsync(FileAccess.ReadAndWrite, cancellationToken).ConfigureAwait(false))
            {
                var bytes = Encoding.UTF8.GetBytes(text.ToCharArray());
                stream.Seek(0, SeekOrigin.End);
                await stream.WriteAsync(bytes, 0, bytes.Length);

            }
        }

        public Task<IFolder> GetParentAsync()
        {
            return FileSystem.Current.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(Path));
        }
    }
}
