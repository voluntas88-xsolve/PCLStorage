﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace PCLStorage
{
    /// <summary>
    /// Specifies what should happen when trying to create a file or folder that already exists.
    /// </summary>
    public enum CreationCollisionOption
    {
        /// <summary>
        /// Creates a new file with a unique name of the form "name (2).txt"
        /// </summary>
        GenerateUniqueName = 0,
        /// <summary>
        /// Replaces any existing file with a new (empty) one
        /// </summary>
        ReplaceExisting = 1,
        /// <summary>
        /// Throws an exception if the file exists
        /// </summary>
        FailIfExists = 2,
        /// <summary>
        /// Opens the existing file, if any
        /// </summary>
        OpenIfExists = 3,
    }

    /// <summary>
    /// Represents a file system folder
    /// </summary>
    public interface IFolder: IFileSystemItem
    {

        /// <summary>
        /// Creates a file in this folder
        /// </summary>
        /// <param name="desiredName">The name of the file to create</param>
        /// <param name="option">Specifies how to behave if the specified file already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created file</returns>
        Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a file in this folder
        /// </summary>
        /// <param name="name">The name of the file to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested file, or null if it does not exist</returns>
        Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of the files in this folder
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of the files in the folder</returns>
        Task<IList<IFile>> GetFilesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a subfolder in this folder
        /// </summary>
        /// <param name="desiredName">The name of the folder to create</param>
        /// <param name="option">Specifies how to behave if the specified folder already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created folder</returns>
        Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a subfolder in this folder
        /// </summary>
        /// <param name="name">The name of the folder to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested folder, or null if it does not exist</returns>
        Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of subfolders in this folder
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of subfolders in the folder</returns>
        Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks whether a folder or file exists at the given location.
        /// </summary>
        /// <param name="name">The name of the file or folder to check for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task whose result is the result of the existence check.</returns>
        Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="FilePath">Specifies file path.</param>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        Task<Stream> OpenFileAsync(string FilePath, FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all folder's items.
        /// </summary>
        /// <returns>A <see cref="IList"/> which contains all folders items (folder and files)</returns>
        Task<IList<IFileSystemItem>> GetItemsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
