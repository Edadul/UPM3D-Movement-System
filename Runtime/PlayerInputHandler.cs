using UnityEngine;
using UnityEngine.InputSystem;

namespace Edadul.MovementSystem
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private MovementController _controller;
        private PlayerInputActions _actions;

        private Vector2 _moveInput;
        private bool _isSprinting;

        private void Awake()
        {
            _controller = GetComponent<MovementController>();
            _actions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _actions.Player.Enable();
            _actions.Player.Jump.performed += OnJump;
            _actions.Player.Sprint.performed += OnSprintStart;
            _actions.Player.Sprint.canceled += OnSprintStop;
        }

        private void OnDisable()
        {
            _actions.Player.Jump.performed -= OnJump;
            _actions.Player.Sprint.performed -= OnSprintStart;
            _actions.Player.Sprint.canceled -= OnSprintStop;
            _actions.Player.Disable();
        }

        private void OnJump(InputAction.CallbackContext ctx) => _controller.Jump();
        private void OnSprintStart(InputAction.CallbackContext ctx) => _isSprinting = true;
        private void OnSprintStop(InputAction.CallbackContext ctx) => _isSprinting = false;

        private void FixedUpdate()
        {
            _moveInput = _actions.Player.Move.ReadValue<Vector2>();

            if (_moveInput.sqrMagnitude > 0.01f)
                _controller.Move(_moveInput, _isSprinting);
            else
                _controller.Decelerate();

            _controller.ApplyGravity();
        }
    }
}