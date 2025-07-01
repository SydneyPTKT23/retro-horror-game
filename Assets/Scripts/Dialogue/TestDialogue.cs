using UnityEngine;

namespace SLC.RetroHorror.Dialogue
{
    public class TestDialogue : MonoBehaviour
    {
        [SerializeField] TextAsset testAsset;
        public void RunTestDialogue()
        {
            DialogueManager.Instance.EnterDialogue(testAsset, null, "Tester");
        }
    }
}
