// <copyright file="NativeMethods.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Contains native methods used by the T4 Toolbox.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="path"> 
        /// A pointer to a string that receives the relative path. This buffer is 
        /// assumed to be at least MAX_PATH characters in size. 
        /// </param>
        /// <param name="from">
        /// A pointer to a null-terminated string of maximum length MAX_PATH that 
        /// contains the path that defines the start of the relative path.
        /// </param>
        /// <param name="fromAttributes">
        /// The file attributes of <paramref name="from"/>. If this value contains FILE_ATTRIBUTE_DIRECTORY, 
        /// from is assumed to be a directory; otherwise, from is assumed to be a file.
        /// </param>
        /// <param name="to">
        /// A pointer to a null-terminated string of maximum length MAX_PATH that contains 
        /// the path that defines the endpoint of the relative path. 
        /// </param>
        /// <param name="toAttributes">
        /// The file attributes of <paramref name="to"/>. If this value contains FILE_ATTRIBUTE_DIRECTORY, 
        /// to is assumed to be directory; otherwise, to is assumed to be a file.
        /// </param>
        /// <returns>
        /// <c>true</c> if method succeeded and <c>false</c> if it failed.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        internal static extern bool PathRelativePathTo([Out] StringBuilder path, [In] string from, [In] uint fromAttributes, [In] string to, [In] uint toAttributes);
    }
}
