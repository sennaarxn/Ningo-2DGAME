using UnityEngine;

public class ChestAnimation : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayOpenAnimation()
    {
        anim.SetBool("isOpen", true);
    }
}