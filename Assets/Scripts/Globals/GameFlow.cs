using System.Collections;
using NPC;
using UnityEngine;
using UnityEngine.AI;

namespace Globals
{
    public class GameFlow : MonoBehaviour
    {
        [Header("NPC References")]
        [SerializeField] private NpcFacade guard;
        [SerializeField] private NpcFacade hood;

        [Header("Delays")]
        [SerializeField] private float delayBeforeHoodPass = 2f;
        [SerializeField] private float delayAfterGuardLeaves = 3f;

        private NpcMovement _guardMove;
        private NpcMovement _hoodMove;

        private void Start()
        {
            _guardMove = guard.GetComponent<NpcMovement>();
            _hoodMove = hood.GetComponent<NpcMovement>();

            _guardMove.OnRouteComplete += OnGuardRouteComplete;
            _hoodMove.OnRouteComplete += OnHoodRouteComplete;

            StartCoroutine(StartScene());
        }

        private IEnumerator StartScene()
        {
            // === Guard заходит ===
            guard.SpawnNpc(guard.gameObject);
            _guardMove.EnterCafe();
            Debug.Log("[GameFlow] Guard заходит в кафе...");

            // Через время — худи проходит мимо
            yield return new WaitForSeconds(delayBeforeHoodPass);
            hood.SpawnNpc(hood.gameObject);
            _hoodMove.PassBy();
            Debug.Log("[GameFlow] Hood проходит мимо...");
        }

        private void OnGuardRouteComplete(RouteType route)
        {
            if (route == RouteType.Exiting)
            {
                Debug.Log("[GameFlow] Guard вышел, готовим сцену для Hood...");
                StartCoroutine(EnterHoodAfterDelay());
            }
        }

        private IEnumerator EnterHoodAfterDelay()
        {
            yield return new WaitForSeconds(delayAfterGuardLeaves);
            _hoodMove.EnterCafe();
            Debug.Log("[GameFlow] Hood входит в кафе.");
        }

        private void OnHoodRouteComplete(RouteType route)
        {
            if (route == RouteType.Entering)
                Debug.Log("[GameFlow] Hood вошёл в кафе (можно начинать диалог).");
        }
    }
}