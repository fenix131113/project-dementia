using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class AudioQueueManager : MonoBehaviour
    {
        public AudioSource voiceSource;
        private Queue<AudioClip> voiceQueue = new();
        private bool isStoryActive = false;

        public void PlayStoryDialogue(List<AudioClip> dialogue, float delayBeforeStart = 1f)
        {
            if (isStoryActive) return;
            StartCoroutine(PlayDialogueSequence(dialogue, delayBeforeStart));
        }

        private IEnumerator PlayDialogueSequence(List<AudioClip> clips, float delay)
        {
            isStoryActive = true;

            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            foreach (var clip in clips)
            {
                voiceSource.clip = clip;
                voiceSource.Play();
                yield return new WaitForSeconds(clip.length);
            }

            isStoryActive = false;
            PlayNextInQueue();
        }

        public void PlayOtherVoice(AudioClip clip)
        {
            if (isStoryActive)
            {
                voiceQueue.Enqueue(clip);
            }
            else
            {
                voiceSource.clip = clip;
                voiceSource.Play();
            }
        }

        private void PlayNextInQueue()
        {
            if (voiceQueue.Count > 0)
            {
                var next = voiceQueue.Dequeue();
                StartCoroutine(PlayQueuedClip(next));
            }
        }

        private IEnumerator PlayQueuedClip(AudioClip clip)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
            yield return new WaitForSeconds(clip.length);
            PlayNextInQueue();
        }

        public void StopAll()
        {
            StopAllCoroutines();
            voiceQueue.Clear();
            voiceSource.Stop();
            isStoryActive = false;
        }
    }
}
