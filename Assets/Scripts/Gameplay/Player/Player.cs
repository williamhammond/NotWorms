using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        // Visible for testing
        public IPlayerInput PlayerInput { get; set; }

        private Rigidbody2D _body;

        private void Awake()
        {
            PlayerInput = new PlayerInput();
            _body = GetComponent<Rigidbody2D>();
            speed = 1f;
        }

        void Update()
        {
            MoveHorizontal();
            if (PlayerInput.IsJumping)
            {
                Jump();
            }
        }

        void MoveHorizontal()
        {
            _body.velocity = new Vector2(PlayerInput.Horizontal * speed, _body.velocity.y);
        }

        void Jump()
        {
            _body.velocity = new Vector2(_body.velocity.x, speed);
        }
    }
}
