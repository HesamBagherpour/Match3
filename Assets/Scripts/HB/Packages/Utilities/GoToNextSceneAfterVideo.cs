using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace HB.Packages.Utilities
{
    public class GoToNextSceneAfterVideo: MonoBehaviour
    {
        public VideoPlayer _videoPlayer;
        [SerializeField]private float _delayAfterVideoFinish = .5f;
        private float timer;
        private bool nextScneTimer = false;
        
        private void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        private void Start()
        {
        
            timer = 0;
            nextScneTimer = false;
            _videoPlayer.Play();
            _videoPlayer.loopPointReached += VideoPlayerOnloopPointReached ;

            StartCoroutine(LoadNextScene());


        }

        private void VideoPlayerOnloopPointReached(VideoPlayer source)
        {
            timer = 0;
            nextScneTimer = true;
        }

        private void Update()
        {
            
        }

        public IEnumerator LoadNextScene()
        {
            var operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            operation.allowSceneActivation = false;

            while (_videoPlayer.isPlaying || !nextScneTimer)
            {   
                yield return new WaitForEndOfFrame();
            }

            while (timer < _delayAfterVideoFinish)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            operation.allowSceneActivation = true;
        }
    }
}