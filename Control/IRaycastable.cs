using static RPG.Control.PlayerController;

namespace RPG.Control
{

    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController playerController);
        CursorType GetCursorType();
    }
}
