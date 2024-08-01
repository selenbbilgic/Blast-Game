using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Rendering;
public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GridManager Instance;
    [SerializeField]
    public int width;
    public int height;
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
    List<Cube> cubesToRemove = new();


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
                FindCubesToRemove(selectedCube);
                RemoveCubes();
            }
        }
    }

    public Cube GetCubeAt(int _x, int _y){
        if(_x >= 0 && _x < width && _y >= 0 && _y < height){
            Node node = grid[_x, _y];
            if(node != null && node.item != null){
                return node.item.GetComponent<Cube>();
            }
        }
        return null;
    }

    #region check the cube click

    void FindCubesToRemove(Cube cube){
        cubesToRemove.Clear();

        Queue<Cube> queue = new Queue<Cube>();
        HashSet<Cube> visited = new HashSet<Cube>();

        queue.Enqueue(cube);
        visited.Add(cube);

        while(queue.Count > 0){
            Cube currentCube = queue.Dequeue();
            cubesToRemove.Add(currentCube);

            foreach (Cube adjacentCube in currentCube.GetAdjacentCubes())
            {
                if(!visited.Contains(adjacentCube) && currentCube.cubeType == adjacentCube.cubeType){
                    queue.Enqueue(adjacentCube);
                    visited.Add(adjacentCube);
                }
            }
        }
    }

    void RemoveCubes(){
        if(cubesToRemove.Count < 2) {return;}
        isProcessingMove = true;
        foreach ( Cube cubetoRemove in cubesToRemove)
        {
            int x = cubetoRemove.xIndex;
            int y = cubetoRemove.yIndex; 

            cubetoRemove.OnTapped();

            grid[x, y] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(grid[x, y].item == null){
                    RefillCubes(x, y);
                }
            }
        }

    }
    void RefillCubes(int x, int y){

        int yOffset = 1;
        isProcessingMove = true;

        while(y + yOffset < height && grid[x, y+yOffset].item == null){
            yOffset++;
        }

        if(y + yOffset < height && grid[x, y+yOffset].item != null){
            Cube cubeAbove = grid[x, y+yOffset].item.GetComponent<Cube>();
            Vector3 targetpos = new Vector3(x-spacingX, y-spacingY, cubeAbove.transform.position.z);

            cubeAbove.MoveToTarget(targetpos);
            cubeAbove.SetIndicies(x, y);
            grid[x, y] = grid[x, y+yOffset];
            grid[x, y+yOffset] = new Node(true, null);
        }

        if(y+yOffset == height){
            DropCubeAtTop(x);
        }

        isProcessingMove = false;

    }

    void DropCubeAtTop(int _x){
        int index = FindIndexOfLowestNull(_x);
        int locationToMove = height-index;

        GameObject newCube = Instantiate(GetPrefab("rand"), new Vector2(_x-spacingX , height-spacingY),  Quaternion.identity);
        newCube.transform.SetParent(cubeParent.transform);
        newCube.GetComponent<Cube>().SetIndicies(_x, index);

        grid[_x, index] = new Node(true, newCube);
        Vector3 targetPosition = new Vector3(newCube.transform.position.x, newCube.transform.position.y - locationToMove, newCube.transform.position.z);

        newCube.GetComponent<Cube>().MoveToTarget(targetPosition);

    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = height-1; y >= 0; y--)
        {
            if(grid[x,y].item == null){
                lowestNull = y;
            }
        }

        return lowestNull;
    }


    #endregion

}
