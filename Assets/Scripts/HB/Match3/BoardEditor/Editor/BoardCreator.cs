using UnityEditor;
using UnityEngine;

namespace HB.Match3.BoardEditor.Editor
{
    public class BoardCreator : MonoBehaviour
    {
        private static readonly string toolName = "LevelEditor";

        [MenuItem("GameObject/Match3/BoardEditor", false, -10)]
        static void CreateBoard()
        {
            // Create board view
            // global::Match3.BoardEditor.BoardEditor boardEditor = Instantiate(Resources.Load<global::Match3.BoardEditor.BoardEditor>("LevelEditor"));
            // GameObject pz = Instantiate(boardEditor.puzzleController.gameObject);
            // boardEditor.puzzleController = pz.GetComponent<PuzzleController>();
            // EditorUtility.SetDirty(boardEditor);
        }
    }
}