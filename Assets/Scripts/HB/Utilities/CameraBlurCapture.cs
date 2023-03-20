using UnityEngine;

namespace HB.Utilities
{
    public class CameraBlurCapture : MonoBehaviour
    {
        private Camera camera = null;
        public bool render = true;
        public RenderTexture m_texture;
        public Material m_material;
        public Shader m_shader;

        void Start()
        {
            camera = GetComponent<Camera>();
        }
        void Update()
        {
            //if (render) 
            //    camera.Render();
        }

        [ContextMenu("RenderManually")]
        public void RenderManually()
        {
            camera.targetTexture = m_texture;
            camera.Render();
            camera.targetTexture = null;
            Blur(m_texture, 1);
        }

        RenderTexture Blur(RenderTexture source, int iterations)
        {
            RenderTexture rt = source;
            Material mat = new Material(m_shader);
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
