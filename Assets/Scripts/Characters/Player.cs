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
        private float speed = 5f;

        [SerializeField]
        private float health = 100f;

        [SerializeField]
        private float energy = 100f;

        [SerializeField]
        private bool isPlayer;

        private EnergyLabel energyLabel;

        public PlayerInputHandler PlayerInputHandler { get; set; }

        private Animator _animator;
        private Weapon _weapon;
        private float _deathAnimationTime;

        private static readonly int AttackID = Animator.StringToHash("attack");
        private static readonly int DeathID = Animator.StringToHash("death");

        private void Awake()
        {
            if (isPlayer)
            {
                PlayerInputHandler = GetComponent<PlayerInputHandler>();
                energyLabel = FindObjectOfType<EnergyLabel>();
            }

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
            if (isPlayer && PlayerInputHandler.Fire && CanAttack())
            {
                _animator.SetTrigger(AttackID);
                _weapon.Fire();
            }

            if (isPlayer && PlayerInputHandler.ResetEnergy)
            {
                energy = 100f;
                UpdateEnergyLabel();
            }

            if (isPlayer && PlayerInputHandler.NextTurn && PlayerInputHandler.canDebouncedAction())
            {
                NextTurn();
            }

            _weapon.IncrementTimer(Time.deltaTime);
        }

        private bool CanAttack()
        {
            return PlayerInputHandler.HorizontalMovement == 0 && !_weapon.OnCooldown();
        }

        public float GetHealth()
        {
            return health;
        }

        public void UpdateEnergyLabel()
        {
            if (isPlayer)
            {
                energyLabel.UpdateEnergy(energy);
            }
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
            PlayerInputHandler.lastDebouncedActionDTime = Time.time;
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        public bool CanMove()
        {
            return energy > 0;
        }
    }
}
