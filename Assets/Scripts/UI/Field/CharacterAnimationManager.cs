using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    private Animator animator; // Animator ������Ʈ

    void Start()
    {
        animator = GetComponent<Animator>(); // Animator
        animator.SetBool("New Bool", true);
    }
}
