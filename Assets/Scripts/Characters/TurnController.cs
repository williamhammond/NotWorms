using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Characters
{
    public class TurnController : MonoBehaviour
    {
        [SerializeField]
        public int currentTurn = 0;

        //private Dictionary<int, Player> playerList;
        private List<Player> turnOrder = new();

        public void AddPlayer(Player player)
        {
            if (turnOrder.Contains(player))
                return;

            turnOrder.Add(player);
        }

        public void ResetTurnOrder()
        {
            turnOrder = (List<Player>)turnOrder.Shuffle();
            currentTurn = 0;
        }

        public Player CurrentPlayer()
        {
            if (turnOrder.Count < 1)
            {
                return null;
            }

            return turnOrder[currentTurn];
        }

        public void NextTurn()
        {
            currentTurn++;
            if (currentTurn >= turnOrder.Count)
            {
                currentTurn = 0;
            }
        }
    }
}
