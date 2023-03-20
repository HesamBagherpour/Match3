using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New IronWallTile", menuName = "Tiles/IronWall Tile")]
    public class IronWallTile : RestrictionTile
    {
        public Vector3 offsetPosition = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector2 scale = Vector2.one;
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = sprite;
            tileData.flags = TileFlags.None;
            // Debug.Log($"Tile offset at {position}: {offsetPosition}");
            Quaternion q = Quaternion.Euler(0, 0, rotation.z);
            tileData.transform = Matrix4x4.TRS(offsetPosition, q, scale);
            // tileData.transform.SetTRS(offsetPosition, q, scale);
            // RefreshTile(position, tilemap);
        }
    }
}