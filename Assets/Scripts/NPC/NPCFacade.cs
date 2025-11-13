using System;
using System.Collections;
using System.ComponentModel;
using Globals;
using Player;
using UnityEngine;

namespace NPC
{
    [RequireComponent(typeof(NpcStateMachine))]
    [RequireComponent(typeof(NpcAnimator))]
    [RequireComponent(typeof(NpcDialogue))]
    [RequireComponent(typeof(NpcMovement))]
    [RequireComponent(typeof(NpcOrder))]
    public class NpcFacade : Interactible
    {
        [SerializeField] private string npcName = "NPC";
        [SerializeField] private float playerApproachDistance = 1.5f;
        
        private NpcStateMachine _stateMachine;
        private NpcDialogue _dialogue;
        private NpcMovement _movement;
        private NpcOrder _order;
        private Transform _player;
        
        public string NpcName => npcName;
        public NpcState CurrentState => _stateMachine.CurrentState;
        
        private void Awake()
        {
            CacheComponents();

            interactText = "Поговорить";
            displayActionButton = true;
        }

        private void Start()
        {
            _player = GameServices.PlayerController.transform;
            RegisterStates();
            
            _stateMachine.ChangeState(NpcState.Entering);
        }

        private void Update() => _stateMachine.Update();

        private void CacheComponents()
        {
            _stateMachine = GetComponent<NpcStateMachine>();
            _dialogue = GetComponent<NpcDialogue>();
            _movement = GetComponent<NpcMovement>();
            _order = GetComponent<NpcOrder>();
        }

        private void RegisterStates()
        {
            if (_stateMachine == null || _movement == null) return;
            
            _stateMachine.RegisterState(new IdleState(_stateMachine, _movement, _player));
            _stateMachine.RegisterState(new EnteringState(_stateMachine, _movement));
            _stateMachine.RegisterState(new ExitingState(_stateMachine, _movement));
            _stateMachine.RegisterState(new TalkingState(_stateMachine, _dialogue));
            _stateMachine.RegisterState(new FollowState(_stateMachine, _movement, _player));
        }

        public override void Interact(GameObject interactor)
        {
            var playerInventory = GameServices.PlayerInventory;
            var heldItem = playerInventory?.CurrentItem;

            if (_order.IsWaitingForOrder && heldItem)
            {
                HandleOrderInteraction(heldItem, playerInventory);
                return;
            }
            
            _dialogue.TryStartDialogue(interactor.transform);
        }

        private void HandleOrderInteraction(GameObject heldItem, PlayerInventory playerInventory)
        {
            if (!_order.TryGiveItem(heldItem)) return;
            
            Debug.Log($"{npcName} получил заказ!");
            playerInventory.RemoveCurrentItem();
            Destroy(heldItem.gameObject);
            _stateMachine.ChangeState(NpcState.Idle);
        }

        public void Leave() => _stateMachine.ChangeState(NpcState.Exiting);

        public void WaitForOrder() => _stateMachine.ChangeState(NpcState.Idle);

        public void SpawnNpc(GameObject npc)
        {
            if (!_movement?.SpawnPoint)
            {
                Debug.LogWarning($"{npcName}: не задана точка спавна!");
                return;
            }
            
            npc.SetActive(true);
            npc.transform.position = _movement.SpawnPoint.position;
            npc.transform.rotation = _movement.SpawnPoint.rotation;
            
            Debug.Log($"{npcName} появился в точке спавна {_movement.SpawnPoint.name}");
        }
    }
}