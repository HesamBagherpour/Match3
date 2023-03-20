using System;
using System.Collections.Generic;
using Garage.Match3;
using MessagePack;

namespace HB.Match3
{
    [Serializable]
    [MessagePackObject]
    public struct BlockType
    {
        public bool Equals(BlockType other)
        {
            return id == other.id && color == other.color;
        }

        public bool CanMatchWith(BlockType other)
        {
            return (id == other.id && color == other.color && id != BlockIDs.flower) ||
               (id == BlockIDs.Meter && color == other.color && id != BlockIDs.flower) ||
               (other.id == BlockIDs.Meter && color == other.color && id != BlockIDs.flower);
        }

        private int hash;

        public override int GetHashCode()
        {
            if (hash == 0)
            {
                unchecked
                {
                    int hashCode = id.GetHashCode();
                    hash = (hashCode * 397) ^ (int)color;
                }
            }
            return hash;
        }


        private sealed class IdColorEqualityComparer : IEqualityComparer<BlockType>
        {
            #region IEqualityComparer<BlockType> Interface

            public bool Equals(BlockType lhs, BlockType rhs)
            {
                return lhs.id == rhs.id && lhs.color == rhs.color;
            }

            public int GetHashCode(BlockType block)
            {
                unchecked
                {
                    return (block.id.GetHashCode() * 397) ^ (int)block.color;
                }
            }

            #endregion
        }

        public static bool operator ==(BlockType lhs, BlockType rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(BlockType lhs, BlockType rhs)
        {
            return !(lhs == rhs);
        }

        public static IEqualityComparer<BlockType> IdColorComparer { get; } = new IdColorEqualityComparer();
        [Key(0)] public string id;
        [Key(1)] public BlockColor color;


        public static BlockType None { get; } = new BlockType { color = BlockColor.None, id = "None", };

        public override string ToString()
        {
            if (color == BlockColor.None) return $"{id}";
            return $"{id}-{color.ToString().ToLower()}";
        }
    }
}