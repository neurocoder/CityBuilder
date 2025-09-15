using System;

namespace CityBuilder.Domain.Entities
{
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y) { X = x; Y = y; }

        public override string ToString() => $"{X},{Y}";
        public bool Equals(GridPosition other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is GridPosition p && Equals(p);
        public override int GetHashCode() => (X, Y).GetHashCode();
        public static bool operator ==(GridPosition a, GridPosition b) => a.Equals(b);
        public static bool operator !=(GridPosition a, GridPosition b) => !a.Equals(b);
    }
}
