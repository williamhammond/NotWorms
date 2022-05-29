using System;
using Mirror;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class PlayerEnergy : NetworkBehaviour
    {
        [SerializeField]
        private float energy = 100f;

        public static event Action ServerEnergyExhausted;
        public static event Action EnergyReset;

        private EnergyLabel _energyLabel;
        private bool _isMoving = false;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _energyLabel = FindObjectOfType<EnergyLabel>();
            _playerInput = GetComponent<PlayerInput>();
        }

        #region Server

        [ServerCallback]
        private void Update()
        {
            if (_isMoving)
            {
                ServerHandleEnergyUpdate(0.1f);
            }
        }

        public override void OnStartServer()
        {
            PlayerCombat.PlayerFired += ServerHandleFire;
            PlayerMovement.MovementStarted += ServerHandleMovementStart;
            PlayerMovement.MovementEnded += ServerHandleMovementEnd;
        }

        public override void OnStopServer()
        {
            PlayerCombat.PlayerFired -= ServerHandleFire;
            PlayerMovement.MovementStarted -= ServerHandleMovementStart;
            PlayerMovement.MovementEnded -= ServerHandleMovementEnd;
        }

        private void ServerHandleMovementEnd()
        {
            _isMoving = false;
        }

        private void ServerHandleMovementStart()
        {
            _isMoving = true;
        }

        [Server]
        private void ServerHandleEnergyUpdate(float cost)
        {
            bool energyPositive = energy > 0;
            energy = Mathf.Max(0, energy - cost);
            if (energyPositive && energy == 0)
            {
                ServerEnergyExhausted?.Invoke();
            }
            RpcHandleEnergyUpdate(energy);
        }

        [Command]
        private void CmdResetEnergy()
        {
            EnergyReset?.Invoke();
            energy = 100f;
            RpcHandleEnergyUpdate(energy);
        }

        [Server]
        private void ServerHandleFire()
        {
            ServerHandleEnergyUpdate(10);
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

        [ClientRpc]
        private void RpcHandleEnergyUpdate(float newEnergy)
        {
            _energyLabel.SetEnergy(newEnergy);
        }

        #endregion

    }
}
