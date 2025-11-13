using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Item settings")]
        [SerializeField] private GameObject itemHolder;
        [SerializeField] private KeyCode dropKey = KeyCode.Q;
        [SerializeField] private float forwardDropMultiplier = 1f;
        [SerializeField] private float upDropMultiplier = 0.3f;

        public GameObject CurrentItem { get; private set; }

        private void Update()
        {
            if (Input.GetKeyDown(dropKey) && CurrentItem)
            {
                DropCurrentItem();
            }
        }

        public bool AddItemToInventory(Interactible itemToAdd)
        {
            if (CurrentItem)
            {   
                Debug.Log("У вас уже есть предмет в руках.");
                return false;
            }
        
            CurrentItem = itemToAdd.gameObject;
            CurrentItem.transform.SetParent(itemHolder.transform);
            CurrentItem.transform.localPosition = Vector3.zero;
            CurrentItem.transform.localRotation = Quaternion.identity;
        
            if(CurrentItem.TryGetComponent(out Collider itemCollider))
                itemCollider.enabled = false;

            if(CurrentItem.TryGetComponent(out Rigidbody itemRigidbody))
                itemRigidbody.isKinematic = true;

            return true;
        }

        private void DropCurrentItem()
        {
            if (!CurrentItem) return;
        
            CurrentItem.transform.SetParent(null);
            CurrentItem.transform.position = transform.position + 
                                             transform.forward * forwardDropMultiplier + 
                                             Vector3.up * upDropMultiplier;
        
            if (CurrentItem.TryGetComponent(out Collider itemCollider))
                itemCollider.enabled = true;

            if (CurrentItem.TryGetComponent(out Rigidbody itemRigidbody))
                itemRigidbody.isKinematic = false;

            if (CurrentItem.TryGetComponent(out Interactible itemInteractable))
                itemInteractable.enabled = true;

            CurrentItem.SetActive(true);
            CurrentItem = null;
        
            Debug.Log("Предмет выброшен");
        }

        public void RemoveCurrentItem()
        {
            if (!CurrentItem) return;
            CurrentItem.transform.SetParent(null);
            CurrentItem = null;
        }
    }
}