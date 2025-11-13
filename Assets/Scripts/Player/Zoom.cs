using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class Zoom : MonoBehaviour
{
    [SerializeField] private float targetFOV = 25f;
    [SerializeField] private float zoomDuration = 0.3f;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;

    private CinemachineCamera linkedCamera;
    private float startFOV = 60f;
    private float currentFOV;
    private float zoomTimer;
    private bool isZooming = false;
    private bool wasZooming = false;

    private void Start()
    {
        linkedCamera = GetComponent<CinemachineCamera>();
        if (linkedCamera != null)
        {
            startFOV = linkedCamera.Lens.FieldOfView;
            currentFOV = startFOV;
        }
    }

    private void Update()
    {
        if (!linkedCamera)
        {
            Debug.LogWarning("No Cinemachine camera");
            return;
        }

        isZooming = Input.GetKey(zoomKey);

        if (isZooming != wasZooming)
        {
            zoomTimer = 0f;
            wasZooming = isZooming;
        }

        zoomTimer += Time.deltaTime;
        var t = Mathf.Clamp01(zoomTimer / zoomDuration);

        var fromFOV = isZooming ? startFOV : targetFOV;
        var toFOV = isZooming ? targetFOV : startFOV;

        currentFOV = Mathf.Lerp(fromFOV, toFOV, t);

        linkedCamera.Lens.FieldOfView = currentFOV;
    }
}