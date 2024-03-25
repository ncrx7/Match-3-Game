using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction _selectAction;
    InputAction _fireAction;

    public event Action Fire;

    public Vector2 Selected => _selectAction.ReadValue<Vector2>(); // read mouse screen position

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _selectAction = _playerInput.actions["Select"];
        _fireAction = _playerInput.actions["Fire"];

        _fireAction.performed += OnFired; // happens events when mouse 1 cliecked.
    }

    private void OnDestroy()
    {
        _fireAction.performed -= OnFired;
    }

    void OnFired(InputAction.CallbackContext obj)
    {
        Fire?.Invoke();
    }
}
