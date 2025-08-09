using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class FPSInputActions : MonoBehaviour
{
    public float speed = 5f;
    public float crouchSpeed = 2f;
    public float lookSensitivity = 1f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public Transform cameraTransform;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;

    public PlayerInputActions InputActions => inputActions;

    private PlayerInputActions inputActions;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch = 0f;
    private Vector3 velocity;
    private bool isCrouching = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }

    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();

        inputActions.Player.Crouch.started += _ => StartCrouch();
        inputActions.Player.Crouch.canceled += _ => StopCrouch();

        inputActions.Player.Interact.performed += ctx => TryInteract();

        inputActions.Player.RiskScroll.performed += ctx => OnRiskScroll(ctx.ReadValue<Vector2>());

        inputActions.Player.UIConfirm.performed += _ => PressUIButton();

        inputActions.Player.Withdraw.performed += _ => TryWithdraw();
        
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
       
    }

    void Update()
    {
        //Mouse look
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);

        lookInput = Vector2.zero;

        //Movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isCrouching ? crouchSpeed : speed;

        //Ground check
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Jump
        if (inputActions.Player.Jump.triggered && controller.isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        // Final Move
        Vector3 finalMove = (move * currentSpeed) + (Vector3.up * velocity.y);
        controller.Move(finalMove * Time.deltaTime);
    }

    void StartCrouch()
    {
        controller.height = crouchingHeight;
        isCrouching = true;
    }

    void StopCrouch()
    {
        controller.height = standingHeight;
        isCrouching = false;
    }

    void TryInteract()
    {
        float interactRange = 5f;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactRange))
        {
                        
            //Interact with Part Ordering Computer
            PartOrderingComputer computer = hit.collider.GetComponent<PartOrderingComputer>();
            if (computer != null){
                computer.ToggleUI();
                Debug.Log("Opened part ordering UI");
                return;
            }

            // Grab money
            //Money cash = hit.collider.GetComponent<Money>();
            //if (cash != null)
            //{
            //    PlayerMoney playerMoney = FindFirstObjectByType<PlayerMoney>();
            //    if (playerMoney != null)
            //    {
            //        playerMoney.money += cash.value;
            //        Debug.Log($"Picked up ${cash.value}. Total: ${playerMoney.money}");
            //        Destroy(cash.gameObject);
            //    }
            //    return;
            //}



        }

    }

    void TryWithdraw()
    {
        float interactRange = 5f;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactRange))
        {


            //Interact with MoneyPrinter
            MoneyPrinter printer = hit.collider.GetComponent<MoneyPrinter>();
            if (printer != null)
            {
                printer.Withdraw();
                Debug.Log("Withdrew money successful.");
                return;
            }


        }
    }



        void OnRiskScroll(Vector2 scrollDelta)
    {
        float scrollY = scrollDelta.y;

        if (Mathf.Abs(scrollY) < 0.01f)
            return;

        float interactRange = 3f;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactRange))
        {
            MoneyPrinter printer = hit.collider.GetComponent<MoneyPrinter>();
            if (printer != null)
            {
                float riskChange = scrollY > 0 ? 0.05f : -0.05f;
                printer.riskSlider.value = Mathf.Clamp01(printer.riskSlider.value + riskChange);
            }
        }
    }

    void PressUIButton()
    {
        
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = new Vector2(Screen.width / 2, Screen.height / 2);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        
        foreach (var result in results)
        {
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn != null)
            {
                Debug.Log("Pressed button: " + btn.gameObject.name);
                btn.onClick.Invoke();
                return;
            }
        }

    }

}

