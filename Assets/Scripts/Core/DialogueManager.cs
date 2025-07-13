using Ink.Runtime;
using SLC.RetroHorror.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SLC.RetroHorror.Core
{
    public class DialogueManager : MonoBehaviour
    {

        #region variables

        public static DialogueManager Instance { get; private set; }

        [Header("Engine Variables")]
        [SerializeField] private NPCData narratorData;
        [SerializeField] private NPCData[] npcDatas;
        [SerializeField] private InputReader input;
        private PlayerController playerController;
        [SerializeField] private AudioSource voiceSource;

        [Header("Dialogue")]
        [SerializeField] private TextAsset globalInkVariables;
        private DialogueVariables dialogueVariables;
        [SerializeField] private Sprite fallbackPortrait;
        [SerializeField] private AudioClip fallbackVoice;
        [SerializeField] private GameObject portraitBG;
        [SerializeField] private Image dialogueImage;
        [SerializeField] private TextMeshProUGUI dialogueSpeaker;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        public Story currentStory { get; private set; }
        [SerializeField] private float typeSpeed = 20f;
        private bool isTyping = false;
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
        private bool choiceWasMade = false;

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

            dialogueVariables = new(globalInkVariables);

            input.DialogueSubmitCancelledEvent += HandleSubmit;

            CheckForDuplicateNPCIds();
        }

        private void Start()
        {
            playerController = FindFirstObjectByType<PlayerController>();
            dialoguePanel.SetActive(false);
        }

        private void Update()
        {
            transform.position = playerController.transform.position;
        }

        #endregion

        #region dialogue methods

        //this enters dialogue with the inkJSON file assigned to the npc
        public void EnterDialogue(TextAsset inkJSON, Action _doAfterDialogue = null)
        {
            input.EnableDialogueInput();

            //first this sets the ink story as the active dialogue and activates dialogue panel
            currentStory = new Story(inkJSON.text);
            dialogueVariables.StartListening(currentStory);
            doAfterDialogue = _doAfterDialogue;
            dialoguePanel.SetActive(true);

            //continue story prints dialogue so it's called here
            ContinueStory();
        }

        //dialogue printer
        private void ContinueStory()
        {
            if (choiceWasMade && currentStory.canContinue)
            {
                choiceWasMade = false;
                currentStory.Continue();
                if (currentStory.currentChoices.Count != 0) DisplayChoices();
                else StartCoroutine(TypeDialogue());
            }
            else if (currentStory.canContinue)
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
            dialogueVariables.StopListening(currentStory);

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

            choiceWasMade = true;
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
            WaitForSeconds realTypeTime = new(maxTypeTime / typeSpeed);

            SetSpeakerData();
            // voiceSource.Play();

            foreach (char c in originalText.ToCharArray())
            {
                if (stopTyping) break;
                alphaIndex++;
                dialogueText.text = originalText;
                displayedText = dialogueText.text.Insert(alphaIndex, alphaCode);
                dialogueText.text = displayedText;
                yield return realTypeTime;
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

        private void SetSpeakerData()
        {
            //grab tags from current line and show either player portrait or NPC portrait based on last tag in list
            if (currentStory.currentTags.Any())
            {
                string _speakerId = "";
                NPCData.Emotion _speakerEmotion = NPCData.Emotion.neutral;

                foreach (string tag in currentStory.currentTags)
                {
                    if (tag.Contains("narrator"))
                    {
                        Debug.Log("Found narrator tag");
                        dialogueSpeaker.text = narratorData.speakerName;
                        dialogueImage.sprite = fallbackPortrait;
                        voiceSource.clip = narratorData.voiceClip;
                        portraitBG.SetActive(false);
                        return;
                    }
                    else if (tag.Contains("speaker:"))
                    {
                        //parse out clean speaker id from tag
                        _speakerId = tag.Replace("speaker:", null);
                        _speakerId = _speakerId.Replace(" ", null);
                        Debug.Log($"Found speaker tag: \"{_speakerId}\"");
                    }
                    else if (tag.Contains("emotion:"))
                    {
                        //parse out emotion string from tag
                        string _emotion = tag.Replace("emotion:", null);
                        _emotion = _emotion.Replace(" ", null);
                        Debug.Log($"Found emotion tag: \"{_emotion}\"");

                        //try to parse emotion string to enum, use neutral on failure
                        if (!Enum.TryParse(_emotion, out _speakerEmotion))
                            _speakerEmotion = NPCData.Emotion.neutral;
                    }
                }

                NPCData _speakerData;

                //try to get a reference to speaker data based on parsed id string
                try
                {
                    _speakerData = npcDatas.Where(data => data.speakerId == _speakerId).First();

                    dialogueSpeaker.text = _speakerData.speakerName;

                    dialogueImage.sprite = _speakerData.GetActiveSprite(_speakerEmotion);
                    if (dialogueImage.sprite == null) dialogueImage.sprite = fallbackPortrait;

                    voiceSource.clip = _speakerData.voiceClip;
                    if (voiceSource.clip == null) voiceSource.clip = fallbackVoice;
                }
                //couldn't find speaker data based on parsed string
                catch (Exception e)
                {
                    Debug.LogError($"Error while trying to find speaker data: \n{e}");
                    dialogueSpeaker.text = "";
                    dialogueImage.sprite = fallbackPortrait;
                    voiceSource.clip = fallbackVoice;
                }
            }
            else
            {
                dialogueSpeaker.text = "";
                dialogueImage.sprite = fallbackPortrait;
                voiceSource.clip = fallbackVoice;
                Debug.Log($"You have given me a dialogue line without tags, how dare you.");
            }

            portraitBG.SetActive(true);
        }

        public Ink.Runtime.Object GetVariableState(string _variableName)
        {
            Ink.Runtime.Object _variableValue = null;
            dialogueVariables.variables.TryGetValue(_variableName, out _variableValue);
            if (_variableValue == null)
            {
                Debug.LogWarning($"{_variableName} was null, did you mean to reference it?");
            }
            return _variableValue;
        }

        #endregion

        #region input handlers

        public void UnsubscribeDialogueEvents()
        {
            input.DialogueSubmitEvent -= HandleSubmit;
        }

        private void HandleSubmit()
        {
            if (isTyping)
            {
                stopTyping = true;
                return;
            }
            if (!makingChoice && !disableInput) ContinueStory();
        }

        #endregion

        #region error handling

        private void CheckForDuplicateNPCIds()
        {
            List<string> ids = new();
            npcDatas.ToList().ForEach(data => ids.Add(data.speakerId));

            ids.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList().ForEach((id) =>
            {
                string error = $"Found NPCs with duplicate ID: {id}\nOn objects: ";
                npcDatas.Where(uid => uid.speakerId == id).ToList().ForEach(uid => error += $"{uid.name} ");
                Debug.LogError(error);
            });
        }

        #endregion
    }

    #region dialogue variables

    public class DialogueVariables
    {
        public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

        public DialogueVariables(TextAsset _globalVariableAsset)
        {
            Story _globalVariableStory = new(_globalVariableAsset.text);
            variables = new();

            foreach (string name in _globalVariableStory.variablesState)
            {
                var value = _globalVariableStory.variablesState.GetVariableWithName(name);
                variables.Add(name, value);
                Debug.Log($"Variable global dialogue initialized: {name} = {value}");
            }
        }

        public void StartListening(Story _story)
        {
            VariablesToStory(ref _story);
            _story.variablesState.variableChangedEvent += VariableChanged;
        }

        public void StopListening(Story _story)
        {
            _story.variablesState.variableChangedEvent -= VariableChanged;
        }

        private void VariableChanged(string _varName, Ink.Runtime.Object _value)
        {
            Debug.Log($"Variable changed: {_varName} = {_value}");

            if (variables.ContainsKey(_varName))
            {
                variables[_varName] = _value;
            }
        }

        private void VariablesToStory(ref Story _story)
        {
            foreach (var variable in variables)
            {
                _story.variablesState.SetGlobal(variable.Key, variable.Value);
            }
        }
    }

    #endregion
}
