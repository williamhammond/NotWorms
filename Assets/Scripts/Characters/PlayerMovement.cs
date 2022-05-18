using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

namespace Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5f;

        private PlayerInput _playerInput;
        private Rigidbody2D _body;
        private Animator _animator;

        private bool _isJumping = false;

        private static readonly int IsRunningID = Animator.StringToHash("isRunning");
        private static readonly int IsJumpingID = Animator.StringToHash("isJumping");

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _animator = GetComponent<Animator>();
            _body = GetComponent<Rigidbody2D>();

            _playerInput.actions["Player/Jump"].performed += HandleJump;
        }

        private void FixedUpdate()
        {
            HandleMovement(_playerInput.actions["Player/Movement"].ReadValue<float>());
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _animator.SetBool(IsJumpingID, false);
        }

        private void HandleMovement(float movement)
        {
            _body.velocity = new Vector2(movement * speed, _body.velocity.y);

            if (movement > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            else if (movement < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            _animator.SetBool(IsRunningID, Mathf.Abs(_body.velocity.x) > 0.1f);
        }

        private void HandleJump(InputAction.CallbackContext context)
        {
            if (!_isJumping)
            {
                _body.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
                _animator.SetBool(IsJumpingID, true);
            }
        }
    }
}
