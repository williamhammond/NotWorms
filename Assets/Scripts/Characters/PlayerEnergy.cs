using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class PlayerEnergy : NetworkBehaviour
    {
        [SyncVar(hook = nameof(ClientHandleEnergyUpdate))]
        private float _energy = 100f;

        private bool _isMoving = false;

        public static event Action<int> ServerEnergyExhausted;
        public static event Action<int> ServerEnergyReset;

        public event Action<float> ClientOnEnergyUpdated;

        private PlayerInput _playerInput;

        public float GetEnergy()
        {
            return _energy;
        }

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        #region Server

        [ServerCallback]
        private void FixedUpdate()
        {
            if (_isMoving)
            {
                ServerHandleEnergyUpdate(0.1f);
            }
        }

        public override void OnStartServer()
        {
            PlayerCombat.ServerPlayerFired += ServerHandleFire;

            PlayerMovement.ServerMovementStarted += ServerHandleMovementStart;
            PlayerMovement.ServerMovementEnded += ServerHandleMovementEnd;

            PlayerMovement.ServerPlayerJumped += ServerHandlePlayerJumped;
        }

        public override void OnStopServer()
        {
            PlayerCombat.ServerPlayerFired -= ServerHandleFire;

            PlayerMovement.ServerMovementStarted -= ServerHandleMovementStart;
            PlayerMovement.ServerMovementEnded -= ServerHandleMovementEnd;

            PlayerMovement.ServerPlayerJumped -= ServerHandlePlayerJumped;
        }

        [Server]
        private void ServerHandleMovementStart(int connectionId)
        {
            if (connectionId == connectionToClient.connectionId)
            {
                _isMoving = true;
            }
        }

        [Server]
        private void ServerHandleMovementEnd(int connectionId)
        {
            if (connectionId == connectionToClient.connectionId)
            {
                _isMoving = false;
            }
        }

        [Server]
        private void ServerHandlePlayerJumped(int connectionId)
        {
            if (connectionId == connectionToClient.connectionId)
            {
                ServerHandleEnergyUpdate(10);
            }
        }

        [Server]
        private void ServerHandleEnergyUpdate(float cost)
        {
            bool energyPositive = _energy > 0;
            _energy = Mathf.Max(0, _energy - cost);
            if (energyPositive && _energy == 0)
            {
                ServerEnergyExhausted?.Invoke(connectionToClient.connectionId);
            }
        }

        [Server]
        private void ServerHandleFire(int connectionId)
        {
            if (connectionId == connectionToClient.connectionId)
            {
                ServerHandleEnergyUpdate(10);
            }
        }

        [Command]
        private void CmdResetEnergy()
        {
            ServerEnergyReset?.Invoke(connectionToClient.connectionId);
            _energy = 100f;
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            _playerInput.actions["Player/ResetEnergy"].performed += ClientResetEnergy;
        }

        public override void OnStopClient()
        {
            _playerInput.actions["Player/ResetEnergy"].performed -= ClientResetEnergy;
        }

        [Client]
        private void ClientResetEnergy(InputAction.CallbackContext obj)
        {
            CmdResetEnergy();
        }

        private void ClientHandleEnergyUpdate(float oldEnergy, float newEnergy)
        {
            ClientOnEnergyUpdated?.Invoke(newEnergy);
        }

        #endregion
    }
}
