using UnityEngine;

public class BottomTextAnimation : MonoBehaviour
{
    public Animator animator; // Animator bileþeni buraya baðlanacak.

    void Start()
    {
        animator.Play("BottomTextSlideIn"); // Animasyonu baþlat.
    }
}
