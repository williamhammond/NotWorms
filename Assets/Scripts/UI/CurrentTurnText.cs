using Characters;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CurrentTurnText : MonoBehaviour
    {
        private TextMeshProUGUI labelText;

        private void Awake()
        {
            labelText = GetComponent<TextMeshProUGUI>();
            TurnController.TurnChanged += HandleTurnChanged;
        }

        private void HandleTurnChanged(int turn)
        {
            labelText.text = $"{turn}";
        }
    }
}
