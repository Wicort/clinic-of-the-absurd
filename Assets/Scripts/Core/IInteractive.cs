namespace Assets.Scripts.Core
{
    public interface IInteractive
    {
        string GetInteractionPrompt();
        void Interact();
    }
}
