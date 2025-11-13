using System;
using Globals;
using UnityEngine;

namespace NPC
{
    public class NpcDialogue : MonoBehaviour
    {
        [SerializeField] private Dialogue orderDialogue;
        [SerializeField] private Dialogue afterOrderDialogue;
        
        private NpcStateMachine _stateMachine;
        private NpcOrder _order;
        
        public event Action OnDialogueEnd;
        
        public Dialogue OrderDialogue => orderDialogue;
        public Dialogue AfterOrderDialogue => afterOrderDialogue;

        private void Awake()
        {
            _stateMachine = GetComponent<NpcStateMachine>();
            _order = GetComponent<NpcOrder>();
        }

        public void TryStartDialogue(Transform player)
        {
            if(DialogueManager.Instance.IsDialogueActive)
                DialogueManager.Instance.ContinueDialogue();

            else
            {
                var currentDialogue = _order.IsOrderReceived? afterOrderDialogue : orderDialogue;
                _stateMachine.ChangeState(NpcState.Talking);
                DialogueManager.Instance.StartDialogue(currentDialogue, gameObject.name, gameObject);
            }
        }

        private void EndDialogue()
        {
            OnDialogueEnd?.Invoke();
        }
    }
}