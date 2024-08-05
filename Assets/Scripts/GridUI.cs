using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridUI : MonoBehaviour
{
    public static GridUI Instance;
    public GameObject gridBackgroundPrefab;

    public GameObject boxGoalItem; 
    public GameObject stoneGoalItem; 
    public GameObject vaseGoalItem; 
    //public GameObject goalCheck;

    public TMP_Text moveCountText;

    public GridLayoutGroup goalContainer;

    private int boxCount;
    private int stoneCount;
    private int vaseCount;


    private TMP_Text tempText;
    private void Awake(){
        Instance = this;
    }

    void Start()
    {
        CreateGridBackground();

        boxCount = GameManager.Instance.boxCount;
        stoneCount = GameManager.Instance.stoneCount;
        vaseCount = GameManager.Instance.vaseCount;

        SetGoalItem(boxGoalItem, boxCount);
        SetGoalItem(stoneGoalItem, stoneCount);
        SetGoalItem(vaseGoalItem, vaseCount);

        AdjustLayout();
    }

    void Update(){
        moveCountText.text = GameManager.Instance.moves.ToString();

        boxCount = GameManager.Instance.boxCount;
        stoneCount = GameManager.Instance.stoneCount;
        vaseCount = GameManager.Instance.vaseCount;

        UpdateGoalItem(boxGoalItem , boxCount);
        UpdateGoalItem(stoneGoalItem, stoneCount);
        UpdateGoalItem(vaseGoalItem, vaseCount);
        
    }

    void UpdateGoalItem(GameObject goalItem, int count){
        Image checkmark = null;
        tempText = goalItem.GetComponentInChildren<TMP_Text>();

        foreach (Transform child in goalItem.transform)
        {
            checkmark = child.GetComponentInChildren<Image>(true);
            if (checkmark != null && checkmark.gameObject != goalItem)
            {
                break;
            }
        }

            if(count > 0){
                if(tempText != null){
                    tempText.text = count.ToString();
                }
            }
            else{
                if (tempText != null)
                {
                    tempText.gameObject.SetActive(false); 
                }
                if (checkmark != null)
                {
                    checkmark.gameObject.SetActive(true); 
                }
            }
        
    }


    void SetGoalItem(GameObject goalItem, int count)
    {
        if (count > 0)
        {
            goalItem.SetActive(true);
            TMP_Text countText = goalItem.GetComponentInChildren<TMP_Text>();
            if (countText != null)
            {
                countText.text = count.ToString();
                //checkmark.gameObject.SetActive(false);
            }
        }
        else
        {
            goalItem.SetActive(false);
        }
    }

    void AdjustLayout()
    {
        int activeItemCount = 0;

        foreach (Transform child in goalContainer.transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeItemCount++;
            }
        }
        if (activeItemCount == 1)
        {
            goalContainer.cellSize = new Vector2(130, 130); 
            goalContainer.spacing = new Vector2(0, 0);
        }
        else
        {
            goalContainer.cellSize = new Vector2(100, 100); 
            goalContainer.spacing = new Vector2(10, 10); 
        }
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
    

