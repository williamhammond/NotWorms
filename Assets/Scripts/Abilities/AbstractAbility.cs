using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AbstractAbility
{
    protected string _animatorID;
    protected Rigidbody2D _body;
    protected Animator _animator;
    protected bool _active;

    public AbstractAbility(string animatorID, Animator animator, Rigidbody2D body)
    {
        _animatorID = animatorID;
        _animator = animator;
        _body = body;
        _active = false;
    }

    public abstract void Initialize(GameObject obj);
    public abstract void Trigger(InputAction.CallbackContext ctxt);

    public void activate()
    {
        _animator.SetBool(_animatorID, true);
        _active = true;
    }
}
