using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private PlayerInputs _inputs;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _mouseSense;

    private Rigidbody _rb;
    private Camera _camera;
    private Animator _anim;

    private float _xRotation = 0;
    private float _yRotation = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = Camera.main;
        _anim = GetComponent<Animator>();        

        _inputs.attackEvent.AddListener(OnAttack);
        _inputs.jumpEvent.AddListener(OnJump);
    }

    void Update()
    {
        OnMove();
        OnLook();
    }

    private void OnMove()
    {
        if (_inputs.move.x == 1)
        {
            _anim.SetTrigger("runForward");
        }
        else if (_inputs.move.x == -1)
        {
            _anim.SetTrigger("runBackward");
        }
        else if (_inputs.move.y == 1)
        {
            _anim.SetTrigger("runRight");
        }
        else if (_inputs.move.y == -1)
        {
            _anim.SetTrigger("runLeft");
        }

        _rb.AddRelativeForce(new Vector3(_inputs.move.x, 0, _inputs.move.y) * _speed * Time.deltaTime);
    }

    private void OnLook()
    {
        _xRotation -= _inputs.look.y;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _yRotation += _inputs.look.x;

        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, _yRotation, 0);
    }

    private void OnAttack()
    {
        
    }

    private void OnJump()
    {
        _anim.SetTrigger("jump");
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
}
