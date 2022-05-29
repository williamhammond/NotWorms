using System;
using System.Collections;
using UnityEngine;
using Combat;
using Mirror;

namespace Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : NetworkBehaviour, IDamagable
    {
        public static event Action<Player> ServerOnPlayerSpawned;
        public static event Action<Player> ServerOnPlayerDespawned;

        public event Action<int, int> ClientOnHealthUpdated;

        [SerializeField]
        private int maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int _currentHealth;

        private PlayerMovement _playerMovement;
        private PlayerCombat _playerCombat;

        private Animator _animator;
        private float _deathAnimationTime;

        private static readonly int DeathID = Animator.StringToHash("death");

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

        public int GetHealth()
        {
            return _currentHealth;
        }

        public bool IsAlive()
        {
            return _currentHealth > 0;
        }
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerCombat = GetComponentInChildren<PlayerCombat>();
            _playerMovement = GetComponentInChildren<PlayerMovement>();
            _currentHealth = maxHealth;

            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (DeathID == Animator.StringToHash(clip.name))
                {
                    _deathAnimationTime = clip.length;
                }
            }
        }
        

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();

            PlayerEnergy.EnergyExhausted += HandleEnergyExhausted;
            PlayerEnergy.EnergyReset += HandleEnergyReset;

            ServerOnPlayerSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            PlayerEnergy.EnergyExhausted -= HandleEnergyExhausted;
            PlayerEnergy.EnergyReset -= HandleEnergyReset;
        }

        [Server]
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            Debug.Log($"Player maxHealth is {maxHealth}");
            if (!IsAlive())
            {
                StartCoroutine(DestroyWithAnimation());
            }
        }

        IEnumerator DestroyWithAnimation()
        {
            _animator.SetTrigger(DeathID);
            yield return new WaitForSeconds(_deathAnimationTime);
            NetworkServer.Destroy(gameObject);
            ServerOnPlayerDespawned?.Invoke(this);
        }

        #endregion

        #region Client
        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }
        #endregion
    }
}
