using Globals;
using NPC;
using Player;

public static class GameServices
{
    public static PlayerController PlayerController { get; private set; }
    public static PlayerInventory PlayerInventory { get; private set; }
    public static DialogueManager DialogueManager { get; private set; }

    public static void RegisterPlayer(PlayerController playerController, PlayerInventory playerInventory)
    {
        PlayerController = playerController;
        PlayerInventory = playerInventory;
    }

    public static void RegisterDialogueManager(DialogueManager dialogueManager)
    {
        DialogueManager = dialogueManager;
    }
}