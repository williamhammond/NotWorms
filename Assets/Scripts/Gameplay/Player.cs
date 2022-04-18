using System;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        public IPlayerInput PlayerInput { get; set; }

        private Rigidbody2D _body;
        private Animator _animator;

        private bool _isJumping = true;

        private static readonly int IsRunningID = Animator.StringToHash("isRunning");
        private static readonly int IsJumpingID = Animator.StringToHash("isJumping");

        private void Awake()
        {
            PlayerInput = new PlayerInput();
            _body = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            speed = 1f;
        }

        void Update()
        {
            MoveHorizontal();
            HandleOrientation();
            _animator.SetBool(IsJumpingID, _isJumping);

            if (PlayerInput.Jump && !_isJumping)
            {
                Jump();
            }
        }

        void MoveHorizontal()
        {
            _body.velocity = new Vector2(PlayerInput.Horizontal * speed, _body.velocity.y);
            _animator.SetBool(IsRunningID, PlayerInput.Horizontal != 0);
        }

        void Jump()
        {
            _body.velocity = new Vector2(_body.velocity.x, speed);
            _isJumping = true;
        }

        private void HandleOrientation()
        {
            if (PlayerInput.Horizontal >= 0f)
            {
                transform.localScale = Vector3.one;
            }
            else if (PlayerInput.Horizontal < 0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.CompareTag("Ground"))
            {
                _isJumping = false;
            }
        }
    }
}
