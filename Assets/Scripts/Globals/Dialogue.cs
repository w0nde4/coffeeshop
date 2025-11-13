using System;
using UnityEngine;

namespace Globals
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [Serializable]
        public class DialogueLine
        {
            [TextArea(3, 5)]
            public string text;
            public float displayTime = 0f;
            public bool waitForInput = true;
        }

        [Serializable]
        public class PlayerChoice
        {
            public string choiceText;
            public Dialogue nextDialogue;
            
            [Header("Optional consequences")]
            public ChoiceEffect.ChoiceEffect[] effects;
        }

        [Header("Dialogue Lines")]
        public DialogueLine[] lines;

        [Header("Next Dialogue Chain")]
        public Dialogue nextDialogue;
        
        [Header("Player Choices (optional)")]
        public PlayerChoice[] playerChoices;

        [Header("Dialogue Settings")]
        public bool skippable = false;
        
        [Header("NPC Behaviour After Dialogue")]
        public bool startOrderAfterDialogue = false;
        public bool leaveAfterDialogue = false;
    }
}