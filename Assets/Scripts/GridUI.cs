using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridUI : MonoBehaviour
{
    
    public static GridUI Instance;
    public GameObject gridBackgroundPrefab;
    public TMP_Text boxCountText;
    public TMP_Text stoneCountText;
    public TMP_Text vaseCountText;
    public TMP_Text moveCountText;
    private void Awake(){
        Instance = this;
    }

    void Start()
    {
        CreateGridBackground();

        // boxCountText.text = GameManager.Instance.boxCount.ToString();
        // stoneCountText.text = GameManager.Instance.stoneCount.ToString();
        // vaseCountText.text = GameManager.Instance.vaseCount.ToString();
        // moveCountText.text = GameManager.Instance.moves.ToString();
    }

    void Update(){
        boxCountText.text = GameManager.Instance.boxCount.ToString();
        stoneCountText.text = GameManager.Instance.stoneCount.ToString();
        vaseCountText.text = GameManager.Instance.vaseCount.ToString();
        moveCountText.text = GameManager.Instance.moves.ToString();
    }

    void CreateGridBackground()
    {

        GameObject bg = Instantiate(gridBackgroundPrefab, transform);
        RectTransform rt = bg.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(GridManager.Instance.width * 92, GridManager.Instance.height*92); 
        rt.SetAsFirstSibling(); 

        Debug.Log("grid background is created");
    }

    
}
