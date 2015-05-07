// <copyright file="Position.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.TemplateAnalysis
{
    using System;
    using System.Globalization;

    internal struct Position : IEquatable<Position>
    {
        private readonly int line;
        private readonly int column;

        public Position(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        public int Line
        {
            get { return this.line; }
        }

        public int Column
        {
            get { return this.column; }
        }

        public bool Equals(Position other)
        {
            return this.Line == other.Line && this.Column == other.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Line.GetHashCode(), this.Column.GetHashCode());
        }

        public override bool Equals(object other)
        {
            return (other is Position) && this.Equals((Position)other);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0},{1})", this.Line, this.Column);
        }
    }
}