using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input actions")] 
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }

    private void Update()
    {
        _playerController.SetMoveInput(moveAction.action.ReadValue<Vector2>());
        _playerController.SetLookInput(lookAction.action.ReadValue<Vector2>());
    }
}