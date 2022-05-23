using Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private Animator _animator;
        private Weapon _weapon;
        private Rigidbody2D _body;

        private static readonly int AttackID = Animator.StringToHash("attack");

        private void Awake()
        {
            _playerInput.actions["Player/Fire"].performed += HandleFire;
        }

        private void OnEnable()
        {
            _playerInput = GetComponentInParent<PlayerInput>();
            _animator = GetComponentInParent<Animator>();
            _weapon = GetComponentInParent<Weapon>();
            _body = GetComponentInParent<Rigidbody2D>();
        }

        private void OnDestroy()
        {
            _playerInput.actions["Player/Fire"].performed -= HandleFire;
        }

        private void HandleFire(InputAction.CallbackContext context)
        {
            if (CanAttack())
            {
                _animator.SetTrigger(AttackID);
                _weapon.Fire();
            }
        }

        private bool CanAttack()
        {
            return _body.velocity.x < 0.01f && _body.velocity.y < 0.01f && !_weapon.OnCooldown();
        }
    }
}
