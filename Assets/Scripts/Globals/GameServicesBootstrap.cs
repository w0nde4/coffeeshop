using Player;
using UnityEngine;

public class GameServicesBootstrap : MonoBehaviour
{ 
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInventory playerInventory;

    private void Awake()
    {
        GameServices.RegisterPlayer(playerController, playerInventory);
    }
}