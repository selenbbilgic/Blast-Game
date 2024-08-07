using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isUsable;
    public GameObject item;

    public Node(bool _isUsable, GameObject _item){
        isUsable = _isUsable;
        item = _item;
    }
}
