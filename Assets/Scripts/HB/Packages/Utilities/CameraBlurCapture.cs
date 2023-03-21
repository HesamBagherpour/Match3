using UnityEngine;
using UnityEngine.Serialization;

namespace HB.Packages.Utilities
{
    public class CameraBlurCapture : MonoBehaviour
    {
        private Camera _camera;
        [FormerlySerializedAs("m_texture")] public RenderTexture mTexture;
        [FormerlySerializedAs("m_material")] public Material mMaterial;
        [FormerlySerializedAs("m_shader")] public Shader mShader;

        void Start()
        {
            _camera = GetComponent<Camera>();
        }
        void Update()
        {
            //if (render) 
            //    camera.Render();
        }

        [ContextMenu("RenderManually")]
        public void RenderManually()
        {
            _camera.targetTexture = mTexture;
            _camera.Render();
            _camera.targetTexture = null;
            Blur(mTexture, 1);
        }

        RenderTexture Blur(RenderTexture source, int iterations)
        {
            RenderTexture rt = source;
            Material mat = new Material(mShader);
            RenderTexture blit = RenderTexture.GetTemporary(1280, 720);
            for (int i = 0; i < iterations; i++)
            {
                Graphics.SetRenderTarget(blit);
                GL.Clear(true, true, Color.black);
                Graphics.Blit(rt, blit, mat);
                Graphics.SetRenderTarget(rt);
                GL.Clear(true, true, Color.black);
                Graphics.Blit(blit, rt, mat);
            }
            RenderTexture.ReleaseTemporary(blit);
            return rt;
        }
    }
}
