using TMPro;
using UnityEngine;

namespace UI
{
    public class EnergyLabel : MonoBehaviour
    {
        public float energy;
        private TextMeshProUGUI labelText;

        private void Awake()
        {
            energy = 100f;
            labelText = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            labelText.text = energy.ToString();
        }

        public void UpdateEnergy(float input)
        {
            energy = input;
        }
    }
}
