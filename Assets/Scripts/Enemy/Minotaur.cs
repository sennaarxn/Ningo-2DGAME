using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Minotaur : MonoBehaviour
{
    [Header("Target Settings")]
    public float chaseRange = 10f;
    public float maxVerticalChaseDistance = 3f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float edgeCheckDistance = 0.5f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Combat")]
    public int enemyHP = 100;
    public Slider enemyHealthBar;

    [Header("References")]
    public Animator animator;
    public Transform borderCheckRight;
    public Transform borderCheckLeft;

    [SerializeField] private Vector3 _defaultPosition;
    private GameObject _target;
    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _isAtEdge;

    private bool isAttacking = false;
    private bool isTakingDamage = false;

    public Vector3 defaultPosition => _defaultPosition;

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
        _defaultPosition = transform.position;
        enemyHealthBar.value = enemyHP;

        if (_target != null)
            Physics2D.IgnoreCollision(_target.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    private void Update()
    {
        if (_target == null) return;

        if (PlayerManager.isGameOver || enemyHP <= 0)
        {
            StopAllMovement();
            return;
        }

        CheckGroundStatus();
        HandleFacingDirection();

        if (isAttacking || isTakingDamage || !_isGrounded)
        {
            // Stop movement if attacking, taking damage, or in air
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            animator.SetBool("isChasing", false);
            return;
        }

        HandleMovement();
    }

    private void StopAllMovement()
    {
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;

        animator.SetBool("isChasing", false);
        animator.SetBool("isReturning", false);
        animator.ResetTrigger("damage");
        animator.ResetTrigger("isAttacking");
        animator.Play("IdleState");
    }

    private void CheckGroundStatus()
    {
        LayerMask combinedGround = groundLayer | (1 << LayerMask.NameToLayer("MovingPlatform"));
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, combinedGround);

        float direction = Mathf.Sign(_target.transform.position.x - transform.position.x);
        Vector2 rayOrigin = new Vector2(groundCheck.position.x + direction * edgeCheckDistance, groundCheck.position.y);
        _isAtEdge = !_isGrounded || !Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, combinedGround);

        Debug.DrawRay(rayOrigin, Vector2.down * 0.5f, Color.red);

        if (!_isGrounded)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        }
    }

    private void HandleFacingDirection()
    {
        transform.localScale = (_target.transform.position.x > transform.position.x)
            ? new Vector2(0.35f, 0.35f)
            : new Vector2(-0.35f, 0.35f);
    }

    private void HandleMovement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _target.transform.position);
        float verticalDistance = Mathf.Abs(transform.position.y - _target.transform.position.y);

        bool shouldChase = distanceToPlayer < chaseRange &&
                           verticalDistance < maxVerticalChaseDistance &&
                           _isGrounded &&
                           !_isAtEdge;

        Vector2 separation = Vector2.zero;
        float separationDistance = 1f;   // How far apart they try to stay
        float separationStrength = 2f;   // How strongly they push away

        // Calculate separation from nearby Minotaurs
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationDistance);
        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject != gameObject && col.CompareTag("Enemy"))
            {
                Vector2 away = (Vector2)(transform.position - col.transform.position);
                separation += away.normalized / away.magnitude;
            }
        }

        if (shouldChase)
        {
            float direction = Mathf.Sign(_target.transform.position.x - transform.position.x);
            Vector2 moveVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);

            // Apply separation
            moveVelocity += separation * separationStrength * Time.deltaTime;

            _rb.linearVelocity = moveVelocity;
            animator.SetBool("isChasing", true);
            animator.SetBool("isReturning", false);
        }
        else
        {
            // Return to default position if not chasing
            Vector2 toDefault = _defaultPosition - transform.position;
            if (_isGrounded && !_isAtEdge && toDefault.magnitude > 0.1f)
            {
                float direction = Mathf.Sign(toDefault.x);
                Vector2 moveVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);

                // Apply separation even when returning
                moveVelocity += separation * separationStrength * Time.deltaTime;

                _rb.linearVelocity = moveVelocity;
                animator.SetBool("isReturning", true);
                animator.SetBool("isChasing", false);
            }
            else
            {
                _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
                animator.SetBool("isReturning", false);
                animator.SetBool("isChasing", false);
            }
        }
    }


    public void TakeDamage(int damageAmount)
    {
        if (enemyHP <= 0) return;

        enemyHP -= damageAmount;
        enemyHealthBar.value = enemyHP;

        if (enemyHP > 0)
        {
            animator.SetTrigger("damage");
            isTakingDamage = true;
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            StartCoroutine(ResetDamageFlag());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(0.3f);
        isTakingDamage = false;
    }

    private void Die()
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        GetComponent<CapsuleCollider2D>().enabled = false;
        enemyHealthBar.gameObject.SetActive(false);
        animator.SetTrigger("dead");
        animator.SetBool("isChasing", false);
        animator.SetBool("isReturning", false);

        AudioManager.instance.Play("Minotaur");

        this.enabled = false;
    }

    public void PlayerDamage()
    {
        if (PlayerManager.isGameOver || _target == null || enemyHP <= 0) return;

        PlayerCollision playerCollision = _target.GetComponent<PlayerCollision>();
        if (playerCollision != null && !playerCollision.isInvincible)
            playerCollision.TakeDamage();
    }

    public void OnAttackStart()
    {
        isAttacking = true;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        StartCoroutine(ResetAttackFlag());
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(0.5f); // Duration of attack animation
        isAttacking = false;
    }


    public void OnAttackEnd() => isAttacking = false;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MovePlatformLeftRight platform = collision.gameObject.GetComponent<MovePlatformLeftRight>();
        if (platform != null)
        {
            _isGrounded = true;
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        MovePlatformLeftRight platform = collision.gameObject.GetComponent<MovePlatformLeftRight>();
        if (platform != null)
            transform.parent = null;
    }
}