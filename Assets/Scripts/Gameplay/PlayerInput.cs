using UnityEngine;

namespace Gameplay
{
    public class PlayerInput : IPlayerInput
    {
        public float Horizontal => Input.GetAxis("Horizontal");
        public bool IsJumping => Input.GetKey(KeyCode.Space);
    }
}
