using UnityEngine;
using UnityEngine.InputSystem;

public class RocketJump : AbstractAbility
{
    public RocketJump(Animator animator, Rigidbody2D body) : base("isRocketing", animator, body) { }

    public override void Initialize(GameObject obj) { }

    //TODO set fields on collision
    public override void Trigger(InputAction.CallbackContext ctxt)
    {
        activate();
        Debug.Log("rocketing " + _active + " state " + _animator.GetBool(_animatorID));
        //https://stackoverflow.com/questions/34250868/unity-addexplosionforce-to-2d
        Vector2 explosionDir = Vector2.up;
        int upwardsModifier = 500;
        float explosionDistance = explosionDir.magnitude;
        explosionDir.y += upwardsModifier;
        _body.AddForce(explosionDir);
    }
}
