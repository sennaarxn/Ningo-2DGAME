using UnityEngine;

public class ReturnState : StateMachineBehaviour
{
    private Transform _target;
    private Vector3 _defaultPosition;
    private float _speed = 2f;
    private float _chaseRange;
    private float _maxVerticalChaseDistance;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Minotaur mino = animator.GetComponent<Minotaur>();
        if (mino != null)
        {
            _defaultPosition = mino.defaultPosition;
            _chaseRange = mino.chaseRange;
            _maxVerticalChaseDistance = mino.maxVerticalChaseDistance;
        }

        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerManager.isGameOver || _target == null)
        {
            animator.SetBool("isReturning", false);
            return;
        }

        // Move towards default position
        animator.transform.position = Vector2.MoveTowards(
            animator.transform.position,
            _defaultPosition,
            _speed * Time.deltaTime
        );

        // Face the direction of movement
        animator.transform.localScale = new Vector3(
            _defaultPosition.x > animator.transform.position.x ? 0.35f : -0.35f,
            0.35f,
            1
        );

        // Check if reached default position
        if (Vector2.Distance(animator.transform.position, _defaultPosition) < 0.1f)
        {
            animator.SetBool("isReturning", false);
        }

        // Check if player is back in chase range
        float horizontalDistance = Mathf.Abs(_target.position.x - animator.transform.position.x);
        float verticalDistance = Mathf.Abs(_target.position.y - animator.transform.position.y);

        if (horizontalDistance < _chaseRange && verticalDistance <= _maxVerticalChaseDistance)
        {
            animator.SetBool("isReturning", false);
            animator.SetBool("isChasing", true);
        }
    }
}