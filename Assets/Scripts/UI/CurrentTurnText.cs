using TMPro;
using UnityEngine;

namespace UI
{
    public class CurrentTurnText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text labelText;

        public void UpdateTurn(int input)
        {
            labelText.text = $"{input}";
        }
    }
}
