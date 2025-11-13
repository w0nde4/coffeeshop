using System;
using System.Collections;
using NPC;
using Player;
using TMPro;
using UnityEngine;

namespace Globals
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;
    
        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject continuePanel;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private PlayerChoiceUI playerChoiceUI;
    
        private Dialogue _dialogue;
        private int _lineIndex = 0;
        private bool _isAutoContinue = false;
        private float _autoTimer = 0f;
        private GameObject _npcObject;

        private Action _onDialogueEnd;
        
        public bool IsDialogueActive { get; private set; } = false;

        #region Lifecycle

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            dialoguePanel?.SetActive(false);
            GameServices.RegisterDialogueManager(this);
        }

        private void Update()
        {
            if (!IsDialogueActive) return;
            if(_isAutoContinue)
            {
                _autoTimer -= Time.deltaTime;
                if (_autoTimer <= 0f)
                    ContinueDialogue();
            }

            if (Input.GetKeyDown(KeyCode.E) && _dialogue.skippable)
                ContinueDialogue();
        }

        #endregion

        #region Public API

        public void StartDialogue(Dialogue dialogue, string speakerName, GameObject npc = null)
        {
            if (!ValidateDialogue(dialogue)) return;
            
            _dialogue = dialogue;
            _npcObject = npc;
            _lineIndex = 0;
            IsDialogueActive = true;

            SetupUI(true, speakerName);
            LockPlayerControls();
            DisplayCurrentLine();
            
            /*if (dialoguePanel)
                dialoguePanel.SetActive(true);
            
            if (nameText)
                nameText.text = speakerName;
        
            if(continuePanel)
                continuePanel.SetActive(_dialogue.skippable);
            
            if(playerChoiceUI)
                playerChoiceUI.gameObject.SetActive(false);

            GameServices.PlayerController.LockLookInput();
            GameServices.PlayerController.LockMoveInput();
            GameServices.PlayerController.ShowCursor();
            if (_npcObject) GameServices.PlayerController.LookAtTarget(_npcObject.transform);

            DisplayCurrentLine();*/
        }

        public void ContinueDialogue()
        {
            if (!IsDialogueActive || _dialogue == null)
            {
                EndDialogue();
                return;
            }
        
            _lineIndex++;
        
            if (_lineIndex < _dialogue.lines.Length)
            {
                DisplayCurrentLine();
                return;
            }
            
            if (_dialogue.playerChoices is { Length: > 0 })
            {
                ShowPlayerChoices(_dialogue.playerChoices);
                return;
            }
            
            if (_dialogue.nextDialogue)
            {
                _dialogue = _dialogue.nextDialogue;
                _lineIndex = 0;
                DisplayCurrentLine();
                return;
            } 
            
            EndDialogue();
        }
        
        public void ForceDialogue(Dialogue dialogue, string speakerName, GameObject npcObject = null)
        {
            if (IsDialogueActive)
                EndDialogue();
            
            Debug.Log($"[DialogueManager] Форсированный диалог: {speakerName}");
            StartDialogue(dialogue, speakerName, npcObject);
        }
        
        public void OnContinueButtonPressed()
        {
            if (IsDialogueActive)
                ContinueDialogue();
        }
        
        public void ForceEndDialogue() => EndDialogue();
        
        public string GetCurrentLine() =>
            _dialogue && _lineIndex < _dialogue.lines.Length
            ? _dialogue.lines[_lineIndex].text
            : string.Empty;
        
        public string GetCurrentSpeaker() => nameText ? nameText.text : string.Empty;

        #endregion

        #region Dialogue Flow

        private void DisplayCurrentLine()
        {
            if (!_dialogue || _lineIndex >= _dialogue.lines.Length) 
            {
                EndDialogue();
                return;
            }

            var line = _dialogue.lines[_lineIndex];
            dialogueText.text = line.text;
        
            _isAutoContinue = line.displayTime > 0 && !line.waitForInput;
            if (_isAutoContinue)
                _autoTimer = line.displayTime;
        }
        
        private void EndDialogue()
        {
            if (!IsDialogueActive) return;
            
            IsDialogueActive = false;
            _isAutoContinue = false;
        
            SetupUI(false);
            UnlockPlayerControls();

            HandleNpcAfterDialogue(_dialogue, _npcObject);

            _onDialogueEnd?.Invoke();
            
            _dialogue = null;
            _npcObject = null;
            _lineIndex = 0;
        }

        private static void HandleNpcAfterDialogue(Dialogue dialogue, GameObject npcObject)
        {
            if (!dialogue || !npcObject) return;

            if (dialogue.startOrderAfterDialogue &&
                npcObject.TryGetComponent<NpcOrder>(out var order))
            {
                order.WaitForOrder();
                Debug.Log($"{npcObject.name} теперь ожидает кофе.");
            }
            
            else if (dialogue.leaveAfterDialogue &&
                     npcObject.TryGetComponent<NpcFacade>(out var npc))
            {
                npc.Leave();
            }
        }

        #endregion

        #region UI & Player

        private void SetupUI(bool active, string speakerName = "")
        {
            dialoguePanel?.SetActive(active);
            continuePanel?.SetActive(active && _dialogue.skippable);
            playerChoiceUI?.gameObject.SetActive(false);

            if (active && nameText)
                nameText.text = speakerName;
        }

        private void LockPlayerControls()
        {
            var pc = GameServices.PlayerController;
            if(!pc) return;
            
            pc.LockLookInput();
            pc.LockMoveInput();
            pc.ShowCursor();
            
            if(_npcObject)
                pc.LookAtTarget(_npcObject.transform);
        }

        private void UnlockPlayerControls()
        {
            var pc = GameServices.PlayerController;
            if(!pc) return;
            
            pc.UnlockLookInput();
            pc.UnlockMoveInput();
            pc.HideCursor();
        }

        #endregion

        #region Player Choices

        private void ShowPlayerChoices(Dialogue.PlayerChoice[] playerChoices)
        {
            StartCoroutine(ShowPlayerChoicesDelayed(playerChoices));
        }
        
        private IEnumerator ShowPlayerChoicesDelayed(Dialogue.PlayerChoice[] playerChoices)
        {
            yield return new WaitForSeconds(0.2f);
            playerChoiceUI.ShowChoices(playerChoices, OnPlayerChoiceSelected);
        }
        
        private void OnPlayerChoiceSelected(Dialogue nextDialogue)
        {
            if(nextDialogue) 
                StartDialogue(nextDialogue, nameText.text, _npcObject);
            else 
                EndDialogue();
        }

        #endregion

        #region Helpers

        private static bool ValidateDialogue(Dialogue dialogue)
        {
            if (!dialogue || dialogue.lines == null || dialogue.lines.Length == 0)
            {
                Debug.LogWarning("[DialogueManager] Попытка начать пустой диалог!");
                return false;
            }
            return true;
        }

        #endregion
        

        

        
    }
}