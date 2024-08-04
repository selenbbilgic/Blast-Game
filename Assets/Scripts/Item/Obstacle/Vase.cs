using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : Obstacle
{
    public GameObject damagedVasePrefab;

    public Sprite damagedVaseSprite; 
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        isDamaged = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer component found on this gameObject.");
        }
    }

    public override void OnDamage()
    {
        
        if (!isDamaged)
        {
            ReplaceWithDamagedSprite();

        } else{
            Debug.Log("vase is destroyinggg");
            Destroy(gameObject);
        }
    }

    void ReplaceWithDamagedSprite()
    {
        isDamaged = true;
        
        if (spriteRenderer != null && damagedVaseSprite != null)
        {
            spriteRenderer.sprite = damagedVaseSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer or damagedVaseSprite is not assigned.");
        }
    }

}
