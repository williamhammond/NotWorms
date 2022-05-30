using Mirror;
using TMPro;

namespace UI
{
    public class CurrentTurnText : NetworkBehaviour
    {
        private TextMeshProUGUI labelText;

        private void Awake()
        {
            labelText = GetComponent<TextMeshProUGUI>();
        }

        public void SetTurn(int turn)
        {
            labelText.text = $"Turn: {turn}";
        }
    }
}
