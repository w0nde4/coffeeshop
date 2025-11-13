using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepDelay = 0.5f;
    [SerializeField] private float volume = 0.5f;
    
    private PlayerController _playerController;
    private AudioSource _audioSource;
    private float _timer;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();
        
        _audioSource.volume = volume;
        _audioSource.spatialBlend = 1f;
    }

    private void Update()
    {
        if(!_playerController.IsMoving || footstepClips.Length == 0) return;
        
        _timer += Time.deltaTime;
        
        if (_timer > 0) return;
        
        PlayStep();
        _timer = footstepDelay;
    }

    private void PlayStep()
    {
        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        _audioSource.PlayOneShot(clip);
    }
}