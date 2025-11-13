using System;
using Globals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerChoiceUI : MonoBehaviour
    {
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private Button[] choiceButtons;
        
        private Action<Dialogue> _onChoiceSelected;
        
        public void ShowChoices(Dialogue.PlayerChoice[] choices, Action<Dialogue> onChoiceSelected)
        {
            _onChoiceSelected = onChoiceSelected;

            if (choices == null || choices.Length == 0)
            {
                choicePanel.SetActive(false);
                return;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            choicePanel.SetActive(true);

            for (var i = 0; i < choiceButtons.Length; i++)
            {
                var button = choiceButtons[i];
                var text = button.GetComponentInChildren<TextMeshProUGUI>();
                button.onClick.RemoveAllListeners();

                if (i < choices.Length)
                {
                    var choice = choices[i];
                    text.text = choice.choiceText;
                    button.gameObject.SetActive(true);

                    button.onClick.AddListener(() =>
                    {
                        ApplyChoice(choice);
                    });
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        private void ApplyChoice(Dialogue.PlayerChoice choice)
        {
            if (choice.effects != null)
            {
                foreach (var effect in choice.effects)
                {
                    effect?.ApplyEffect();
                }
            }

            choicePanel.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _onChoiceSelected?.Invoke(choice.nextDialogue);
        }
    }
}