using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting.Dependencies.Sqlite;
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
    public Sprite[] TNTEligibleSprites;
    private string[] gridToBuild;
    private Node[,] grid;
    public GameObject[] gridGO;
    public GameObject cubeParent;

    [SerializeField]
    private Item selectedCube;

    [SerializeField]
    private bool isProcessingMove;
    List<Item> itemsToRemove = new();


    private void Awake(){
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        spacingY = (float)((height-1)/2) + 3;

        for (int y = 0; y  < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x-spacingX, y-spacingY);

                int index = y * width + x;
                string itemType = gridToBuild[index];

                GameObject item = Instantiate(GetPrefab(itemType), position, Quaternion.identity);
                item.transform.SetParent(cubeParent.transform);

                if (item.GetComponent<Item>() != null)
                {
                    item.GetComponent<Item>().SetIndicies(x, y);
                }
                grid[x, y] = new Node(true, item);

            }
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
        if(!GameManager.Instance.isGameEnded){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if(hit.collider != null)
        {
            Item item = hit.collider.gameObject.GetComponent<Item>();
            if(item != null){
                
                if(isProcessingMove){
                    return;
                }
                
                if(item is Cube){
                    selectedCube = (Cube)item;
                    FindCubesToRemove(selectedCube);

                    if(itemsToRemove.Count < 2) {return;}
                    GameManager.Instance.UpdateMoves();

                    isProcessingMove = true;

                    int cubeCount = itemsToRemove.OfType<Cube>().Count();
                    if (cubeCount >= 5) {
                            selectedCube.OnDamage();
                            itemsToRemove.Remove(selectedCube);
                            CreateTNT();
                    }
                }
                else if (item is TNT){
                    TNT tnt = (TNT)item;
                    selectedCube = tnt;
                    tnt.OnDamage();
                    GameManager.Instance.UpdateMoves();

                    itemsToRemove.Clear();
                    FindItemsToExplode(tnt);
                }
            }

            RemoveItems();
        }
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
                    else if (adjacentItem is Obstacle)
                    {
                        visited.Add(adjacentItem);
                        if(adjacentItem is Box || adjacentItem is Vase){
                            itemsToRemove.Add(adjacentItem);
                        }
                    }
                    else
                    {
                        visited.Add(adjacentItem);
                    }
                }
            }
        }
    }

    void CheckTNTEligibility(){
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(grid[x,y] != null){
                    Item item = grid[x,y].item.GetComponent<Item>();
                    if(item is Cube cube){
                        if(!cube.isTntEligible){
                            FindCubesToRemove(cube);

                            int cubeCount = itemsToRemove.OfType<Cube>().Count();
                            if (cubeCount >= 5) {
                                cube.ChangeSprite();
                            }
                        }
                        
                    }
                }
            }
        }
    }

    #endregion

    #region item dropping

    void RemoveItems(){

        foreach ( Item itemtoRemove in itemsToRemove)
        {
            int x = itemtoRemove.xIndex;
            int y = itemtoRemove.yIndex; 

            if(itemtoRemove is Vase vase){
                if (!vase.isDamaged){
                    itemtoRemove.OnDamage();
                }
                else{
                    itemtoRemove.OnDamage();
                    GameManager.Instance.UpdateItems(itemtoRemove);
                    GameManager.Instance.CheckWinState();
                    grid[x, y] = new Node(true, null);
                }
            }
            else{
                itemtoRemove.OnDamage();
                GameManager.Instance.UpdateItems(itemtoRemove);
                GameManager.Instance.CheckWinState();
                grid[x, y] = new Node(true, null);
            }
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

    void RefillCubes(int x, int y){

        int yOffset = 1;
        isProcessingMove = true;

        while(y + yOffset < height && grid[x, y+yOffset].item == null){
            yOffset++;
        }

        if(y + yOffset < height && grid[x, y+yOffset].item != null){
            Item itemAbove = grid[x, y+yOffset].item.GetComponent<Item>();
            if(itemAbove is not Stone){
                Vector3 targetpos = new Vector3(x-spacingX, y-spacingY, itemAbove.transform.position.z);
                itemAbove.MoveToTarget(targetpos);
                itemAbove.SetIndicies(x, y);
                grid[x, y] = grid[x, y+yOffset];
                grid[x, y+yOffset] = new Node(true, null);
            }
            
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

    #endregion

    #region helpers

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = height + 1;
        for (int y = height-1; y >= 0; y--)
        {
            var currentItem = grid[x, y].item;

            if(currentItem == null){
                lowestNull = y;
            }

            else if(currentItem.GetComponent<Item>() is Stone){
                return lowestNull;
            }
        }
        return lowestNull;
    }

    void CreateTNT(){

        GameObject tnt = Instantiate(GetPrefab("TNT"), new Vector2(selectedCube.xIndex - spacingX , selectedCube.yIndex - spacingY), Quaternion.identity);
        tnt.transform.SetParent(cubeParent.transform);
        tnt.GetComponent<TNT>().SetIndicies(selectedCube.xIndex, selectedCube.yIndex);
        grid[selectedCube.xIndex, selectedCube.yIndex] = new Node(true, tnt);

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
            case "v2": return itemPrefabs[12];
            case "t": return itemPrefabs[13]; 
            case "rand":
                int randIndex = Random.Range(0, 4);
                return itemPrefabs[randIndex];
            default: return null;
        }
    }

    #endregion
}
