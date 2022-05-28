using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class PlayerEnergy : MonoBehaviour
    {
        [SerializeField]
        private float energy = 100f;

        public static event Action EnergyExhausted;
        public static event Action EnergyReset;

        private EnergyLabel _energyLabel;
        private PlayerInput _playerInput;
        private bool _isMoving = false;

        private void Awake()
        {
            _energyLabel = FindObjectOfType<EnergyLabel>();

            _playerInput = GetComponentInParent<PlayerInput>();

            _playerInput.actions["Player/Fire"].performed += HandleFire;
            _playerInput.actions["Player/ResetEnergy"].performed += ResetEnergy;
            _playerInput.actions["Player/Movement"].started += HandleMovementStart;
            _playerInput.actions["Player/Movement"].canceled += HandleMovementEnd;

            AbstractAbility.AbilityTriggered += HandleEnergyUpdate;
        }

        private void OnDestroy()
        {
            _playerInput.actions["Player/Fire"].performed -= HandleFire;
            _playerInput.actions["Player/ResetEnergy"].performed -= ResetEnergy;
            _playerInput.actions["Player/Movement"].started -= HandleMovementStart;
            _playerInput.actions["Player/Movement"].canceled -= HandleMovementEnd;
        }

        private void Update()
        {
            if (_isMoving)
            {
                HandleEnergyUpdate(0.1f);
            }
        }

        private void HandleMovementEnd(InputAction.CallbackContext context)
        {
            _isMoving = false;
        }

        private void HandleMovementStart(InputAction.CallbackContext context)
        {
            _isMoving = true;
        }

        private void ResetEnergy(InputAction.CallbackContext context)
        {
            EnergyReset?.Invoke();
            energy = 100f;
            _energyLabel.SetEnergy(energy);
        }

        private void HandleFire(InputAction.CallbackContext context)
        {
            HandleEnergyUpdate(10);
        }

        private void HandleEnergyUpdate(float cost)
        {
            bool energyPositive = energy > 0;
            energy = Mathf.Max(0, energy - cost);
            if (energyPositive && energy == 0)
            {
                EnergyExhausted?.Invoke();
            }
            _energyLabel.SetEnergy(energy);
        }
    }
}
