using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EnergyLabel : MonoBehaviour
    {
        private TextMeshProUGUI labelText;

        private void Awake()
        {
            labelText = GetComponent<TextMeshProUGUI>();
        }

        public void SetEnergy(float input)
        {
            labelText.text = $"Energy: {Math.Round(input)}";
        }
    }
}
