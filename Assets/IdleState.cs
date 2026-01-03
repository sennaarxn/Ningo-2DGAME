using UnityEngine;

public class IdleState : StateMachineBehaviour
{
    Transform target;
    Transform borderCheckRight;
    Transform borderCheckLeft;
    float chaseStartRange = 5f;
    float maxVerticalChaseDistance;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Minotaur mino = animator.GetComponent<Minotaur>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            target = null; // No player in scene
        }

        if (mino != null)
        {
            borderCheckRight = mino.borderCheckRight;
            borderCheckLeft = mino.borderCheckLeft;
            maxVerticalChaseDistance = mino.maxVerticalChaseDistance;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerManager.isGameOver) return;
        if (target == null) return; // Player not found

        bool isGroundedRight = Physics2D.Raycast(borderCheckRight.position, Vector2.down, 2f);
        bool isGroundedLeft = Physics2D.Raycast(borderCheckLeft.position, Vector2.down, 2f);
        if (!isGroundedRight && !isGroundedLeft) return;

        float horizontalDistance = Mathf.Abs(target.position.x - animator.transform.position.x);
        float verticalDistance = Mathf.Abs(target.position.y - animator.transform.position.y);

        if (horizontalDistance < chaseStartRange && verticalDistance <= maxVerticalChaseDistance)
        {
            animator.SetBool("isChasing", true);
        }
    }
}
