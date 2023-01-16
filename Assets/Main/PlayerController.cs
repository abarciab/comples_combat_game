using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform camPivot;

    [Header("Game Stats (Resets on Play)")]
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private float currentMaxMoveSpeed;
    [SerializeField] private bool dashing;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float cameraPivotRotation;
    [SerializeField] private bool dashReady;
    private ContactPoint _groundCP;

    [Header("Player Stats Settings")]
    public float maxMoveSpeed;
    //public float sprintModifier;
    public float moveAcceleration;
    public float jumpForce;
    public float cameraSensitivity;
    //public float interactRange;
    //public LayerMask interactableLayers;
    public float dashSpeedModifier;
    public float dashDuration;
    public float dashCooldown;

    private List<ContactPoint> contactPoints = new List<ContactPoint>();

    void Start()
    {
        currentMaxMoveSpeed = maxMoveSpeed;
        _rigidbody.sleepThreshold = 0f;
        dashReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMove();
        ApplyLook();
        if(!dashing)
            LimitMovement();
    }

    private void FixedUpdate()
    {
        _groundCP = default(ContactPoint);
        isGrounded = FindGround(out _groundCP, contactPoints);
        contactPoints.Clear();
    }

    #region Inputs

    public void Move(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(dashReady && isGrounded)
            StartCoroutine(ApplyDash());

        /*
        // Code from sprint method, unused
        if (context.started)
        {
            dashing = true;
            UpdateCurrentMaxMoveSpeed();
        }

        if (context.canceled)
        {
            dashing = false;
            UpdateCurrentMaxMoveSpeed();
        }
        */
    }

    public void Jump(InputAction.CallbackContext context)
    {
        ApplyJump();
    }

    public void Look(InputAction.CallbackContext context)
    {
        lookDirection = context.ReadValue<Vector2>();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        
    }

    /*
    public void Interact(InputAction.CallbackContext context)
    {
        TryInteract();
    }
    */
    #endregion
    

    #region Input Application

    private void ApplyMove()
    {
        Vector3 movement = isGrounded ? (Vector3.Cross(transform.forward, _groundCP.normal) * -moveDirection.x) + (Vector3.Cross(transform.right, _groundCP.normal) * moveDirection.y ) : 
                                        (transform.right * moveDirection.x) + (transform.forward * moveDirection.y);
        _rigidbody.AddForce(movement * moveAcceleration * (currentMaxMoveSpeed/maxMoveSpeed) * (Time.deltaTime * 60), ForceMode.Acceleration);
        //Debug.DrawLine(transform.position, transform.position + movement, color: Color.blue, 0.5f);
    }

    private void UpdateCurrentMaxMoveSpeed()
    {
        if (dashing)
        {
            currentMaxMoveSpeed = maxMoveSpeed;
        }
        else
        {
            currentMaxMoveSpeed = maxMoveSpeed;
        }
    }

    public void UpdateGroundedState(bool state)
    {
        isGrounded = state;
    }

    private void ApplyLook()
    {
        float lookX = lookDirection.x * cameraSensitivity;
        float lookY = lookDirection.y * cameraSensitivity;

        cameraPivotRotation -= lookY;
        cameraPivotRotation = Mathf.Clamp(cameraPivotRotation, -90f, 90f);
        transform.Rotate(Vector3.up, lookX);
        camPivot.localRotation = Quaternion.Euler(cameraPivotRotation, 0, 0);
    }

    private void ApplyJump()
    {
        if (!isGrounded)
        {
            return;
        }
        _rigidbody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
    }

    private IEnumerator ApplyDash()
    {
        dashing = true;
        dashReady = false;
        currentMaxMoveSpeed = maxMoveSpeed + dashSpeedModifier;
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
        currentMaxMoveSpeed = maxMoveSpeed;
        yield return new WaitForSeconds(dashCooldown);
        dashReady = true;
    }

    /*
    private void TryInteract()
    {
        RaycastHit hit;
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * interactRange, color:Color.blue, 1f);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactRange, interactableLayers))
        {
            print(hit.transform.gameObject.name);
            if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Interactable")
            {
                //print("hit");
                hit.transform.GetComponentInParent<Interactable>().InteractAction();
            }
        }
    }
    */

    #endregion

    #region Collision

    private void OnCollisionEnter(Collision collision)
    {
        contactPoints.AddRange(collision.contacts);
    }

    private void OnCollisionStay(Collision collision)
    {
        contactPoints.AddRange(collision.contacts);
    }

    bool FindGround(out ContactPoint groundCP, List<ContactPoint> allCPs)
    {
        groundCP = default(ContactPoint);
        bool found = false;
        foreach (ContactPoint cp in allCPs)
        {
            //Pointing with some up direction
            if (cp.normal.y > 0.0001f && (found == false || cp.normal.y > groundCP.normal.y))
            {
                groundCP = cp;
                found = true;
            }
        }

        return found;
    }

    #endregion
    private void LimitMovement()
    {
        _rigidbody.velocity = Vector3.ClampMagnitude(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z), currentMaxMoveSpeed) + new Vector3(0, _rigidbody.velocity.y, 0); 
    }

    
}
