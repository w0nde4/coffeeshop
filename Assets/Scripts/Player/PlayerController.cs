using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minVelocityThreshold = 0.1f;
    
    [Header("Camera")] 
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float rotationSpeed = 20f;

    private Rigidbody _rb;
    private float _xRotation;
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private bool _isLookLocked = false;
    private bool _isMoveLocked = false;
    
    public bool IsMoving { get; private set; }
    public void SetMoveInput(Vector2 input) => _moveInput = input;
    public void SetLookInput(Vector2 input) => _lookInput = input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void LateUpdate()
    {
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        if (_isMoveLocked) _moveInput = Vector2.zero;
        
        var moveDirection = (transform.forward * _moveInput.y 
                            + transform.right * _moveInput.x).normalized;
        var targetVelocity = moveDirection * moveSpeed;

        _rb.linearVelocity = new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.z);
        IsMoving = _moveInput.magnitude > minVelocityThreshold;
    }

    private void HandleMouseLook()
    {
        if (_isLookLocked) _lookInput = Vector2.zero;
        
        var mouseX = _lookInput.x * mouseSensitivity * Time.deltaTime;
        var mouseY = _lookInput.y * mouseSensitivity * Time.deltaTime;
        
        _xRotation = Mathf.Clamp(_xRotation - mouseY, -maxLookAngle, maxLookAngle);

        cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    #region PublicAPI

    public void LockLookInput()
    {
        _isLookLocked = true;
        _lookInput = Vector2.zero;
    }

    public void UnlockLookInput()
    {
        _isLookLocked = false;   
    }

    public void LockMoveInput()
    {
        _isMoveLocked = true;
        _moveInput = Vector2.zero;
    }

    public void UnlockMoveInput()
    {
        _isMoveLocked = false;
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void LookAtTarget(Transform target)
    {
        StartCoroutine(LookAtTargetRoutine(target));
    }

    #endregion

    private IEnumerator LookAtTargetRoutine(Transform target)
    {
        if (!target) yield break;

        var direction = target.position - transform.position;
        direction.y = 0;

        if (direction == Vector3.zero) yield break;

        var targetRotation = Quaternion.LookRotation(direction.normalized);
    
        var isLookingAtTarget = false;
    
        while (!isLookingAtTarget)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
            var angle = Quaternion.Angle(transform.rotation, targetRotation);
            isLookingAtTarget = angle < 1f;
        
            yield return null;
        }
    
        transform.rotation = targetRotation;
    }
}