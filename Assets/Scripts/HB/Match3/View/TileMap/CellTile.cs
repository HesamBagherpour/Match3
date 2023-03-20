using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace HB.Match3.View.TileMap
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Cell Tile", menuName = "Tiles/Cell Tile")]
    public class CellTile : TileBase
    {
        #region Public Fields

        [FormerlySerializedAs("m_Sprites")]
        [SerializeField]
        public Sprite[] mSprites;

        #endregion

        #region Unity

        private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;

            int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

            byte original = (byte)mask;
            if ((original | 254) < 255) mask &= 125;
            if ((original | 251) < 255) mask &= 245;
            if ((original | 239) < 255) mask &= 215;
            if ((original | 191) < 255) mask &= 95;

            int index = GetIndex((byte)mask);
            if (index == 14)
            {
                if ((location.y & 1) == 1)
                {
                    if ((location.x & 1) == 1)
                    {
                        index = 15;
                    }

                }
                else
                {
                    if ((location.x & 1) == 0)
                    {
                        index = 15;
                    }
                }
            }

            if (index >= 0 && index < mSprites.Length && TileValue(tileMap, location))
            {
                tileData.sprite = mSprites[index];
                tileData.transform = GetTransform((byte)mask);
                tileData.color = Color.white;
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

        #endregion

        #region Public Methods

        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            for (int yd = -1; yd <= 1; yd++)
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                    if (TileValue(tileMap, position))
                        tileMap.RefreshTile(position);
                }
        }

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            UpdateTile(location, tileMap, ref tileData);
        }

        #endregion

        #region Private Methods

        private bool TileValue(ITilemap tileMap, Vector3Int position)
        {
            TileBase tile = tileMap.GetTile(position);
            return tile != null && tile == this;
        }

        private int GetIndex(byte mask)
        {
            switch (mask)
            {
                case 0: return 0;
                case 1:
                case 4:
                case 16:
                case 64: return 1;
                case 5:
                case 20:
                case 80:
                case 65: return 2;
                case 7:
                case 28:
                case 112:
                case 193: return 3;
                case 17:
                case 68: return 4;
                case 21:
                case 84:
                case 81:
                case 69: return 5;
                case 23:
                case 92:
                case 113:
                case 197: return 6;
                case 29:
                case 116:
                case 209:
                case 71: return 7;
                case 31:
                case 124:
                case 241:
                case 199: return 8;
                case 85: return 9;
                case 87:
                case 93:
                case 117:
                case 213: return 10;
                case 95:
                case 125:
                case 245:
                case 215: return 11;
                case 119:
                case 221: return 12;
                case 127:
                case 253:
                case 247:
                case 223: return 13;
                case 255: return 14;
            }

            return -1;
        }

        private Matrix4x4 GetTransform(byte mask)
        {
            switch (mask)
            {
                case 4:
                case 20:
                case 28:
                case 68:
                case 84:
                case 92:
                case 116:
                case 124:
                case 93:
                case 125:
                case 221:
                case 253:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
                case 16:
                case 80:
                case 112:
                case 81:
                case 113:
                case 209:
                case 241:
                case 117:
                case 245:
                case 247:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -180f), Vector3.one);
                case 64:
                case 65:
                case 193:
                case 69:
                case 197:
                case 71:
                case 199:
                case 213:
                case 215:
                case 223:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
            }

            return Matrix4x4.identity;
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CellTile))]
    public class CellTileEditor : Editor
    {
        #region Private Properties

        private CellTile Tile => target as CellTile;

        #endregion

        #region Unity

        public void OnEnable()
        {
            if (Tile.mSprites == null || Tile.mSprites.Length != 16)
            {
                Tile.mSprites = new Sprite[16];
                EditorUtility.SetDirty(Tile);
            }
        }

        #endregion

        #region Public Methods

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Place sprites shown based on the contents of the sprite.");
            EditorGUILayout.Space();

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            Tile.mSprites[0] =
                (Sprite)EditorGUILayout.ObjectField("Filled", Tile.mSprites[0], typeof(Sprite), false, null);

            Tile.mSprites[1] =
                (Sprite)EditorGUILayout.ObjectField("Three Sides", Tile.mSprites[1], typeof(Sprite), false, null);

            Tile.mSprites[2] = (Sprite)EditorGUILayout.ObjectField("Two Sides and One Corner",
                Tile.mSprites[2],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[3] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Sides",
                Tile.mSprites[3],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[4] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Sides",
                Tile.mSprites[4],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[5] = (Sprite)EditorGUILayout.ObjectField("One Side and Two Corners",
                Tile.mSprites[5],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[6] = (Sprite)EditorGUILayout.ObjectField("One Side and One Lower Corner",
                Tile.mSprites[6],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[7] = (Sprite)EditorGUILayout.ObjectField("One Side and One Upper Corner",
                Tile.mSprites[7],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[8] =
                (Sprite)EditorGUILayout.ObjectField("One Side", Tile.mSprites[8], typeof(Sprite), false, null);

            Tile.mSprites[9] =
                (Sprite)EditorGUILayout.ObjectField("Four Corners", Tile.mSprites[9], typeof(Sprite), false, null);

            Tile.mSprites[10] =
                (Sprite)EditorGUILayout.ObjectField("Three Corners", Tile.mSprites[10], typeof(Sprite), false, null);

            Tile.mSprites[11] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Corners",
                Tile.mSprites[11],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[12] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Corners",
                Tile.mSprites[12],
                typeof(Sprite),
                false,
                null);

            Tile.mSprites[13] =
                (Sprite)EditorGUILayout.ObjectField("One Corner", Tile.mSprites[13], typeof(Sprite), false, null);

            Tile.mSprites[14] =
                (Sprite)EditorGUILayout.ObjectField("Empty", Tile.mSprites[14], typeof(Sprite), false, null);

            Tile.mSprites[15] =
                (Sprite)EditorGUILayout.ObjectField("Checkered", Tile.mSprites[15], typeof(Sprite), false, null);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Tile);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        #endregion
    }
#endif
}