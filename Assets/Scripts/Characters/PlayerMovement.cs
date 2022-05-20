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
        private bool _isRocketing = false;

        private static readonly int IsRunningID = Animator.StringToHash("isRunning");
        private static readonly int IsJumpingID = Animator.StringToHash("isJumping");
        private static readonly int IsRocketingID = Animator.StringToHash("isRocketing");

        private void Awake()
        {
            _animator = GetComponentInParent<Animator>();
            _body = GetComponentInParent<Rigidbody2D>();

            _playerInput = GetComponentInParent<PlayerInput>();
            _playerInput.actions["Player/Jump"].performed += HandleJump;
            _playerInput.actions["Player/Rocket"].performed += HandleRocket;
        }

        private void OnDestroy()
        {
            _playerInput.actions["Player/Jump"].performed -= HandleJump;
        }

        private void FixedUpdate()
        {
            HandleMovement(_playerInput.actions["Player/Movement"].ReadValue<float>());
        }

        private void OnDisable()
        {
            _animator.SetBool(IsJumpingID, false);
            _animator.SetBool(IsRunningID, false);
        }

        private void HandleMovement(float movement)
        {
            _body.velocity = new Vector2(movement * speed, _body.velocity.y);

            if (movement > 0.01f)
            {
                _body.transform.localScale = Vector3.one;
            }
            else if (movement < -0.01f)
            {
                _body.transform.localScale = new Vector3(-1, 1, 1);
            }
            _animator.SetBool(IsRunningID, Mathf.Abs(_body.velocity.x) > 0.1f);
        }

        private void HandleRocket(InputAction.CallbackContext context)
        {
            if (!_isRocketing)
            {
                Debug.Log("rocketing");
                _isRocketing = true;

                //https://stackoverflow.com/questions/34250868/unity-addexplosionforce-to-2d
                Vector2 explosionDir = Vector2.up;
                int upwardsModifier = 500;
                float explosionDistance = explosionDir.magnitude;
                explosionDir.y += upwardsModifier;
                _body.AddForce(explosionDir);
                _animator.SetBool(IsRocketingID, true);
            }
        }

        private void HandleJump(InputAction.CallbackContext context)
        {
            if (!_isJumping)
            {
                _body.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
                _animator.SetBool(IsJumpingID, true);
                _isJumping = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _animator.SetBool(IsJumpingID, false);
        }
    }
}
