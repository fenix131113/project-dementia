using UnityEngine;

namespace AudioSystem
{
    public class CharacterAudioPlayer : MonoBehaviour
    {
        [Header("Footsteps")]
        public AudioSource footstepSource;
        public AudioClip[] footstepClips;

        [Header("Jump")]
        public AudioSource jumpSource;
        public AudioClip[] jumpClips;

        public void PlayFootstep()
        {
            PlayRandom(footstepClips, footstepSource);
        }

        public void PlayJump()
        {
            PlayRandom(jumpClips, jumpSource);
        }

        private void PlayRandom(AudioClip[] clips, AudioSource source)
        {
            if (clips == null || clips.Length == 0 || source == null) return;
            var clip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(clip);
        }
    }
}