using UnityEngine;

public class BottomTextAnimation : MonoBehaviour
{
    public Animator animator; // Animator bile�eni buraya ba�lanacak.

    void Start()
    {
        animator.Play("BottomTextSlideIn"); // Animasyonu ba�lat.
    }
}
