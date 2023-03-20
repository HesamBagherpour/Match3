using MessagePack;

namespace Garage.Match3.Cells
{
    [MessagePackObject]
    [System.Serializable]
    public struct Point
    {
        [Key(0)] public int x;
        [Key(1)] public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
            hash = 0;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public bool Equals(Point other)
        {
            return other.x == x && other.y == y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point other)) return false;
            return Equals(other);
        }

        private int hash;

        public override int GetHashCode()
        {
            if (hash == 0)
                unchecked
                {
                    hash = $"{x}{y}".GetHashCode();
                }

            return hash;
        }

        public static bool operator ==(Point lhs, Point rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Point lhs, Point rhs)
        {
            return !(lhs == rhs);
        }

        public static Point None { get; } = new Point { x = -1, y = -1 };
    }
}