using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace IndiMatchThree
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputReader : MonoBehaviour
    {
        PlayerInput playerInput;
        InputAction selectAction;
        InputAction fireAction;

        public event Action Fire;

        public Vector2 Selected => selectAction.ReadValue<Vector2>();

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            selectAction = playerInput.actions["select"];
            fireAction = playerInput.actions["fire"];

            fireAction.performed += OnFire;
        }

        private void OnDestroy()
        {
            fireAction.performed -= OnFire;
        }

        private void OnFire(InputAction.CallbackContext obj) => Fire?.Invoke();
    }
}
