using System;
using UnityEngine;
using Combat;
using UI;

namespace Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, IDamagable
    {
        public static event Action<Player> PlayerSpawned;
        public static event Action<Player> PlayerDespawned;
        public static event Action EndTurn;

        [SerializeField]
        private float health = 100f;

        [SerializeField]
        private bool isPlayer;
        private PlayerInput _playerInput;
        private Animator _animator;
        private Weapon _weapon;
        private float _deathAnimationTime;

        private bool _isJumping = false;

        private static readonly int AttackID = Animator.StringToHash("attack");
        private static readonly int DeathID = Animator.StringToHash("death");

        private void Awake()
        {
            _playerInput = GetComponentInChildren<PlayerInput>();
            _animator = GetComponent<Animator>();
            _weapon = GetComponent<Weapon>();

            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (DeathID == Animator.StringToHash(clip.name))
                {
                    _deathAnimationTime = clip.length;
                }
            }
        }

        private void Start()
        {
            PlayerSpawned?.Invoke(this);
        }

        private void OnDestroy()
        {
            PlayerDespawned?.Invoke(this);
        }

        void Update()
        {
            if (_playerInput.Fire && CanAttack())
            {
                _animator.SetTrigger(AttackID);
                _weapon.Fire();
            }

            if (isPlayer && _playerInput.NextTurn && _playerInput.canDebouncedAction())
            {
                NextTurn();
            }

            _weapon.IncrementTimer(Time.deltaTime);
        }

        private bool CanAttack()
        {
            return _playerInput.HorizontalMovement == 0 && !_isJumping && !_weapon.OnCooldown();
        }

        public float GetHealth()
        {
            return health;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            Debug.Log($"Player health is {health}");
            if (!IsAlive())
            {
                _animator.SetTrigger(DeathID);
                Destroy(gameObject, _deathAnimationTime);
            }
        }

        void NextTurn()
        {
            EndTurn?.Invoke();
            this._playerInput.lastDebouncedActionDTime = Time.time;
        }

        public bool IsAlive()
        {
            return health > 0;
        }
    }
}
