using TMPro;
using UnityEngine;

namespace UI
{
    public class CurrentTurnText : MonoBehaviour
    {
        public int currentTurn;
        private TextMeshProUGUI labelText;

        private void Awake()
        {
            currentTurn = 0;
            labelText = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            labelText.text = currentTurn.ToString();
        }

        public void UpdateTurn(int input)
        {
            currentTurn = input;
        }
    }
}
