using System;
using System.Collections;
using UnityEngine;
using Combat;
using Mirror;
using UnityEngine.InputSystem;

namespace Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : NetworkBehaviour, IDamagable
    {
        public static event Action<Player> ServerOnPlayerSpawned;
        public static event Action<Player> ServerOnPlayerDespawned;

        public static event Action ServerPlayerEndedTurn;

        public event Action<int, int> ClientOnHealthUpdated;

        [SerializeField]
        private int maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int _currentHealth;

        private Animator _animator;
        private PlayerMovement _playerMovement;
        private PlayerCombat _playerCombat;
        private PlayerInput _playerInput;
        private NetworkAnimator _networkAnimator;
        private float _deathAnimationTime;

        private static readonly int DeathID = Animator.StringToHash("death");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _networkAnimator = GetComponent<NetworkAnimator>();
            _playerMovement = GetComponent<PlayerMovement>();
            _playerCombat = GetComponent<PlayerCombat>();
            _playerInput = GetComponent<PlayerInput>();
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

        public int GetHealth()
        {
            return _currentHealth;
        }

        public bool IsAlive()
        {
            return _currentHealth > 0;
        }

        #region Server

        public override void OnStartServer()
        {
            PlayerEnergy.ServerEnergyExhausted += ServerHandleEnergyExhausted;
            PlayerEnergy.ServerEnergyReset += ServerHandleEnergyReset;

            ServerOnPlayerSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            PlayerEnergy.ServerEnergyExhausted -= ServerHandleEnergyExhausted;
            PlayerEnergy.ServerEnergyReset -= ServerHandleEnergyReset;

            ServerOnPlayerDespawned?.Invoke(this);
        }

        private void ServerHandleEnergyExhausted(int connectionId)
        {
            if (connectionToClient.connectionId != connectionId)
            {
                return;
            }

            RpcHandleEnergyExhaustion();
        }

        private void ServerHandleEnergyReset(int connectionId)
        {
            if (connectionToClient.connectionId != connectionId)
            {
                return;
            }
            RpcHandleEnergyReset();
        }

        [Server]
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            Debug.Log($"Player maxHealth is {maxHealth}");
            if (!IsAlive())
            {
                RpcOnDeath();
            }
        }

        IEnumerator DestroyWithAnimation()
        {
            _networkAnimator.SetTrigger(DeathID);
            yield return new WaitForSeconds(_deathAnimationTime);
            NetworkServer.Destroy(gameObject);
            ServerOnPlayerDespawned?.Invoke(this);
        }

        [Command]
        private void CmdEndTurn()
        {
            ServerPlayerEndedTurn?.Invoke();
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            _playerInput.actions["Player/EndTurn"].performed += ClientHandleNextTurn;
        }

        public override void OnStopClient()
        {
            _playerInput.actions["Player/EndTurn"].performed -= ClientHandleNextTurn;
        }

        private void ClientHandleNextTurn(InputAction.CallbackContext obj)
        {
            CmdEndTurn();
        }

        [ClientRpc]
        private void RpcOnDeath()
        {
            StartCoroutine(DestroyWithAnimation());
        }

        [ClientRpc]
        private void RpcHandleEnergyExhaustion()
        {
            if (_playerMovement)
            {
                _playerMovement.enabled = false;
            }

            if (_playerCombat)
            {
                _playerCombat.enabled = false;
            }
        }

        [ClientRpc]
        private void RpcHandleEnergyReset()
        {
            if (_playerMovement)
            {
                _playerMovement.enabled = true;
            }

            if (_playerCombat)
            {
                _playerCombat.enabled = true;
            }
        }

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }

        #endregion
    }
}
