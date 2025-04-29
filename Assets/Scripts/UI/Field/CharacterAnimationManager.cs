using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    private Animator animator; // Animator ÄÄÆ÷³ÍÆ®

    void Start()
    {
        animator = GetComponent<Animator>(); // Animator
        animator.SetBool("New Bool", true);
    }
}
