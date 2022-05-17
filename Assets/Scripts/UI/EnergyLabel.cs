using TMPro;
using UnityEngine;

namespace UI
{
    public class EnergyLabel : MonoBehaviour
    {
        public float energy;

        [SerializeField]
        private TMP_Text labelText;

        private void Awake()
        {
            energy = 100f;
        }

        public void UpdateEnergy(float input)
        {
            labelText.text = $"{input}";
        }
    }
}
