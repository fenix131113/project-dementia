using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class DialogueSequencePlayer : MonoBehaviour
    {
        [Tooltip("Аудио по порядку")] public List<AudioClip> dialogueClips;
        [Tooltip("Задержка перед началом воспроизведения")] public float delayBeforeStart = 1f;
        [Tooltip("Автозапуск при старте сцены")] public bool playOnStart = true;

        private AudioQueueManager queueManager;

        private void Start()
        {
            queueManager = FindObjectOfType<AudioQueueManager>();
            if (playOnStart && dialogueClips.Count > 0)
            {
                queueManager.PlayStoryDialogue(dialogueClips, delayBeforeStart);
            }
        }
    }
}
