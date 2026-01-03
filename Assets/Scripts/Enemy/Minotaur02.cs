using System.Threading;
using UnityEngine;

public class Minotaur02 : MonoBehaviour
{
    
    #region Public Variables
    public Transform raycast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance; //Minimum distacne for attack
    public float moveSpeed;
    public float timer; // Timer for cooldown between attacks
    #endregion

    #region Private Variables
    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance; // stores distance between player and enemy
    private bool attackMode;
    private bool inRange; // check if player is in range 
    private bool cooling;
    private float intTimer;
    #endregion

    private void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();



    }
    void Update()
    {
        if (inRange)
        {
            hit = Physics2D.Raycast(raycast.position, Vector2.left, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        //When player is detected
        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }
        if (inRange == false)
        {
            anim.SetBool("walk", false);
            StopAttack();
        }
    }

    private void OnTriggerEnter(Collider trig)
    {
        if(trig.gameObject.tag=="Player")
        {
            target = trig.gameObject;
            inRange = true;
        }
    }
    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, transform.transform.position);

        if (distance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if ( attackDistance >= distance && cooling == false )
        {
            Attack();
        }
        if(cooling)
        {
            Cooldown();
            anim.SetBool("attack", false);
        }
    }

    void Move()
    {
        anim.SetBool("walk", true);
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("02minoattack"))
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x,transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    void Attack()
    {
        timer = intTimer;
        attackMode = true;

        anim.SetBool("walk ", false);
        anim.SetBool("attack", true);
    }
    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }
    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("attack", false);

    }
    void RaycastDebugger()
    {
        if(distance > attackDistance)
        {
            Debug.DrawRay(raycast.position, Vector2.left * rayCastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
            Debug.DrawRay(raycast.position, Vector2.left * rayCastLength, Color.green);
        }
    }
    public void TriggerCooling ()
    {
        cooling = true;
    }
}
