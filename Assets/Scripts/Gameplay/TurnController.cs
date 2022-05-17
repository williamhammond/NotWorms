using System.Collections.Generic;
using Character;
using UI;
using UnityEngine;

namespace Gameplay
{
    public class TurnController : MonoBehaviour
    {
        [SerializeField]
        private CurrentTurnText _turnText;

        private int _currentTurn = 0;
        private List<Player> _turnOrder = new List<Player>();

        public void Start()
        {
            Player.PlayerSpawned += HandlePlayerSpawned;
            Player.PlayerDespawned += HandlePlayerDespawned;
            Player.EndTurn += NextTurn;
        }

        public void OnDestroy()
        {
            Player.PlayerSpawned -= HandlePlayerSpawned;
            Player.PlayerDespawned -= HandlePlayerDespawned;
            Player.EndTurn -= NextTurn;
        }

        private void HandlePlayerSpawned(Player player)
        {
            if (_turnOrder.Contains(player))
                return;

            _turnOrder.Add(player);
        }

        private void HandlePlayerDespawned(Player player)
        {
            _turnOrder.Remove(player);
        }

        public void ResetTurnOrder()
        {
            _turnOrder.Shuffle();
            _currentTurn = 0;
        }

        public Player CurrentPlayer()
        {
            if (_turnOrder.Count < 1)
            {
                return null;
            }

            return _turnOrder[_currentTurn];
        }

        public void NextTurn()
        {
            _currentTurn = _currentTurn++ % _turnOrder.Count;
            _turnText.UpdateTurn(_currentTurn);
        }
    }
}
