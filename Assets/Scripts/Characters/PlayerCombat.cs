using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class PlayerCombat : NetworkBehaviour
    {
        private PlayerInput _playerInput;
        private Animator _animator;
        private NetworkAnimator _networkAnimator;
        private Weapon _weapon;
        private Rigidbody2D _body;

        private static readonly int AttackID = Animator.StringToHash("attack");

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _animator = GetComponent<Animator>();
            _networkAnimator = GetComponent<NetworkAnimator>();
            _weapon = GetComponent<Weapon>();
            _body = GetComponent<Rigidbody2D>();
        }

        #region Server


        [Command]
        private void CmdFire()
        {
            if (CanAttack())
            {
                _networkAnimator.SetTrigger(AttackID);
                _weapon.Fire();
            }
        }

        private bool CanAttack()
        {
            return _body.velocity.x < 0.01f && _body.velocity.y < 0.01f && !_weapon.OnCooldown();
        }
        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            _playerInput.actions["Player/Fire"].performed += AuthorityHandleFire;
        }

        public override void OnStopClient()
        {
            _playerInput.actions["Player/Fire"].performed -= AuthorityHandleFire;
        }

        private void AuthorityHandleFire(InputAction.CallbackContext context)
        {
            CmdFire();
        }
        #endregion

    }
}
