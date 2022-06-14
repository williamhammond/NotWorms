using Characters;
using Mirror;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EnergyLabel : MonoBehaviour
    {
        private TextMeshProUGUI labelText;

        private PlayerEnergy _playerEnergy;

        private void Awake()
        {
            labelText = GetComponent<TextMeshProUGUI>();
        }

        public void Update()
        {
            // TODO: Hack until we have a lobby
            if (_playerEnergy == null)
            {
                _playerEnergy = NetworkClient.connection.identity.GetComponent<PlayerEnergy>();
                if (_playerEnergy != null)
                {
                    ClientHandleEnergyUpdated(_playerEnergy.GetEnergy());
                    _playerEnergy.ClientOnEnergyUpdated += ClientHandleEnergyUpdated;
                }
            }
        }

        private void ClientHandleEnergyUpdated(float energy)
        {
            labelText.text = $"Energy: {energy:0.00}";
        }
    }
}
