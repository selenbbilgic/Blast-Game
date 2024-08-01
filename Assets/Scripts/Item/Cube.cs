using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Item
{
    public CubeType cubeType;

    private Animator animator;
    private bool IsDestroyed = false;

    void Start(){
        animator = GetComponent<Animator>();
    }

    public override void OnTapped(){
        if(!IsDestroyed){
            IsDestroyed = true;
            animator.SetBool("Destroy", true);
            Debug.Log("destroyed");
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.9f);
        Destroy(gameObject);
    }
   
}

public enum CubeType{
    Blue,
    TNTEligableBlue,
    Green,
    TNTEligableGreen,
    Red,
    TNTEligableRed,
    Yellow,
    TNTEligableYellow,
}
