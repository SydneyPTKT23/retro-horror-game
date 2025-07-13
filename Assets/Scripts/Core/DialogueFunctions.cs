using UnityEngine;

namespace SLC.RetroHorror.Core
{
    /// <summary>
    /// ALL FUNCTIONS IN THIS CLASS MUST BE NAMED EXACTLY THE SAME AS THE INK TAG
    /// THAT IS USED TO CALL THEM, OTHERWISE EVERYTHING BREAKS AND I EXPLODE.
    /// </summary>
    public class DialogueFunctions : MonoBehaviour
    {
        private DialogueVariables dialogueVariables => DialogueManager.Instance.dialogueVariables;
        private DialogueManager dialogueManager => DialogueManager.Instance;

        public void TestFunction()
        {
            Debug.Log("Test function was called!");
            Debug.Log($"State of test variable: {dialogueManager.GetVariableState("g_test_seen")}");
        }
    }
}