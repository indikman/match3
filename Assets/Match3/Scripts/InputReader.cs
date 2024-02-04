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

        void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            selectAction = playerInput.actions["Select"];
            fireAction = playerInput.actions["Fire"];

            fireAction.performed += OnFire;
        }

        void OnDestroy()
        {
            fireAction.performed -= OnFire;
        }

        public void OnFire(InputAction.CallbackContext obj) => Fire?.Invoke();
    }
}
