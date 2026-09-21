using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportOut;
    public Collider2D otherCollider;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        animator.Play("Teleport");
        collision.gameObject.transform.position = teleportOut.position;
        otherCollider.enabled = false;
        Animator anim2;
        if(otherCollider.gameObject.TryGetComponent<Animator>(out anim2))
        {
            anim2.Play("Teleport");
        }
        yield return new WaitForSeconds(1);
        otherCollider.enabled = true;
    }
}
