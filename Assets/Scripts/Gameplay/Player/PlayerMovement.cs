using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private Rigidbody2D _body;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            speed = 1f;
        }

        void Update()
        {
            MoveHorizontal();
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }

        void MoveHorizontal()
        {
            transform.position += new Vector3(1, 0);
            // _body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, _body.velocity.y);
        }

        void Jump()
        {
            _body.velocity = new Vector2(_body.velocity.x, speed);
        }
    }
}
