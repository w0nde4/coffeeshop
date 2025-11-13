using Coffee;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Lid : Interactible
{
    [Header("Texts")] 
    [SerializeField] private string pickUpText = "Взять крышку";
    
    private void Start()
    {
        interactText = pickUpText;
        displayActionButton = true;
    }

    public override void Interact(GameObject interactor)
    {
        var inventory = GameServices.PlayerInventory;
        if (!inventory) return;

        if (inventory.CurrentItem == null)
        {
            if (inventory.AddItemToInventory(this))
            {
                Debug.Log("Вы взяли крышку.");
            }
            return;
        }

        Debug.Log("У вас уже есть предмет в руках. Выбросите его сначала.");
    }
    
    public void AttachToCup(CoffeeCup cup)
    {
        if (!cup) return;
        
        Debug.Log("Крышка прикреплена к чашке.");

        Destroy(gameObject);
    }
}