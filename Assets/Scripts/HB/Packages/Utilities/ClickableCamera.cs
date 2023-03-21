using UnityEngine;

namespace HB.Packages.Utilities
{
    public class ClickableCamera : MonoBehaviour
    {
        private readonly Vector3 _aspectRatioOffsetFor3X4 = new Vector3(-21.4f, 8.47f, 20.75f);
        private void Awake()
        {
            ClickableObjectSystem.Instance.SetCamera(GetComponent<Camera>());
            if ((float)Screen.width / (float)Screen.height < 1.5f)
                transform.position = _aspectRatioOffsetFor3X4;
            // Debug.Log("width " + Screen.width + " height " + Screen.height);
        }
    }
}
