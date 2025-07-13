using UnityEngine;

namespace SLC.RetroHorror.Core
{
    [CreateAssetMenu(menuName = "NPCData")]
    public class NPCData : ScriptableObject
    {
        public enum Emotion
        {
            neutral = 0,
            happy,
            sad,
            angry,
            scared
        }

        [field: SerializeField] public string speakerId { get; private set; }
        [field: SerializeField] public string speakerName { get; private set; }
        [field: SerializeField] public AudioClip voiceClip { get; private set; }
        [SerializeField] private Sprite neutralSprite;
        [SerializeField] private Sprite happySprite;
        [SerializeField] private Sprite sadSprite;
        [SerializeField] private Sprite angrySprite;
        [SerializeField] private Sprite scaredSprite;

        public Sprite GetActiveSprite(Emotion _currentEmotion)
        {
            return _currentEmotion switch
            {
                Emotion.neutral => neutralSprite,
                Emotion.happy => happySprite,
                Emotion.sad => sadSprite,
                Emotion.angry => angrySprite,
                Emotion.scared => scaredSprite,
                _ => neutralSprite,
            };
        }
    }
}
