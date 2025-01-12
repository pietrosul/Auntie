using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
 
 
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
 
 
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
 
    public bool canMove = true;
    public bool cursorLock = true;
 
    public float crouchSpeed = 3f;
    public float crouchHeight = 1f;
    private float originalHeight;
    private bool isCrouching = false;

    public GameObject globalVolume;
    
    CharacterController characterController;
    
    [Header("Head Bobbing")]
    public float bobbingSpeed = 10f;
    public float bobbingAmount = 0.05f;
    public float midpoint = 2.0f;
    
    private float timer = 0.0f;
    private float defaultCameraY;
    private bool wasWalking;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
        defaultCameraY = playerCamera.transform.localPosition.y;
        
        if(cursorLock == true) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrouch();
        }

        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        bool isRunning = !isCrouching && Input.GetKey(KeyCode.LeftShift);
        
        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
        
        float curSpeedX = canMove ? currentSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? currentSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
 
        #endregion
 
        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        #endregion
 
        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);
 
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            
            HandleHeadBob();
        }
 
        #endregion
    }

    private void HandleHeadBob()
    {
        if (!playerCamera) return;
        
        Vector3 localPosition = playerCamera.transform.localPosition;
        bool isMoving = Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f;
        
        if(isMoving && characterController.isGrounded)
        {
            timer += Time.deltaTime * bobbingSpeed;
            localPosition.y = defaultCameraY + Mathf.Sin(timer) * bobbingAmount;
            localPosition.x = Mathf.Cos(timer/2) * bobbingAmount/2;
        }
        else
        {
            timer = 0f;
            localPosition.y = Mathf.Lerp(localPosition.y, defaultCameraY, Time.deltaTime * midpoint);
            localPosition.x = Mathf.Lerp(localPosition.x, 0f, Time.deltaTime * midpoint);
        }
        
        playerCamera.transform.localPosition = localPosition;
    }

    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        
        characterController.height = isCrouching ? crouchHeight : originalHeight;
        characterController.center = new Vector3(0, characterController.height / 2f, 0);
        
        defaultCameraY = isCrouching ? crouchHeight : originalHeight;
        playerCamera.transform.localPosition = new Vector3(
            playerCamera.transform.localPosition.x,
            defaultCameraY,
            playerCamera.transform.localPosition.z
        );
    }
}