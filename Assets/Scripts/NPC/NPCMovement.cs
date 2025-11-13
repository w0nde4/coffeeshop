using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPC
{
    public enum RouteType
    {
        None,
        Entering,
        Exiting,
        PassBy
    }
    
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Collider))]
    public class NpcMovement : MonoBehaviour
    {
        [Header("Movement route")]
        [SerializeField] private List<Transform> enteringPoints = new();
        [SerializeField] private List<Transform> exitingPoints = new();
        [SerializeField] private List<Transform> passByPoints = new();

        [Header("Waiting settings")]
        [SerializeField] private float minWaitTime = 1f;
        [SerializeField] private float maxWaitTime = 3f;

        [Header("Patrol or single action")]
        [SerializeField] private bool isLooped = false;

        [Header("Following")]
        [SerializeField] private float followDistance = 2f;
        [SerializeField] private float rotationSpeed = 5f;
        
        private NavMeshAgent _agent;
        private Coroutine _moveRoutine;
        private int _currentIndex = 0;
        private List<Transform> _currentRoute;
        private Transform _target;
        private RouteType _currentRouteType = RouteType.None;
        
        public event Action<RouteType> OnRouteComplete; // <-- теперь с контекстом
        public RouteType CurrentRouteType => _currentRouteType;
        public Transform SpawnPoint => enteringPoints.Count > 0 ? enteringPoints[0] : transform;

        private void Awake() => _agent = GetComponent<NavMeshAgent>();

        #region --- Route ---

        public void StartRoute(List<Transform> points, RouteType type, bool looped = false)
        {
            if (points == null || points.Count == 0)
            {
                Debug.LogWarning($"{name}: Пустой маршрут для {type}!");
                return;
            }

            StopAllMovement();
            _currentRoute = points;
            _currentRouteType = type;
            isLooped = looped;
            _moveRoutine = StartCoroutine(MoveAlongRoute());
        }

        private IEnumerator MoveAlongRoute()
        {
            _currentIndex = 0;
            _agent.isStopped = false;

            while (_currentIndex < _currentRoute.Count)
            {
                _agent.SetDestination(_currentRoute[_currentIndex].position);

                yield return new WaitUntil(ReachedDestination);

                var waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(waitTime);

                _currentIndex++;

                if (_currentIndex >= _currentRoute.Count && isLooped)
                    _currentIndex = 0;
            }

            _agent.isStopped = true;
            _moveRoutine = null;
            
            Debug.Log($"{name}: маршрут {_currentRouteType} завершён.");
            OnRouteComplete?.Invoke(_currentRouteType);
            _currentRouteType = RouteType.None;
        }

        public bool ReachedDestination()
        {
            if (_agent.pathPending) return false;
            return _agent.remainingDistance <= _agent.stoppingDistance;
        }

        #endregion

        #region --- Handlers ---

        private void StopAllMovement()
        {
            if (_moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
                _moveRoutine = null;
            }
            _agent.isStopped = true;
            _target = null;
        }

        public void Resume() => _agent.isStopped = false;
        public void Stop() => _agent.isStopped = true;

        #endregion

        #region --- Follow player ---

        public void FollowTarget(Transform target)
        {
            _target = target;
            StopAllMovement();
            StartCoroutine(FollowRoutine());
        }

        private IEnumerator FollowRoutine()
        {
            _agent.isStopped = false;

            while (_target)
            {
                var distance = Vector3.Distance(transform.position, _target.position);

                if (distance > followDistance)
                    _agent.SetDestination(_target.position);
                else
                    _agent.isStopped = true;

                LookAtTarget(_target);
                yield return null;
            }

            _agent.isStopped = true;
        }

        #endregion

        #region --- Look at player ---

        public void LookAtTarget(Transform target)
        {
            if (!target) return;

            var direction = (target.position - transform.position).normalized;
            direction.y = 0;

            if (direction == Vector3.zero) return;

            var lookRotation = Quaternion.LookRotation(direction);
            
            _agent.updateRotation = false;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            _agent.updateRotation = true;
        }

        #endregion

        #region --- Scenarios ---

        public void EnterCafe() => StartRoute(enteringPoints, RouteType.Entering);
        public void ExitCafe() => StartRoute(exitingPoints, RouteType.Exiting);
        public void PassBy() => StartRoute(passByPoints, RouteType.PassBy);

        #endregion
    }  
}
