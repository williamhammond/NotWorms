using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        public IPlayerInput PlayerInput { get; set; }

        private Rigidbody2D _body;
        private Animator _animator;
        private Weapon _weapon;

        private bool _isJumping = false;

        private static readonly int IsRunningID = Animator.StringToHash("isRunning");
        private static readonly int IsJumpingID = Animator.StringToHash("isJumping");
        private static readonly int AttackID = Animator.StringToHash("attack");

        private void Awake()
        {
            PlayerInput = new PlayerInput();
            _body = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _weapon = GetComponent<Weapon>();
            speed = 5f;
        }

        void Update()
        {
            MoveHorizontal();
            HandleOrientation();
            _animator.SetBool(IsJumpingID, _isJumping);

            if (PlayerInput.Jump && !_isJumping)
            {
                Jump();
            }

            if (PlayerInput.Fire && CanAttack())
            {
                _animator.SetTrigger(AttackID);
                _weapon.Fire();
            }
            _weapon.IncrementTimer(Time.deltaTime);
        }

        void MoveHorizontal()
        {
            _body.velocity = new Vector2(PlayerInput.Horizontal * speed, _body.velocity.y);
            _animator.SetBool(IsRunningID, PlayerInput.Horizontal != 0);
        }

        void Jump()
        {
            _body.velocity = new Vector2(_body.velocity.x, speed);
            _isJumping = true;
        }

        private bool CanAttack()
        {
            return PlayerInput.Horizontal == 0 && !_isJumping && !_weapon.OnCooldown();
        }

        private void HandleOrientation()
        {
            if (PlayerInput.Horizontal >= 0f)
            {
                transform.localScale = Vector3.one;
            }
            else if (PlayerInput.Horizontal < 0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.CompareTag("Ground"))
            {
                _isJumping = false;
            }
        }
    }
}
