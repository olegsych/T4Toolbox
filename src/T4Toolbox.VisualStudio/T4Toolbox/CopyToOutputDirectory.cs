// <copyright file="CopyToOutputDirectory.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox
{
    /// <summary>
    /// Defines if and when a file is copied to the output directory.
    /// </summary>
    public enum CopyToOutputDirectory
    {
        /// <summary>
        /// Do not copy this file to the output directory.
        /// </summary>
        DoNotCopy = 0,
        
        /// <summary>
        /// Always copy this file to the output directory.
        /// </summary>
        CopyAlways = 1,
       
        /// <summary>
        /// Copy this file to the output directory only if it is 
        /// newer than the same file in the output directory.
        /// </summary>
        CopyIfNewer = 2,
    }
}
