using System.Collections;
using UnityEngine;

namespace AudioSystem
{
    public class RandomVoicePlayer : MonoBehaviour
    {
        public AudioClip[] randomClips;
        public float minDelay = 120f;
        public float maxDelay = 300f;

        private AudioQueueManager queueManager;

        private void Start()
        {
            queueManager = FindObjectOfType<AudioQueueManager>();
            StartCoroutine(RandomVoiceRoutine());
        }

        private IEnumerator RandomVoiceRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

                if (randomClips.Length > 0)
                {
                    var clip = randomClips[Random.Range(0, randomClips.Length)];
                    queueManager.PlayOtherVoice(clip);
                }
            }
        }
    }
}