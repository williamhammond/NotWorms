using System;
using Character;
using UnityEngine;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private float maxLifetime;

        [SerializeField]
        private float damage;

        private Animator _animator;
        private BoxCollider2D _boxCollider;

        private bool _isHit = false;
        private float _direction;
        private static readonly int ExplodeID = Animator.StringToHash("explode");

        private static readonly float Tolerance = 1f;

        public void Setup(float direction)
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            SetDirection(direction);
            Destroy(gameObject, maxLifetime);
        }

        private void SetDirection(float direction)
        {
            _direction = direction;

            float localScaleX = transform.localScale.x;
            if (Math.Abs(Mathf.Sign(localScaleX) - _direction) > Tolerance)
            {
                localScaleX = -localScaleX;
            }

            var localScale = transform.localScale;
            localScale = new Vector3(localScaleX, localScale.y, localScale.z);
            transform.localScale = localScale;
        }

        private void Update()
        {
            if (_isHit)
            {
                return;
            }

            float velocity = speed * Time.deltaTime * _direction;
            transform.Translate(velocity, 0, 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isHit = true;
            _boxCollider.enabled = false;
            _animator.SetTrigger(ExplodeID);
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                player.TakeDamage(damage);
            }
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
