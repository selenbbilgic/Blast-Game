using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public bool isMoving;
    public virtual void Initialize(int _x, int _y){
        xIndex = _x;
        yIndex = _y;
    }

    public virtual void SetIndicies(int _x, int _y){
        xIndex = _x;
        yIndex = _y;  
    }

        public void MoveToTarget(Vector2 _targetPos){
        StartCoroutine(MoveCoroutine(_targetPos));
    }

    public IEnumerator MoveCoroutine(Vector2 _targetPos){
        isMoving = true;
        float duration = 0.3f;

        Vector2 startPosition = transform.position;
        float elaspedTime = 0f;

        while(elaspedTime<duration){
            float t = elaspedTime / duration;
            transform.position = Vector2.Lerp(startPosition, _targetPos, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = _targetPos;
        isMoving = false;

    }

    public abstract void OnTapped();
}
