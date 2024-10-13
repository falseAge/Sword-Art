using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovementController : NetworkBehaviour
{

    public static PlayerMovementController Current { get; private set; } = null;

    [SerializeField] private Camera headCamera;
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimatorOverrideController _oneHanded;
    [SerializeField] private AnimatorOverrideController _twoHanded;

    [SerializeField, Range(0, 100)] private float _currentHealth = 100;
    [SerializeField, Range(0, 100)] private float _currentMana = 100;
    [SerializeField, Range(0, 50)] private float speed = 10;
    [SerializeField, Range(0, 50)] private float sprintSpeed = 15;
    [SerializeField, Range(0, 2)] private float characterSense = 1f;

    private float _maxHealth = 100;
    private float _maxMana = 100;

    public UnityEvent<float> OnGetDamage;
    public UnityEvent<float> OnUsingMana;
    public UnityEvent<int> OnHotBarChange;
    

    public bool IsGrounded { get; private set; } = false;

    private CharacterController characterController;
    private PlayerInput input;
    
    private Vector3 movementDirection = Vector3.zero;
    private Vector2 lookDirection = Vector2.zero;

    private Vector3 localMovementAcelerationVector = Vector3.zero;

    private bool SprintState = false;
    
    private Vector3 velocity = Vector3.zero;
    private float contollerHitResetTimeout = 0;

    private Vector3 resultmovementDirection = Vector3.zero;

    public void StartControlling()
    {
        input.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void StopControlling()
    {
        input.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private RuntimeAnimatorController _defaultController;

    private void Awake()
    {
        _defaultController = _animator.runtimeAnimatorController;

        characterController = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    
        StartControlling();
        Current = this;

        _currentHealth = _maxHealth;
        _currentMana = _maxMana;
    }
    private void FixedUpdate()
    {
        ResetCollisionData();
        CalculateVelocity(ref velocity);

        resultmovementDirection = velocity * 5f + CalculateMovementDirection();
    }
    private void LateUpdate()
    {
        var timescale = Time.deltaTime * 20f;

        transform.rotation = 
            Quaternion.Lerp(
                transform.rotation, 
                Quaternion.Euler(0, lookDirection.x, 0), 
                timescale);
        
        headCamera.transform.localRotation = 
            Quaternion.Lerp(
                headCamera.transform.localRotation, 
                Quaternion.Euler(-lookDirection.y, 0, 0), 
                timescale);

        characterController.Move(resultmovementDirection * Time.deltaTime);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (headCamera == null)
        {
            headCamera = GetComponentInChildren<Camera>();
        }
    }
#endif

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contollerHitResetTimeout = 0.1f;

        IsGrounded = Vector3.Angle(hit.normal, Vector3.up) <= 35;


        var normalAngle = Quaternion.FromToRotation(hit.normal, Vector3.down);

        var deltaVelocity = normalAngle * velocity;
        deltaVelocity.y = Mathf.Min(0, deltaVelocity.y);


        if (IsGrounded)
        {
            deltaVelocity.x = 0;    
            deltaVelocity.z = 0;
        }

        // deltaVelocity.x = Mathf.MoveTowards(deltaVelocity.x, 0, hit.collider.material.dynamicFriction / 10);
        // deltaVelocity.z = Mathf.MoveTowards(deltaVelocity.z, 0, hit.collider.material.dynamicFriction / 10);

        velocity = Quaternion.Inverse(normalAngle) * deltaVelocity;
    }

    private void OnMove(InputValue inputValue)
    {
        var input = inputValue.Get<Vector2>();
        _animator.SetFloat("walkDirectionX", input.x);
        _animator.SetFloat("walkDirectionY", input.y);
        movementDirection = new Vector3(input.x, 0, input.y);
    }

    private void OnLook(InputValue inputValue)
    {
        lookDirection += inputValue.Get<Vector2>() * characterSense;

        lookDirection.y = Mathf.Clamp(lookDirection.y, -89, 89);
    }

    private void OnAttack(InputValue inputValue)
    {
         _animator.SetTrigger("isPunchAttackRight");
        // if (inputValue.isPressed)
        // {
        //     bool randomPunch = Random.value > 0.5;
        //     if (randomPunch)
        //         _animator.SetTrigger("isPunchAttackRight");
        //     else
        //         _animator.SetTrigger("isPunchAttackLeft");
        // }
    }

    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && IsGrounded)
        {
            if (movementDirection !=  Vector3.zero)
                _animator.SetTrigger("isRunningJump");
            else
                _animator.SetTrigger("isJump");
            velocity = Vector3.up * 3;
        }
    }

    private void OnSprint(InputValue inputValue)
    {
        SprintState = inputValue.isPressed;
    }

    private Vector3 CalculateMovementDirection()
    {
        localMovementAcelerationVector = Vector3.Lerp(localMovementAcelerationVector, transform.rotation * movementDirection * (SprintState ? sprintSpeed : speed), (IsGrounded ? 10 : 1) * Time.fixedDeltaTime);

        return localMovementAcelerationVector; 
    }
    private void CalculateVelocity(ref Vector3 velocity)
    {
        velocity = Vector3.Lerp(velocity, Physics.gravity, Time.fixedDeltaTime);
    }

    private void ResetCollisionData()
    {
        contollerHitResetTimeout -= Time.fixedDeltaTime;
        
        if (contollerHitResetTimeout < 0)
        {
            IsGrounded = false;
        }
    }

    private void OnHotBar(int slot)
    {
        ChangeAnimationsType(slot);
        OnHotBarChange?.Invoke(slot);
    }

    private void OnSlot1(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(1);
    }
    private void OnSlot2(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(2);
    }
    private void OnSlot3(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(3);
    }
    private void OnSlot4(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(4);
    }
    private void OnSlot5(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(5);
    }
    private void OnSlot6(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(6);
    }
    private void OnSlot7(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(7);
    }
    private void OnSlot8(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(8);
    }
    private void OnSlot9(InputValue inputValue)
    {
        if (inputValue.isPressed)
            OnHotBar(9);
    }

    private void OnTestButton(InputValue inputValue)
    {
        if (inputValue.isPressed)
            TakeDamage(10);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        OnGetDamage?.Invoke(_currentHealth);
    }

    public void UsingMana(float mana)
    {
        _currentMana -= mana;
        OnUsingMana?.Invoke(_currentMana);
    }

    private void ChangeAnimationsType(int hotBarSlot)
    {
        switch (hotBarSlot)
        {
            case 1:
                _animator.runtimeAnimatorController = _defaultController;
                break;
            case 2:
                _animator.runtimeAnimatorController = _oneHanded;
                break;
            case 3:
                _animator.runtimeAnimatorController = _twoHanded;
                break;
        }
    }
}
