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
    private Item selectedCube;

    [SerializeField]
    private bool isProcessingMove;
    List<Item> itemsToRemove = new();


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
            case "TNT": return itemPrefabs[8];
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

    #region check the item click
    void SelectCube()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        Debug.Log("selen1.");
        if(hit.collider != null)
        {
            Debug.Log("selen2");
            Item item = hit.collider.gameObject.GetComponent<Item>();
            if(item != null){
                
                if(isProcessingMove){
                    return;
                }
                if(item is Cube){
                    selectedCube = (Cube)item;
                    FindCubesToRemove(selectedCube);

                    if(itemsToRemove.Count < 2) {return;}
                    isProcessingMove = true;

                    if(itemsToRemove.Count > 4) {
                        selectedCube.OnTapped();
                        itemsToRemove.Remove(selectedCube);
                        CreateTNT();
                    }
                }
                else if (item is TNT){
                    TNT tnt = (TNT)item;
                    selectedCube = tnt;
                    tnt.OnTapped();
                    itemsToRemove.Clear();
                    FindItemsToExplode(tnt);
                }
            }

            RemoveItems();
        }
    }

    void FindItemsToExplode(TNT tnt){
        
        int range = 2;

        for (int x = tnt.xIndex - range; x <= tnt.xIndex+range; x++)
        {
            for (int y = tnt.yIndex - range; y <= tnt.yIndex + range; y++)
            {
                if(x>=0 && x<width && y>=0 && y<height){
                    Item item = GetItemAt(x,y);
                    if(item != null && !itemsToRemove.Contains(item)){

                        if(item is TNT && item != tnt){
                            FindItemsToExplode((TNT)item);
                        }
                        else{
                            itemsToRemove.Add(item);
                        }
                        
                    }
                }
            }
        }
    }


    void FindCubesToRemove(Item item){
        itemsToRemove.Clear();

        Queue<Item> queue = new Queue<Item>();
        HashSet<Item> visited = new HashSet<Item>();

        queue.Enqueue(item);
        visited.Add(item);

        while(queue.Count > 0){
            Item currentItem = queue.Dequeue();
            itemsToRemove.Add(currentItem);

            foreach (Item adjacentItem in GetAdjacentItems(currentItem.xIndex, currentItem.yIndex))
            {
                if (!visited.Contains(adjacentItem))
                {
                    if (adjacentItem is Cube)
                    {
                        Cube adjCube = (Cube)adjacentItem;
                        if (adjCube.cubeType == ((Cube)item).cubeType)
                        {
                            queue.Enqueue(adjCube);
                            visited.Add(adjCube);
                        }
                    }
                    else if (adjacentItem is TNT)
                    {
                        visited.Add(adjacentItem);
                    }
                    else if (adjacentItem is Obstacle)
                    {
                        visited.Add(adjacentItem);
                    }
                }
            }
        }
    }

    public List<Item> GetAdjacentItems(int xIndex, int yIndex)
    {
        List<Item> adjacentItems = new List<Item>();

        if (xIndex > 0 && GetItemAt(xIndex - 1, yIndex) != null)
        {
            adjacentItems.Add(GetItemAt(xIndex - 1, yIndex));
        }

        if (xIndex < width - 1 && GetItemAt(xIndex + 1, yIndex) != null)
        {
            adjacentItems.Add(GetItemAt(xIndex + 1, yIndex));
        }

        if (yIndex < height - 1 && GetItemAt(xIndex, yIndex + 1) != null)
        {
            adjacentItems.Add(GetItemAt(xIndex, yIndex + 1));
        }

        if (yIndex > 0 && GetItemAt(xIndex, yIndex - 1) != null)
        {
            adjacentItems.Add(GetItemAt(xIndex, yIndex - 1));
        }

        return adjacentItems;
    }
    public Item GetItemAt(int _x, int _y){
        if(_x >= 0 && _x < width && _y >= 0 && _y < height){
            Node node = grid[_x, _y];
            if(node != null && node.item != null){
                return node.item.GetComponent<Item>();
            }
        }
        return null;
    }

    void RemoveItems(){

        List<TNT> tnts = new();

        foreach ( Item itemtoRemove in itemsToRemove)
        {

            int x = itemtoRemove.xIndex;
            int y = itemtoRemove.yIndex; 
            itemtoRemove.OnTapped();
            Debug.Log(x + "selennenenenne nullinggg" + y);
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
        isProcessingMove = false;
    }

    void CreateTNT(){

        GameObject tnt = Instantiate(GetPrefab("TNT"), new Vector2(selectedCube.xIndex - spacingX , selectedCube.yIndex - spacingY), Quaternion.identity);
        tnt.transform.SetParent(cubeParent.transform);
        tnt.GetComponent<TNT>().SetIndicies(selectedCube.xIndex, selectedCube.yIndex);
        grid[selectedCube.xIndex, selectedCube.yIndex] = new Node(true, tnt);

        Debug.Log("TNT is crearted!!!!!!");

    }

    void RefillCubes(int x, int y){

        int yOffset = 1;
        isProcessingMove = true;

        while(y + yOffset < height && grid[x, y+yOffset].item == null){
            yOffset++;
        }

        if(y + yOffset < height && grid[x, y+yOffset].item != null){
            Item itemAbove = grid[x, y+yOffset].item.GetComponent<Item>();
            Vector3 targetpos = new Vector3(x-spacingX, y-spacingY, itemAbove.transform.position.z);
            itemAbove.MoveToTarget(targetpos);
            itemAbove.SetIndicies(x, y);
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
