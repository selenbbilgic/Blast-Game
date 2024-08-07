using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Item
{
    public CubeType cubeType;

    private Animator animator;
    private bool IsDestroyed = false;
    private SpriteRenderer spriteRenderer;
    public Sprite TntEligibleSprite; 
    public bool isTntEligible;

    void Start(){
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer==null)
        {
            Debug.LogError("No SpriteRenderer component found on this gameObject.");
        }
        isTntEligible = false;
        
    }

    public void ChangeSprite(){
        isTntEligible = true;
        if (spriteRenderer != null && TntEligibleSprite != null)
        {
            spriteRenderer.sprite = TntEligibleSprite;
            spriteRenderer.enabled = false;
            spriteRenderer.enabled = true;
        }
    }

    public override void OnDamage(){
        if(!IsDestroyed){
            IsDestroyed = true;
            animator.SetBool("Destroy", true);
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

   
}

public enum CubeType {
    Blue = 1,

    Green = 2,

    Red = 3,

    Yellow = 4,

}
