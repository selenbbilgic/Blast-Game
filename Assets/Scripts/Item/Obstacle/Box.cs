using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacle
{
    private ParticleSystem explosionParticles;
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        isDamaged = false;
        explosionParticles = GetComponentInChildren<ParticleSystem>();
    }

    public override void OnDamage(){
        if(!isDamaged){
            isDamaged = true;
            //animator.SetBool("Destroy", true);
            
            explosionParticles.Play();
            StartCoroutine(DestroyAfterAnimation());
            //Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

}
