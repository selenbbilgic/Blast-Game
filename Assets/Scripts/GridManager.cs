using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Rendering;
public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GridManager Instance;
    [SerializeField]
    private int width;
    private int height;
    public float spacingX;
    public float spacingY;
    public GameObject[] itemPrefabs;
    private string[] gridToBuild;
    private Node[,] grid;
    public GameObject[] gridGO;
    public GameObject cubeParent;
    public GameObject obstacleParent;
    [SerializeField]
    private Cube selectedCube;

    [SerializeField]
    private bool isProcessingMove;

    private void Awake(){
        Instance = this;
    }

    void Start(){
        width = GameManager.Instance.currentLevelData.grid_width;
        height = GameManager.Instance.currentLevelData.grid_height;
        gridToBuild = GameManager.Instance.currentLevelData.grid;
        InitializeGrid();
    }

    void InitializeGrid(){
        grid = new Node[width, height];
        spacingX = (float)(width-1)/2;
        spacingY = (float)((height-1)/2) +1;

        for (int y = 0; y  < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x-spacingX, y-spacingY);

                int index = y * width + x;
                string itemType = gridToBuild[index];

                GameObject item = Instantiate(GetPrefab(itemType), position, Quaternion.identity);
                item.transform.SetParent(cubeParent.transform);

                Debug.Log(item);

                if (item.GetComponent<Cube>() != null)
                {
                    item.GetComponent<Cube>().SetIndicies(x, y);
                }
                grid[x, y] = new Node(true, item);

            }
        }
    }

    GameObject GetPrefab(string itemType)
    {
        switch (itemType)
        {
            case "r": return itemPrefabs[0]; 
            case "g": return itemPrefabs[1]; 
            case "b": return itemPrefabs[2]; 
            case "y": return itemPrefabs[3]; 
            case "bo": return itemPrefabs[9]; 
            case "s": return itemPrefabs[10]; 
            case "v": return itemPrefabs[11];
            case "rand":
                int randIndex = Random.Range(0, 4);
                return itemPrefabs[randIndex];
            default: return null;
        }
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            SelectCube();
        }
    }

    void SelectCube()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if(hit.collider != null && hit.collider.gameObject.GetComponent<Cube>())
        {
            Cube cube = hit.collider.GetComponent<Cube>();
            if (cube != null)
            {
                if(isProcessingMove){
                    return;
                }
                
                selectedCube = cube;
                selectedCube.OnTapped();
            }
        }
    }

}
