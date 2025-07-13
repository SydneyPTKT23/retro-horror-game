using UnityEngine;
using UnityEngine.UI;

namespace SLC.RetroHorror.Core
{
    public class TestDialogue : MonoBehaviour
    {
        [SerializeField] TextAsset testAsset;
        [SerializeField] private string[] externalFunctions;
        [SerializeField] private Color seenColor;
        [SerializeField] private Color unSeenColor;
        private Image image;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            bool seenExample = ((Ink.Runtime.BoolValue)DialogueManager.Instance.GetVariableState("g_test_seen")).value;

            image.color = seenExample ? seenColor : unSeenColor;
        }

        public void RunTestDialogue()
        {
            DialogueManager.Instance.EnterDialogue(testAsset, externalFunctions, null);
        }
    }
}
