using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Obstacle
{

    private void Start()
    {
        isDamaged = false;
    }

    public override void OnDamage()
    {
        
        if (!isDamaged)
        {
            isDamaged = true;
            Destroy(gameObject);
        }
    }

}
