using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public virtual void Initialize(int _x, int _y){
        xIndex = _x;
        yIndex = _y;
    }

    public virtual void SetIndicies(int _x, int _y){
        xIndex = _x;
        yIndex = _y;
    }
    public abstract void OnTapped();
}
