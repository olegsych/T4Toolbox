// <copyright file="HashCode.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.TemplateAnalysis
{
    internal static class HashCode
    {
        internal static int Combine(int hashCode1, int hashCode2)
        {
            return (hashCode1 << 5) + hashCode1 ^ hashCode2;            
        }
    }
}