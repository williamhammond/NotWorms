using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        public static Action<Type> MovementStateEntered;
        public static Action<Type> MovementStateExited;

        [SerializeField]
        private float speed;

        [SerializeField]
        private LayerMask floorLayerMask;

        private Animator _animator;
        private PlayerInput _playerInput;
        private Rigidbody2D _body;
        private Collider2D _collider;

        private StateMachine _stateMachine;

        private static readonly int IsRunningID = Animator.StringToHash("isRunning");
        private static readonly int IsJumpingID = Animator.StringToHash("isJumping");

        private MoveState _moveState;
        private IdleState _idleState;
        private JumpState _jumpState;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();

            _playerInput = GetComponentInChildren<PlayerInput>();
            _animator = GetComponent<Animator>();

            _moveState = new MoveState(this);
            _idleState = new IdleState(this);
            _jumpState = new JumpState(this);

            Dictionary<IState, HashSet<IState>> transitions = new Dictionary<
                IState,
                HashSet<IState>
            >()
            {
                {
                    _idleState,
                    new HashSet<IState>() { _jumpState, _moveState }
                },
                {
                    _moveState,
                    new HashSet<IState>() { _idleState, _jumpState }
                },
                {
                    _jumpState,
                    new HashSet<IState>() { _idleState }
                }
            };
            _stateMachine = new StateMachine(transitions, _idleState);
        }

        void Update()
        {
            if (_playerInput.Jump)
            {
                _stateMachine.ChangeState(_jumpState);
                _stateMachine.Update();
            }
            else if (Mathf.Abs(_playerInput.HorizontalMovement) > 0.01f)
            {
                _stateMachine.ChangeState(_moveState);
                _stateMachine.Update();
            }
            else
            {
                _stateMachine.ChangeState(_idleState);
                _stateMachine.Update();
            }
        }

        private interface IState
        {
            public void Enter();
            public void Execute();
            public void Exit();

            public bool ReadyToTransition();
        }

        private class StateMachine
        {
            private IState _currentState;
            private readonly Dictionary<IState, HashSet<IState>> _transitions;

            public StateMachine(
                Dictionary<IState, HashSet<IState>> transitions,
                IState initialState
            )
            {
                this._transitions = transitions;
                this._currentState = initialState;
            }

            public IState GetState()
            {
                return _currentState;
            }

            public void ChangeState(IState newState)
            {
                if (!_transitions.ContainsKey(newState))
                {
                    throw new InvalidOperationException(
                        $"{newState} not a valid state for state machine {_transitions}"
                    );
                }
                if (!_transitions[_currentState].Contains(newState))
                {
                    return;
                }
                if (!_currentState.ReadyToTransition())
                {
                    return;
                }

                _currentState.Exit();
                _currentState = newState;
                _currentState.Enter();
            }

            public void Update()
            {
                _currentState.Execute();
            }
        }

        public class MoveState : IState
        {
            private readonly PlayerMovement _playerMovement;

            public MoveState(PlayerMovement playerMovement)
            {
                this._playerMovement = playerMovement;
            }

            public void Enter()
            {
                MovementStateEntered?.Invoke(typeof(MoveState));
                _playerMovement._animator.SetBool(IsRunningID, true);
            }

            public void Execute()
            {
                _playerMovement._body.velocity = new Vector2(
                    _playerMovement._playerInput.HorizontalMovement * _playerMovement.speed,
                    _playerMovement._body.velocity.y
                );
                if (_playerMovement._playerInput.HorizontalMovement > 0.01f)
                {
                    _playerMovement.transform.localScale = Vector3.one;
                }
                else if (_playerMovement._playerInput.HorizontalMovement < -0.01f)
                {
                    _playerMovement.transform.localScale = new Vector3(-1, 1, 1);
                }
            }

            public void Exit()
            {
                _playerMovement._body.velocity = new Vector2(0, 0);
                _playerMovement._animator.SetBool(IsRunningID, false);
                MovementStateExited?.Invoke(typeof(MoveState));
            }

            public bool ReadyToTransition()
            {
                return true;
            }
        }

        public class JumpState : IState
        {
            private readonly PlayerMovement _playerMovement;

            public JumpState(PlayerMovement playerMovement)
            {
                this._playerMovement = playerMovement;
            }

            public void Enter()
            {
                MovementStateEntered?.Invoke(typeof(JumpState));
                _playerMovement._animator.SetBool(IsJumpingID, true);
            }

            public void Execute()
            {
                _playerMovement._body.velocity = new Vector2(
                    _playerMovement._body.velocity.x,
                    _playerMovement.speed
                );
            }

            public void Exit()
            {
                _playerMovement._animator.SetBool(IsJumpingID, false);
                MovementStateEntered?.Invoke(typeof(JumpState));
            }

            public bool ReadyToTransition()
            {
                return _playerMovement.IsGrounded();
            }
        }

        public class IdleState : IState
        {
            private PlayerMovement _playerMovement;

            public IdleState(PlayerMovement playerMovement)
            {
                this._playerMovement = playerMovement;
            }

            public void Enter()
            {
                MovementStateEntered?.Invoke(typeof(IdleState));
            }

            public void Execute() { }

            public void Exit()
            {
                MovementStateExited?.Invoke(typeof(IdleState));
            }

            public bool ReadyToTransition()
            {
                return true;
            }
        }

        private bool IsGrounded()
        {
            float tolerance = 0.1f;
            return Physics.Raycast(
                transform.position,
                Vector2.down,
                _collider.bounds.extents.y + tolerance,
                floorLayerMask
            );
        }
    }
}
