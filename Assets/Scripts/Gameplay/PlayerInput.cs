using UnityEngine;

namespace Gameplay
{
    public class PlayerInput : IPlayerInput
    {
        public float Horizontal => Input.GetAxis("Horizontal");
        public bool Jump => Input.GetKey(KeyCode.Space);
        public bool Fire => Input.GetMouseButton(0);

        public bool ResetEnergy => Input.GetKey(KeyCode.X);
        public bool NextTurn => Input.GetKey(KeyCode.R);
    }
}
