using UI;
using UnityEngine;

namespace Characters
{
    public class PlayerEnergy : MonoBehaviour
    {
        private EnergyLabel _energyLabel;
        private PlayerInput _playerInput;

        [SerializeField]
        private float energy = 100f;

        private void Awake()
        {
            _energyLabel = GetComponent<EnergyLabel>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if (_playerInput.ResetEnergy)
            {
                energy = 100f;
            }
            UpdateEnergyLabel();
            if (energy == 0)
            {
                _playerInput.gameObject.SetActive(false);
            }
        }

        private void UpdateEnergyLabel()
        {
            _energyLabel.UpdateEnergy(energy);
        }
    }
}
