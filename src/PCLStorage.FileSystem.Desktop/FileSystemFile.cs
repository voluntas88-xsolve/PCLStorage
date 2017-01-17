﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCLStorage
{
    /// <summary>
    /// Represents a file in the <see cref="DesktopFileSystem"/>
    /// </summary>
    [DebuggerDisplay("Name = {_name}")]
    public class FileSystemFile : IFile
    {
        private string _name;
        private string _path;

        public bool Equals(IFileSystemItem other)
        {
            return this.Path == other.Path;
        }

        /// <summary>
        /// Creates a new <see cref="FileSystemFile"/> corresponding to the specified path
        /// </summary>
        /// <param name="path">The file path</param>
        public FileSystemFile(string path)
        {
            _name = System.IO.Path.GetFileName(path);
            _path = path;
        }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The "full path" of the file, which should uniquely identify it within a given <see cref="IFileSystem"/>
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (fileAccess == FileAccess.Read)
            {
                return File.OpenRead(Path);
            }
            else if (fileAccess == FileAccess.ReadAndWrite)
            {
                return File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite);
            }
            else
            {
                throw new ArgumentException("Unrecognized FileAccess value: " + fileAccess);
            }
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>A task which will complete after the file is deleted.</returns>
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (!File.Exists(Path))
            {
                throw new PCLStorage.Exceptions.FileNotFoundException("File does not exist: " + Path);
            }
            
            File.Delete(Path);
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

            await MoveAsync(System.IO.Path.GetDirectoryName(_path), newName, collisionOption, cancellationToken);
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

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string newDirectory = destFolderPath;
            string newName = desiredNewName.Length > 0 ? desiredNewName : _name;

            for (int counter = 1; ; counter++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string candidateName = newName;
                if (counter > 1)
                {
                    candidateName = String.Format(
                        CultureInfo.InvariantCulture,
                        "{0} ({1}){2}",
                        System.IO.Path.GetFileNameWithoutExtension(newName),
                        counter,
                        System.IO.Path.GetExtension(newName));
                }

                string candidatePath = PortablePath.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case NameCollisionOption.FailIfExists:
                            throw new IOException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            File.Delete(candidatePath);
                            break;
                    }
                }

                File.Move(_path, candidatePath);
                _path = candidatePath;
                _name = candidateName;
                return;
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

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string newDirectory = destFolderPath;
            string newName = desiredNewName.Length>0 ? desiredNewName:_name;

            for (int counter = 1; ; counter++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string candidateName = newName;
                if (counter > 1)
                {
                    candidateName = String.Format(
                        CultureInfo.InvariantCulture,
                        "{0} ({1}){2}",
                        System.IO.Path.GetFileNameWithoutExtension(newName),
                        counter,
                        System.IO.Path.GetExtension(newName));
                }

                string candidatePath = PortablePath.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case NameCollisionOption.FailIfExists:
                            throw new IOException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            File.Delete(candidatePath);
                            break;
                    }
                }

                File.Copy(_path, candidatePath);
                _path = candidatePath;
                _name = candidateName;
                return;
            }
        }

        public async Task AppendTextAsync(string text, CancellationToken cancellationToken)
        {
            using (StreamWriter sw = File.AppendText(_path))
            {
                sw.WriteLine(text);
            }
        }

        public Task<IFolder> GetParentAsync()
        {
            return FileSystem.Current.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(Path));
        }

        /// <summary>
		/// Gets the basic properties of the current folder.
		/// </summary>
		/// <returns>When this method completes successfully, it returns the basic properties of the current file as a IFileExtraProperties object.</returns>
		public async Task<IBasicProperties> GetBasicPropertiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var info = new FileInfo(Path);

            return new BasicProperties(info.LastWriteTime, (ulong)info.Length);
        }

        /// <summary>
        /// Reads the contents of the specified file and returns a buffer.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>When this method completes, it returns byte array that represents the contents of the file.</returns>
        public async Task<byte[]> ReadBufferAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (Stream stream = await OpenAsync(FileAccess.ReadAndWrite, cancellationToken))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Write(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
