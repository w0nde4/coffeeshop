using Coffee;
using NPC;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction settings")]
        [SerializeField] private float interactionDistance = 2f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private TextMeshProUGUI interactText;

        private Interactible _currentInteractable;
        private RaycastHit _lastHit;
        private bool _hasHit;
    
        private PlayerInventory PlayerInventory => GameServices.PlayerInventory;

        private void Update()
        {
            UpdateRaycast();
            UpdateUI();
            HandleInteraction();
        }
    
        private void UpdateRaycast()
        {
            _hasHit = Physics.Raycast(transform.position,
                transform.forward,
                out _lastHit,
                interactionDistance);
        
            _currentInteractable = _hasHit ?  _lastHit.collider.GetComponent<Interactible>() : null;
        }

        private void UpdateUI()
        {
            var isEmptyInteractText = !_currentInteractable 
                                      || !_currentInteractable.enabled
                                      || !_currentInteractable.DisplayActionButton
                                      || _currentInteractable is NpcFacade { CurrentState: NpcState.Talking };
        
            if (isEmptyInteractText)
            {
                interactText?.SetText("");
                return;
            }

            var display = _currentInteractable.DisplayActionButton ? "[E] " : string.Empty;
            interactText?.SetText($"{display}{GetInteractionText(_currentInteractable)}");
        }
    
        private string GetInteractionText(Interactible interactable)
        {
            if (interactable is not CoffeeCup coffeeCup) return interactable.InteractText;

            if (PlayerInventory?.CurrentItem &&
                PlayerInventory.CurrentItem.TryGetComponent<Lid>(out _) &&
                coffeeCup.CurrentState == CoffeeCup.CupState.Filled)
            {
                return "Надеть крышку";
            }
            
            return coffeeCup.InteractText;
        }

        private void HandleInteraction()
        {
            if (_currentInteractable && Input.GetKeyDown(interactKey))
            {
                _currentInteractable.Interact(gameObject);
            }
        }
    
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            var startPos = transform.position;
            var endPos = transform.position + transform.forward * interactionDistance;

            if (_hasHit)
            {
                endPos = _lastHit.point;

                if (_lastHit.collider.GetComponent<Interactible>() != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(_lastHit.point, 0.1f);
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawLine(startPos, endPos);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(startPos, 0.05f);
        }
    }
}