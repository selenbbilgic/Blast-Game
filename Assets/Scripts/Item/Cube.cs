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
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    public List<Cube> GetAdjacentCubes()
    {
        List<Cube> adjacentCubes = new List<Cube>();

        if (xIndex > 0 && GridManager.Instance.GetCubeAt(xIndex - 1, yIndex) != null)
        {
            Debug.Log("seloşş" + GridManager.Instance.GetCubeAt(xIndex - 1, yIndex).cubeType);
            adjacentCubes.Add(GridManager.Instance.GetCubeAt(xIndex - 1, yIndex));
        }

        if (xIndex < GridManager.Instance.width - 1 && GridManager.Instance.GetCubeAt(xIndex + 1, yIndex) != null)
        {
            adjacentCubes.Add(GridManager.Instance.GetCubeAt(xIndex + 1, yIndex));
        }

        if (yIndex < GridManager.Instance.height - 1 && GridManager.Instance.GetCubeAt(xIndex, yIndex + 1) != null)
        {
            adjacentCubes.Add(GridManager.Instance.GetCubeAt(xIndex, yIndex + 1));
        }

        if (yIndex > 0 && GridManager.Instance.GetCubeAt(xIndex, yIndex - 1) != null)
        {
            adjacentCubes.Add(GridManager.Instance.GetCubeAt(xIndex, yIndex - 1));
        }

        return adjacentCubes;
    }
   
}

public enum CubeType{
    Blue,

    Green,

    Red,

    Yellow,

}
