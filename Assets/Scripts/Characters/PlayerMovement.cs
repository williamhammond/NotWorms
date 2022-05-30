using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

namespace Characters
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField]
        private float speed = 5f;

        [SerializeField]
        private LayerMask terrainLayerMask;

        public static event Action<int> ServerMovementStarted;
        public static event Action<int> ServerMovementEnded;
        public static event Action<int> ServerPlayerJumped;

        private PlayerInput _playerInput;
        private Rigidbody2D _body;
        private Collider2D _collider;
        private Animator _animator;

        private bool _isJumping = false;
        private bool _isMoving = false;
        private Transform _groundChecker;

        private static readonly int IsRunningID = Animator.StringToHash("isRunning");
        private static readonly int IsJumpingID = Animator.StringToHash("isJumping");

        public void Awake()
        {
            _animator = GetComponent<Animator>();
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnDisable()
        {
            _isJumping = false;
            _animator.SetBool(IsJumpingID, _isJumping);
            _animator.SetBool(IsRunningID, false);
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            float movement = _playerInput.actions["Player/Movement"].ReadValue<float>();
            if (_isMoving != (Mathf.Abs(movement) > 0.01f))
            {
                if (Mathf.Abs(movement) > 0.01f)
                {
                    _isMoving = true;
                    CmdMovementStarted();
                }
                else
                {
                    _isMoving = false;
                    CmdMovementEnded();
                }
            }
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            float movement = _playerInput.actions["Player/Movement"].ReadValue<float>();
            ClientHandleMove(movement);

            _isJumping = !Physics2D.BoxCast(
                _collider.bounds.center,
                _collider.bounds.size,
                0f,
                Vector2.down,
                0.1f,
                terrainLayerMask
            );
            _animator.SetBool(IsJumpingID, _isJumping);
        }

        #region Server

        [Server]
        public override void OnStopServer()
        {
            _isJumping = false;
            _animator.SetBool(IsJumpingID, _isJumping);
            _animator.SetBool(IsRunningID, false);
        }

        [Command]
        private void CmdMovementStarted()
        {
            ServerMovementStarted?.Invoke(connectionToClient.connectionId);
        }

        [Command]
        private void CmdMovementEnded()
        {
            ServerMovementEnded?.Invoke(connectionToClient.connectionId);
        }

        [Command]
        private void CmdJump()
        {
            ServerPlayerJumped?.Invoke(connectionToClient.connectionId);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            _playerInput.actions["Player/Jump"].performed += ClientHandleJump;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority)
            {
                return;
            }

            _playerInput.actions["Player/Jump"].performed -= ClientHandleJump;
        }

        private void ClientHandleMove(float movement)
        {
            if (!enabled)
            {
                return;
            }
            _body.velocity = new Vector2(movement * speed, _body.velocity.y);
            Vector2 moveDirection = _body.velocity;

            bool isMoving = Mathf.Abs(moveDirection.x) > 0.01f;
            if (isMoving)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }

            _animator.SetBool(IsRunningID, isMoving);
        }

        private void ClientHandleJump(InputAction.CallbackContext obj)
        {
            if (!enabled)
            {
                return;
            }
            if (!_isJumping)
            {
                _body.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
                _isJumping = true;
                _animator.SetBool(IsJumpingID, _isJumping);
                CmdJump();
            }
        }

        #endregion
    }
}
