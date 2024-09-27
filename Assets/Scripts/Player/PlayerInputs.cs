using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerInputs : MonoBehaviour
{
    public UnityEvent attackEvent = new();
    public UnityEvent jumpEvent = new();

    public Vector2 move;
    public Vector2 look;
    public bool attack;
    public bool jump;

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        attack = value.isPressed;
        attackEvent?.Invoke();
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
        jumpEvent?.Invoke();
    }
}
