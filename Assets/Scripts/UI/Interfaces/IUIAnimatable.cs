namespace HFantasy.Script.UI.Interfaces
{
    public interface IUIAnimatable
    {
        float EnterDelay { get; }
        float ExitDelay { get; }
        void PlayEnterAnimation();
        void PlayExitAnimation();
        bool IsAnimating { get; }
    }
}