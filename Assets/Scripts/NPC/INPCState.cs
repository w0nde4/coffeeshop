namespace NPC
{
    public interface INpcState
    {
        NpcState State { get; }
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}