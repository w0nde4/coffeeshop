using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    [RequireComponent(typeof(Animator))]
    public class NpcAnimator : MonoBehaviour
    {
        private Animator _animator;
        private NavMeshAgent _agent;
        
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        [SerializeField] private float smoothing = 0.1f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            UpdateMovementAnimation();
        }

        private void UpdateMovementAnimation()
        {
            var normalizedSpeed = _agent.velocity.magnitude / _agent.speed;
            
            _animator.SetFloat(SpeedHash, normalizedSpeed, smoothing, Time.deltaTime);
        }
    }
}