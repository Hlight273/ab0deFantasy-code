
using HFantasy.Script.Entity.Player;

namespace HFantasy.Script.Player.Interaction
{
    public interface IInteractable
    {
        string Description { get; }
        void Interact(PlayerEntity playerEntity);
    }
}
