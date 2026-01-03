using UnityEngine;

public class ChaseState : StateMachineBehaviour
{
    Transform target;
    public float speed = 3f;
    Transform borderCheckRight;
    Transform borderCheckLeft;
    float chaseRange;
    float maxVerticalChaseDistance;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Minotaur mino = animator.GetComponent<Minotaur>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        borderCheckRight = mino.borderCheckRight;
        borderCheckLeft = mino.borderCheckLeft;
        chaseRange = mino.chaseRange;
        maxVerticalChaseDistance = mino.maxVerticalChaseDistance;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerManager.isGameOver)
        {
            
            return;
        }


        Vector2 minotaurPos = animator.transform.position;
        Vector2 playerPos = target.position;

        float horizontalDistance = Mathf.Abs(playerPos.x - minotaurPos.x);
        float verticalDistance = Mathf.Abs(playerPos.y - minotaurPos.y);

        if (horizontalDistance > chaseRange || verticalDistance > maxVerticalChaseDistance)
        {
            animator.SetBool("isChasing", false);
            animator.SetBool("isReturning", true);
            return;
        }

        Vector2 currentPosition = animator.transform.position;
        Vector2 targetPosition = new Vector2(target.position.x, currentPosition.y);
        animator.transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        bool isGroundedRight = Physics2D.Raycast(borderCheckRight.position, Vector2.down, 2f);
        bool isGroundedLeft = Physics2D.Raycast(borderCheckLeft.position, Vector2.down, 2f);

        if (!isGroundedRight && !isGroundedLeft)
        {
            animator.SetBool("isChasing", false);
            return;
        }

        float distance = Vector2.Distance(target.position, animator.transform.position);
        if (distance < 0.8f)
            animator.SetBool("isAttacking", true);
    }
}
