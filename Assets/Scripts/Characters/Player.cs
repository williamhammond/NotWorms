using System;
using UnityEngine;
using Combat;

namespace Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, IDamagable
    {
        public static event Action<Player> PlayerSpawned;
        public static event Action<Player> PlayerDespawned;

        [SerializeField]
        private float health = 100f;

        private PlayerMovement _playerMovement;
        private PlayerCombat _playerCombat;

        private Animator _animator;
        private float _deathAnimationTime;

        private static readonly int DeathID = Animator.StringToHash("death");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerCombat = GetComponentInChildren<PlayerCombat>();
            _playerMovement = GetComponentInChildren<PlayerMovement>();

            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (DeathID == Animator.StringToHash(clip.name))
                {
                    _deathAnimationTime = clip.length;
                }
            }

            PlayerEnergy.EnergyExhausted += HandleEnergyExhausted;
            PlayerEnergy.EnergyReset += HandleEnergyReset;
        }

        private void OnDestroy()
        {
            PlayerDespawned?.Invoke(this);
            PlayerEnergy.EnergyExhausted -= HandleEnergyExhausted;
            PlayerEnergy.EnergyReset -= HandleEnergyReset;
        }

        private void HandleEnergyExhausted()
        {
            if (_playerMovement)
            {
                _playerMovement.gameObject.SetActive(false);
            }
            if (_playerCombat)
            {
                _playerCombat.gameObject.SetActive(false);
            }
        }

        private void HandleEnergyReset()
        {
            if (_playerMovement)
            {
                _playerMovement.gameObject.SetActive(true);
            }
            if (_playerCombat)
            {
                _playerCombat.gameObject.SetActive(true);
            }
        }

        private void Start()
        {
            PlayerSpawned?.Invoke(this);
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

        public bool IsAlive()
        {
            return health > 0;
        }
    }
}
