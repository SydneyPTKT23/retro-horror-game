using Ink.Runtime;
using SLC.RetroHorror.Core;
using SLC.RetroHorror.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SLC.RetroHorror.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        #region variables

        [Header("Engine Variables")]
        public static DialogueManager Instance { get; private set; }
        [SerializeField] private AudioClip playerVoice;
        private PlayerController playerController;
        [SerializeField] private InputReader input;
        private AudioSource voiceSource;
        private AudioClip npcVoice;

        [Header("Dialogue UI")]
        [SerializeField] private string playerName = "Player";
        [SerializeField] private Sprite playerPortrait;
        private Sprite NPCPortrait;
        private string NPCName;
        [SerializeField] private GameObject portraitBG;
        [SerializeField] private Image dialogueImage;
        [SerializeField] private TextMeshProUGUI dialogueSpeaker;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        public Story currentStory { get; private set; }
        private bool isTyping = false;
        [SerializeField] private float typeSpeed = 20f;
        private bool stopTyping = false;
        private bool disableInput = false;

        //Const values related to typing. Do not change.
        private const string alphaCode = "<color=#00000000>";
        private const float maxTypeTime = 0.1f;

        [Header("Choices UI")]
        [SerializeField] private GameObject choiceHandler;
        [SerializeField] private GameObject choiceObject;
        private const float timeBeforeChoices = 0.2f;
        private bool makingChoice = false;

        [Header("Other Variables")]
        private Action doAfterDialogue;

        #endregion

        #region standard methods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log("Found more than one DialogueManager, fixing.");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            input.DialogueSubmitEvent += HandleSubmit;
        }

        private void Start()
        {
            playerController = FindFirstObjectByType<PlayerController>();
            dialoguePanel.SetActive(false);
            voiceSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            transform.position = playerController.transform.position;
        }

        #endregion

        #region dialogue methods

        //this enters dialogue with the inkJSON file assigned to the npc
        public void EnterDialogue(TextAsset inkJSON, AudioClip _npcVoice, string _name, Action _doAfterDialogue = null, Sprite _portrait = null)
        {
            input.EnableDialogueInput();
            //first this sets the ink story as the active dialogue and activates dialogue panel
            currentStory = new Story(inkJSON.text);
            NPCPortrait = _portrait;
            NPCName = _name;
            npcVoice = _npcVoice;
            doAfterDialogue = _doAfterDialogue;
            dialoguePanel.SetActive(true);

            //continue story prints dialogue so it's called here
            ContinueStory();
        }

        //dialogue printer
        private void ContinueStory()
        {
            if (currentStory.canContinue)
            {
                StartCoroutine(TypeDialogue());
            }
            else
            {
                //if no more story left, exit dialogue
                ExitDialogue();
            }
        }

        //this sets dialogue panel inactive, empties dialogue text and sets input scheme back to gameplay
        private void ExitDialogue()
        {
            doAfterDialogue?.Invoke();
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
            input.EnablePlayerInput();
        }

        //choice printer
        private void DisplayChoices()
        {
            List<Choice> currentChoices = currentStory.currentChoices;
            int index = 0;
            //loop to instantiate a choice object onto the screen for every possible choice
            foreach (Choice choice in currentChoices)
            {
                int capturedIndex = index;      //can't use raw index as that will increase for each loop
                                                //instantiate object as child object of choice handler to let layoutgroup handle their positioning
                GameObject _choiceObject = Instantiate(choiceObject, choiceHandler.transform);
                //instantiated object's text is set to the text of the current choice in list
                _choiceObject.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                //add listener to button so we can make choice based on... made choice
                _choiceObject.GetComponent<Button>().onClick.AddListener(() => MakeChoice(capturedIndex));
                index++;
            }
            disableInput = false;
            //if there were choices, highlight choice and set makingChoice to true to disable inputs so story doesn't try to advance
            if (index != 0)
            {
                StartCoroutine(SelectFirstChoice());
                makingChoice = true;
            }
        }

        private IEnumerator SelectFirstChoice()
        {
            //unity apparently requires you to wait for the end of a frame until you can highlight an option so we do that
            EventSystem.current.SetSelectedGameObject(null);
            yield return null;
            EventSystem.current.SetSelectedGameObject(choiceHandler.transform.GetChild(0).gameObject);
        }

        //this is called when choice is made to advance ink story based on made choice
        public void MakeChoice(int choiceNumber)
        {
            currentStory.ChooseChoiceIndex(choiceNumber);
            //choice is made, we are no longer making a choice and inputs are re-enabled
            makingChoice = false;

            //after choice is made, destroy active choice buttons
            foreach (Transform child in choiceHandler.transform)
            {
                Destroy(child.gameObject);
            }

            ContinueStory();
        }

        //this coroutine types the dialogue one letter at a time
        private IEnumerator TypeDialogue()
        {
            isTyping = true;
            dialogueText.text = "";
            string originalText = currentStory.Continue();
            string displayedText;
            int alphaIndex = 0;

            //grab tags from current line and show either player portrait or NPC portrait based on last tag in list
            if (currentStory.currentTags.Any())
            {
                portraitBG.SetActive(true);
                if (currentStory.currentTags.Last() == "Player")
                {
                    dialogueSpeaker.text = playerName;
                    if (playerPortrait != null)
                    dialogueImage.sprite = playerPortrait;
                    // voiceSource.clip = playerVoice;
                }
                else
                {
                    dialogueSpeaker.text = NPCName;
                    dialogueImage.sprite = NPCPortrait;
                    voiceSource.clip = npcVoice;
                }
                // voiceSource.Play();
            }
            else
            {
                portraitBG.SetActive(false);
                dialogueSpeaker.text = "";
            }

            foreach (char c in originalText.ToCharArray())
            {
                if (stopTyping) break;
                alphaIndex++;
                dialogueText.text = originalText;
                displayedText = dialogueText.text.Insert(alphaIndex, alphaCode);
                dialogueText.text = displayedText;
                yield return new WaitForSeconds(maxTypeTime / typeSpeed);
            }

            if (stopTyping) dialogueText.text = originalText;
            stopTyping = false;
            isTyping = false;
            disableInput = true;
            // voiceSource.Stop();
            yield return new WaitForSeconds(timeBeforeChoices);
            //this is called on advance in case there are choices, does nothing if there are none
            DisplayChoices();
        }

        #endregion

        #region input handlers

        public void UnsubscribeDialogueEvents()
        {
            input.DialogueSubmitEvent -= HandleSubmit;
        }

        private void HandleSubmit()
        {
            Debug.Log("handling submit");
            if (isTyping)
            {
                stopTyping = true;
                return;
            }
            if (!makingChoice && !disableInput) ContinueStory();
        }

        #endregion
    }
}
