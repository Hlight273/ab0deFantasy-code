namespace HFantasy.Script.UI.Interfaces
{
    public interface IUIAnimatable
    {
        void PlayEnterAnimation(float delay = 0f);
        void PlayExitAnimation(float delay = 0f);
        bool IsAnimating { get; }
    }
}