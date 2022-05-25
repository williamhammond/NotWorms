using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AbstractAbility
{
    protected string _animatorID;
    protected Rigidbody2D _body;
    protected Animator _animator;
    protected bool _active;
    protected float _energyCost;

    public static event Action<float> AbilityTriggered;

    public AbstractAbility(string animatorID, Animator animator, Rigidbody2D body, float energyCost)
    {
        _animatorID = animatorID;
        _animator = animator;
        _body = body;
        _active = false;
        _energyCost = energyCost;
    }

    public abstract void Initialize(GameObject obj);
    public abstract void Trigger(InputAction.CallbackContext ctxt);

    public void Cleanup()
    {
        AbilityTriggered?.Invoke(_energyCost);
    }

    public void Activate()
    {
        _animator.SetBool(_animatorID, true);
        _active = true;
    }
}
