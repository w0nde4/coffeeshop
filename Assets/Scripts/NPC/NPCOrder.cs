using UnityEngine;

namespace NPC
{
    public class NpcOrder : MonoBehaviour
    {
        [SerializeField] private string expectedItemTag = "CoffeeReady";

        public bool IsOrderReceived { get; private set; } = false;
        public bool IsWaitingForOrder { get; private set; } = false;

        public void WaitForOrder()
        {
            if(IsWaitingForOrder) return;
            IsWaitingForOrder = true;
            IsOrderReceived = false; 
        }
        
        public bool TryGiveItem(GameObject item)
        {
            if(!IsWaitingForOrder) return false;
            if(!item.CompareTag(expectedItemTag)) return false;

            IsWaitingForOrder = false;
            IsOrderReceived = true;
            return true;
        }
    }
}