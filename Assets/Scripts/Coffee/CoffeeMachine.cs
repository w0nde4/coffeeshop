using System.Collections;
using Player;
using UnityEngine;

namespace Coffee
{
    public class CoffeeMachine : Interactible
    {
        [Header("Coffee Machine Settings")] 
        [SerializeField] private Transform cupPlacement;
        [SerializeField] private string needCupText = "Нужна чашка";
        [SerializeField] private string placeCupText = "Поставить чашку";
        [SerializeField] private string brewText = "Начать приготовление";
        [SerializeField] private bool isActive = true;

        [Header("Optional Visuals")]
        [SerializeField] private ParticleSystem coffeePourEffect;
        [SerializeField] private AudioClip pourSound;
    
        private AudioSource _audioSource;
        private CoffeeCup _placedCup;
        private bool _isBrewing;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            displayActionButton = true;
            interactText = placeCupText;
        }

        public override void Interact(GameObject interactor)
        {
            if(!isActive || _isBrewing) return;

            var inventory = GameServices.PlayerInventory;
            if (!inventory) return;

            if (!_placedCup) TryPlaceCup(inventory);
            else TryRetrieveCup(inventory);
        }
    
        private void TryPlaceCup(PlayerInventory inventory)
        {
            if (!inventory.CurrentItem || 
                !inventory.CurrentItem.TryGetComponent(out CoffeeCup cup))
            {
                ShowTemporaryText(needCupText);
                return;
            }

            if (cup.CurrentState != CoffeeCup.CupState.Empty)
            {
                Debug.Log("Можно ставить только пустую чашку.");
                return;
            }

            _placedCup = cup;
            inventory.RemoveCurrentItem();

            cup.transform.SetParent(cupPlacement);
            cup.transform.localPosition = Vector3.zero;
            cup.transform.localRotation = Quaternion.identity;

            cup.GetComponent<Collider>().enabled = false;
            cup.GetComponent<Rigidbody>().isKinematic = true;

            interactText = brewText;
        }

        private void TryRetrieveCup(PlayerInventory inventory)
        {
            switch (_placedCup.CurrentState)
            {
                case CoffeeCup.CupState.Filled when 
                    !inventory.AddItemToInventory(_placedCup.GetComponent<Interactible>()):
                    return;
                case CoffeeCup.CupState.Filled:
                    _placedCup = null;
                    interactText = placeCupText;
                    return;
                case CoffeeCup.CupState.Empty:
                    StartCoroutine(BrewRoutine());
                    break;
            }
        }

        private IEnumerator BrewRoutine()
        {
            _isBrewing = true;
            displayActionButton = false;
            interactText = "Готовим кофе...";

            coffeePourEffect?.Play();
            _audioSource?.PlayOneShot(pourSound);

            yield return new WaitForSeconds(pourSound.length);

            _placedCup.FillCup();

            coffeePourEffect?.Stop();
            displayActionButton = true;
            interactText = "Забрать кофе";

            _isBrewing = false;
        }

        private void ShowTemporaryText(string text)
        {
            interactText = text;
            CancelInvoke(nameof(ResetText));
            Invoke(nameof(ResetText), 1.5f);
        }

        private void ResetText()
        {
            interactText = placeCupText;
        }
    }
}