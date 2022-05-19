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

        private EnergyLabel _energyLabel;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _energyLabel = FindObjectOfType<EnergyLabel>();

            _playerInput = GetComponent<PlayerInput>();
            _playerInput.actions["Player/Fire"].performed += HandleFire;
            _playerInput.actions["Player/ResetEnergy"].performed += ResetEnergy;
        }

        private void ResetEnergy(InputAction.CallbackContext obj)
        {
            energy = 100f;
        }

        private void HandleFire(InputAction.CallbackContext obj)
        {
            energy = Mathf.Max(0, energy - 10);
            _energyLabel.SetEnergy(energy);
        }
    }
}
