using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerControls controls;
    float direction = 0;

    public float speed = 400;
    public bool isFacingRight = true;

    public float jumpForce = 5;
    bool isGrounded;
    int numberOfJumps = 0;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public int playerCoins = 0;
    public int diamondCount = 0;

    public Rigidbody2D playerRB;
    public Animator animator;

    public ParticleSystem dust;

    // Add this flag to control movement
    private bool canMove = true;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();

        controls.Land.Move.performed += ctx =>
        {
            if (canMove) // Check if movement is allowed
            {
                direction = ctx.ReadValue<float>();
            }
            else
            {
                direction = 0; // Force direction to 0 if can't move
            }
        };

        controls.Land.Jump.performed += ctx =>
        {
            if (canMove) // Check if movement is allowed
            {
                Jump();
            }
        };
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
        }
    }

    // Add these public methods for the portal to call
    public void DisableMovement()
    {
        canMove = false;
        direction = 0; // Reset direction when movement is disabled
        //Debug.Log("PlayerMovement: Movement disabled");
    }

    public void EnableMovement()
    {
        canMove = true;
        //Debug.Log("PlayerMovement: Movement enabled");
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            // If can't move, set velocity to 0 but keep gravity
            playerRB.linearVelocity = new Vector2(0, playerRB.linearVelocity.y);
            animator.SetFloat("speed", 0);
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("speed", Mathf.Abs(direction));

        float horizontalVelocity = direction * speed * Time.fixedDeltaTime * 50f;
        float verticalVelocity = playerRB.linearVelocity.y;

        // Dust particle logic
        if (isGrounded && Mathf.Abs(direction) > 0.01f && verticalVelocity < 0.01f)
        {
            if (!dust.isPlaying)
            {
                dust.Play();
            }
        }
        else
        {
            if (dust.isPlaying)
            {
                dust.Stop();
            }
        }

        // Apply ground sticking
        if (isGrounded && Mathf.Abs(direction) != 0 && verticalVelocity < 0.01f)
        {
            verticalVelocity = -2f;
        }

        playerRB.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);

        if ((isFacingRight && direction < 0) || (!isFacingRight && direction > 0))
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);

        if (isGrounded)
        {
            CreateDust();
        }
    }

    void CreateDust()
    {
        dust.Play();
    }

    void Jump()
    {
        if (isGrounded && playerRB != null)
        {
            numberOfJumps = 0;
            dust.Play();

            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
            numberOfJumps++;
            AudioManager.instance.Play("FirstJump");
        }
        else if (numberOfJumps == 1 && playerRB != null)
        {
            dust.Play();
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce * 0.9f);
            numberOfJumps++;

            AudioManager.instance.Play("SecondJump");
        }
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        PlayerManager manager = Object.FindFirstObjectByType<PlayerManager>();
        if (manager != null)
        {
            manager.numberOfCoins = playerCoins;
            manager.coinsText.text = playerCoins.ToString();
        }
    }

    public void AddDiamond()
    {
        diamondCount++;
        PlayerManager pm = FindFirstObjectByType<PlayerManager>();
        if (pm != null)
        {
            pm.numberOfDiamonds = diamondCount;
        }
    }
}