using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerInput : IPlayerInput
    {
        public float Horizontal => Input.GetAxis("Horizontal");
        public bool IsJumping => Input.GetKey(KeyCode.Space);
    }
}
