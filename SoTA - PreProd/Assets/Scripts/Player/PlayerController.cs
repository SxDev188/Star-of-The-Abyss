using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using System.Collections.Generic;
using System.Collections;
/// <summary>
/// Author: Gabbriel, Karin, Linus
/// 
/// Modified by: Sixten, Emil
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    bool justRespawned;

    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float boulderPushSpeed = 3.0f;
    [SerializeField] private float movementRotationByDegrees = 45;
    [SerializeField] private CharacterController characterController;
    private PlayerHealth playerHealth;
    public float VerticalVelocity = 0; //Used to manage gravity
    [SerializeField] float VerticalVelocityLowerCap = -2;
    [SerializeField] float VerticalVelocityUpperCap = 2;
    [SerializeField] Animator animator;
    public CharacterController CharacterController => characterController;

    private bool isMoving = false;
    private bool isMovementLocked = false;
    private bool isAttachedToBoulder = false;
    private bool isBeingGravityPulled = false;

    public bool IsAttachedToBoulder { get { return isAttachedToBoulder; } } //used in PlayerBoulderLockSymbolScript.cs
    public bool inputLocked = false; //Used to lock movement during gravity pull
    public bool disableGravityDuringPull = false; //Used to disable downward gravity during gravity pull


    private Vector3 movementLockAxis;
    private Vector3 movementDirection;
    private Vector2 lastMoveDirection;
    private Vector3 rotationAxis = Vector3.up;
    private Vector3 movementInput = Vector3.zero;

    private EventInstance playerSlither; //Audio

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        playerSlither = AudioManager.Instance.CreateInstance(FMODEvents.Instance.SlitherSound);
        playerHealth = GetComponent<PlayerHealth>();

    }

    private void FixedUpdate()
    {
        if (playerHealth.IsDead) InteruptMovement(); //stops player movement upon death, otherwise player might keep moving as long as the input stays exactly the same

        if (justRespawned)
        {
            justRespawned = false;
            return;
        }

        if (disableGravityDuringPull)
        {
            VerticalVelocity = 0;
        }
        else
        {
            //Apply gravity if not grounded (falling in Abyss)
            if (!characterController.isGrounded)
            {
                if (VerticalVelocity < VerticalVelocityUpperCap && VerticalVelocity > VerticalVelocityLowerCap)
                {
                    VerticalVelocity += Physics.gravity.y * Time.deltaTime;
                }
            }
            else
            {
                VerticalVelocity = 0;  //Resets to no gravity when grounded
            }
        }

        if (isMovementLocked && movementLockAxis != Vector3.zero)
        {
            movementInput = Vector3.Scale(movementInput, movementLockAxis);
        }

        if (isAttachedToBoulder) //don't move if attached to a boulder
        {
            return;
        }
        if(isBeingGravityPulled) 
        {  
            return; 
        }

        if (isMovementLocked && isMoving) //aka is pushing/pulling boulder
        {
            //this movement does not depend on where player is facing, only movementInput
            characterController.Move(movementInput * boulderPushSpeed * Time.deltaTime + Vector3.up * VerticalVelocity);
        }
        else if (isMoving)
        {
            Vector3 move = movementInput * moveSpeed * Time.deltaTime;
            move.y = VerticalVelocity;  // Apply vertical velocity here
            characterController.Move(move); // Move the character with gravity
        }
        else
        {
            characterController.Move(Vector3.up * VerticalVelocity);
        }
    }

    void OnMoveInput(InputValue input)
    {
        if (playerHealth.IsDead) return; //so player cannot do new movements when dead
        if (inputLocked) return;

        isMoving = true;
        SetMovingAnimationTrue();

        

        Vector2 input2d = input.Get<Vector2>();



        movementInput = new Vector3(input2d.x, 0, input2d.y);

        if (!isMovementLocked)
        {
            movementInput = RotateVector3(movementInput, movementRotationByDegrees, rotationAxis);
            LookAtMovementDirection();
        }

        if (input2d != Vector2.zero)
        {
            lastMoveDirection = input2d.normalized;
        }
    }


    void OnMoveRelease(InputValue input)
    {
        InteruptMovement();
    }

    public void InteruptMovement()
    {
        isMoving = false;
        SetMovingAnimationFalse();
        movementInput = Vector3.zero;
    }
   
    void LookAtMovementDirection()
    {
        if (movementInput != Vector3.zero)
        {
            movementDirection = (transform.position + movementInput) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(movementDirection, rotationAxis);

            transform.rotation = rotation;
        }

    }

    public void LockMovement(Vector3 axis) //For boulder movement
    {
        isMovementLocked = true;
        movementLockAxis = axis;
    }

    public void UnlockMovement()
    {
        isMovementLocked = false;
        movementLockAxis = Vector3.zero;
        InteruptMovement();
    }

    public bool GetIsMovementLocked()
    {
        return isMovementLocked;
    }

    public Vector3 RayBoulderInteraction(float interactionRange, GameObject interactedBoulder)
    {
        RaycastHit hit;
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        for (int i = 0; i < directions.Length; i++)
        {  
            if (Physics.Raycast(transform.position, directions[i], out hit, interactionRange))
            {
                Debug.DrawRay(transform.position, directions[i] * hit.distance, Color.red);

                if (hit.transform.gameObject == interactedBoulder && hit.transform.tag == "Boulder") //tag check here is probably not necessary but just a precaution
                {
                    return directions[i];
                }
            }
        }

        //means the boulder will not be attached to the player
        return Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spikes") || other.CompareTag("Abyss"))
        {
            VerticalVelocity = 0;
            playerHealth.Death();
        }
    }

   public void SetPlayerPosition(Vector3 position)
   {
        CharacterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
   }

    private void UpdateSound()
    {
        if (isMoving)
        {
            PLAYBACK_STATE playbackState;
            playerSlither.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerSlither.start();
            }
        }
    }

    private Vector3 RotateVector3(Vector3 vectorToRotate, float degrees, Vector3 rotationAxis)
    {
        Vector3 rotatedVector = Quaternion.AngleAxis(degrees, rotationAxis) * vectorToRotate;
        return rotatedVector;
    }

    public Vector2 GetLastMoveDirection() //Used for CameraPan
    {
        return lastMoveDirection;
    }

    float boulderMoveThreshhold = 0.5f;
    public Vector3 GetBoulderPushDirection() //Used for boulder push/pull
    {
        if (isAttachedToBoulder && isMovementLocked && movementLockAxis != Vector3.zero)
        {
            Vector3 boulderPushDirection = Vector3.Scale(movementInput, movementLockAxis);

            if (boulderPushDirection.sqrMagnitude > Mathf.Pow(boulderMoveThreshhold, 2))  //here using boulderMoveThreshhold means that small amounts of stick drift on gamepad won't be enough to push the boulder in any direction
            {
                boulderPushDirection = boulderPushDirection.normalized;
                return boulderPushDirection;
            }
        }

        return Vector3.zero;
    }
    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.2f);
    }


    public void AttachToBoulder()
    {
        isAttachedToBoulder = true;
    }

    public void DetachFromBoulder()
    {
        isAttachedToBoulder = false;
    }
    public void StartBeingGravityPulled()
    {
        isBeingGravityPulled = true;
        inputLocked = true;
        disableGravityDuringPull = true;
    }

    public void StopBeingGravityPulled()
    {
        isBeingGravityPulled = false;
        inputLocked = false;
        disableGravityDuringPull = false;
    }
    public void SetDeathAnimationTrue()
    {
        animator.SetBool("isDead", true);
    }

    public void SetDeathAnimationFalse()
    {
        animator.SetBool("isDead", false);
    }

    private void SetMovingAnimationTrue()
    {
        animator.SetBool("isMoving", true);
    }

    private void SetMovingAnimationFalse()
    {
        animator.SetBool("isMoving", false);
    }

}
