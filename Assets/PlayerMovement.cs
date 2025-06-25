using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float dashForce = 10f;
    public float dashCooldown = 2f;

    private bool canDash = true;



    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private PlayerControls controls;



    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => TryJump();
        controls.Player.Dash.performed += ctx => TryDash();

    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (transform.position.y < -5f)
        {

            GameManager.Instance.ResetGame();
        }

    }

    void FixedUpdate()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 move = direction * moveSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }

    void TryJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void TryDash()
    {
        if (canDash && isGrounded)
        {
            Vector3 dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            if (dashDirection == Vector3.zero)
            {
                dashDirection = transform.forward; // Default direction if no input
            }

            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            canDash = false;

            //Timer
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    void ResetDash()
    {
        canDash = true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.ResetGame();
        }

        if (other.CompareTag("Finish"))
        {
            Debug.Log("You completed the Game!!");
            GameManager.Instance.StopTimer();
        }
    }
    
    
    


}
