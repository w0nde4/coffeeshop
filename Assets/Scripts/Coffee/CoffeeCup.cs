using System;
using NPC;
using Player;
using UnityEngine;

namespace Coffee
{
    [RequireComponent(typeof(Collider))]
    public class CoffeeCup : Interactible
    {
        public enum CupState { Empty, Filled, Closed }

        [Header("Visual children (assign in inspector)")] 
        [SerializeField] private GameObject lid;
        [SerializeField] private GameObject cup;
        [SerializeField] private GameObject coffee;

        [Header("Texts")] 
        [SerializeField] private string takeEmptyText = "Взять чашку";
        [SerializeField] private string takeFilledText = "Взять кофе";
        [SerializeField] private string takeClosedText = "Взять кофе с крышкой";
        [SerializeField] private string putLidText = "Надеть крышку";
        [SerializeField] private string serveText = "Отдать посетителю";

        [Header("Serve settings")]
        [SerializeField] private string readyCoffeeTag = "CoffeeReady";
            
        private bool _canBeServed;
        public CupState CurrentState { get; private set; } = CupState.Empty;
        
        public event Action<CoffeeCup> OnServed;
    
        private void Start()
        {
            _canBeServed = false;
            
            UpdateVisuals();
            UpdateInteractText();
        }

        #region Interact logic
        public override void Interact(GameObject interactor)
        {
            if (interactor && interactor.TryGetComponent(out NpcFacade npc) && _canBeServed)
            {
                if (CurrentState is CupState.Closed)
                {
                    ServeTo(npc);
                    return;
                }
            }

            var inventory = GameServices.PlayerInventory;
            if (!inventory) return;

            if (!inventory.CurrentItem)
            {
                if (inventory.AddItemToInventory(this))
                {
                    Debug.Log("Вы взяли чашку.");
                }
                return;
            }

            if (inventory.CurrentItem.TryGetComponent(out Lid lid))
            {
                TryAttachLid(lid, inventory);
                return;
            }

            Debug.Log("Нет подходящего действия с чашкой.");
        }

        #endregion

        #region Public API

        public void FillCup()
        {
            if (CurrentState != CupState.Empty) return;
            CurrentState = CupState.Filled;
            UpdateVisuals();
            UpdateInteractText();
        }

        private void AttachLid(Lid lidComp)
        {
            if (!lidComp) return;

            if (lidComp)
            {
                lid.SetActive(true);
            }

            CurrentState = CupState.Closed;
            UpdateVisuals();
            UpdateInteractText();
            
            tag = readyCoffeeTag;
            _canBeServed = true;

            lidComp.AttachToCup(this);
        }

        private void ServeTo(NpcFacade npc)
        {
            if (!npc) return;
            if (!_canBeServed) return;
            if (CurrentState != CupState.Closed) return;

            Debug.Log($"Чашка подана посетителю {npc.NpcName}.");

            OnServed?.Invoke(this);

            Destroy(gameObject);

            // При желании: уведомить DialogueManager или квест-систему через GameServices/Events (в будущем)
        }

        #endregion

        #region Helpers

        private void TryAttachLid(Lid lidComp, PlayerInventory inventory)
        {
            if (!lidComp || !inventory) return;

            if (CurrentState != CupState.Filled)
            {
                Debug.Log("Сначала налейте кофе!");
                return;
            }

            inventory.RemoveCurrentItem();

            AttachLid(lidComp);
            Debug.Log("Крышка надета на кружку.");
        }

        private void UpdateVisuals()
        {
            if (coffee)
                coffee.SetActive(CurrentState is CupState.Filled or CupState.Closed);

            if (lid)
                lid.SetActive(CurrentState == CupState.Closed);

            if (cup)
                cup.SetActive(true);
        }

        private void UpdateInteractText()
        {
            interactText = CurrentState switch
            {
                CupState.Empty => takeEmptyText,
                CupState.Filled => takeFilledText,
                CupState.Closed => takeClosedText,
                _ => interactText
            };
        }

        #endregion
    }
}