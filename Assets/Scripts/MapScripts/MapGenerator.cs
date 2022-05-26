using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public GameObject placableMapTile;
    public GameObject enemiesPathMapTile;
    public GameObject entrancePathMapTile;
    public GameObject exitPathMapTile;

    public GameObject waterMapTile;
    public GameObject hillMapTile;
    public GameObject abyssMapTile;
    public GameObject treeMapTile;

    public static List<GameObject> mapTiles = new List<GameObject>(); /** entire map */
    public static List<List<GameObject>> pathsTiles = new List<List<GameObject>>(); /** enemies paths */
    public static List<GameObject> barrierTiles = new List<GameObject>(); /** tower placement blocked tiles */

    /** paths of enemies (can edit here)*/ 
    private string[] enemyPaths = {
        "S(4,0)", "D3", "L1", "D2", "R2", "U2", "L2", "D2", "R2", "U2", "L2", "D2", "R2", "U2", "L1", "U3", "E", 
        "S(4, 8)", "U3", "R1", "U2", "L2", "D2", "R2", "U2", "L2", "D2", "R2", "U2", "L2", "D2", "R1", "D3", "E",
        "S(0, 4)", "R3", "D1", "R2", "U2", "L2", "D2", "R2", "U2", "L2", "D2", "R2", "U2", "L2", "D1", "L3", "E",
        "S(8, 4)", "L3", "U1", "L2", "D2", "R2", "U2", "L2", "D2", "R2", "U2", "L2", "D2", "R2", "U1", "R3", "E"
    };
    // private string[] enemyPaths = {"S(5,0)", "D4", "L3", "U2", "L2", "E", "S(5,0)", "D6", "R1", "D1", "R2", "E"};

    private string[] blockedPaths = {};
    // private string[] blockedPaths = {"W(4,2)", "H(4,3)", "A(6,3)", "T(6,4)"};

    private int mapHeight = 9;
    private int mapWidth = 9;

    // Start is called before the first frame update
    private void Start()
    {
        generateMap();        
    }

    private void generateMap()
    {
        /**generate entire map */
        for (int y = 0; y < mapHeight; y++) 
        {
            for (int x = 0; x < mapWidth; x++) 
            {
                GameObject newMapTile = Instantiate(placableMapTile);
                mapTiles.Add(newMapTile);
                newMapTile.transform.position = new Vector2(x - 7, -y + 4);
            }
        }

        /**generate enemy path from code */
        Vector2 currentPosition = new Vector2(0, 0);
        int path = 0;
        for (int i = 0; i < enemyPaths.Length; i++)
        {
            string[] seperatingChars = { "S", "L", "R", "D", "U", "E", "(", ",", ")", " " };
            string[] encodedNumbers = enemyPaths[i].Split(seperatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            /**start at position (x,y) */
            if(enemyPaths[i].Contains("S"))
            {
                /**encode position x and y */
                pathsTiles.Add(new List<GameObject>());
                EnemiesOnMap.enemiesOnMap.Add(new List<GameObject>());
                GameObject newPathTile = Instantiate(entrancePathMapTile);
                pathsTiles[path].Add(newPathTile);
                currentPosition = new Vector2(int.Parse(encodedNumbers[0]) - 7, -int.Parse(encodedNumbers[1]) + 4);
                newPathTile.transform.position = currentPosition;
            }
            /**create path on left of current position */
            else if(enemyPaths[i].Contains("L"))
            {
                currentPosition = addMultipleTiles(pathsTiles[path], currentPosition, x: -int.Parse(encodedNumbers[0]));
            }
            /**create path on right of current position */
            else if(enemyPaths[i].Contains("R"))
            {
                currentPosition = addMultipleTiles(pathsTiles[path], currentPosition, x: int.Parse(encodedNumbers[0]));
            }
            /**create path on bottom (down) of current position */
            else if(enemyPaths[i].Contains("D"))
            {
                currentPosition = addMultipleTiles(pathsTiles[path], currentPosition, y: -int.Parse(encodedNumbers[0]));
            }
            /**create path on above (up) of current position */
            else if(enemyPaths[i].Contains("U"))
            {
                currentPosition = addMultipleTiles(pathsTiles[path], currentPosition, y: int.Parse(encodedNumbers[0]));
            }
            /**end of path */
            else if(enemyPaths[i].Contains("E"))
            {
                Destroy(pathsTiles[path][pathsTiles[path].Count - 1]);
                pathsTiles[path].RemoveAt(pathsTiles[path].Count - 1);
                GameObject newPathTile = Instantiate(exitPathMapTile);  
                pathsTiles[path].Add(newPathTile);
                newPathTile.transform.position = currentPosition;
                path++;
            }
        }

        /** generate barrier tile */
        for(int i = 0; i < blockedPaths.Length; i++)
        {
            
            string[] seperatingChars = { "W", "H", "A", "T", "(", ",", ")", " " };
            string[] encodedNumbers = blockedPaths[i].Split(seperatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            GameObject newMapTile = null;

            if(blockedPaths[i].Contains("W"))
            {
                newMapTile = Instantiate(waterMapTile);
            }
            else if(blockedPaths[i].Contains("H"))
            {
                newMapTile = Instantiate(hillMapTile);
            }
            else if(blockedPaths[i].Contains("A"))
            {
                newMapTile = Instantiate(abyssMapTile);
            }
            else if(blockedPaths[i].Contains("T"))
            {
                newMapTile = Instantiate(treeMapTile);
            }

            barrierTiles.Add(newMapTile);
            newMapTile.transform.position = new Vector2(int.Parse(encodedNumbers[0]) - 7, -int.Parse(encodedNumbers[1]) + 4);
        }
    }

    /**move tile up/down left/right from current position */
    private Vector2 addMultipleTiles(List<GameObject> pathTiles, Vector2 currentPosition, int x = 0, int y = 0)
    {
        if(x > 0)
        {
            for (int i = 0; i < x; i++)
            {
                GameObject newPathTile = Instantiate(enemiesPathMapTile);
                pathTiles.Add(newPathTile);
                currentPosition = currentPosition + new Vector2(1, 0);
                newPathTile.transform.position = currentPosition;
            }
        }
        else if(x < 0)
        {
            for (int i = 0; i > x; i--)
            {
                GameObject newPathTile = Instantiate(enemiesPathMapTile);
                pathTiles.Add(newPathTile);
                currentPosition = currentPosition + new Vector2(-1, 0);
                newPathTile.transform.position = currentPosition;
            }
        }
        if(y > 0)
        {
            for (int i = 0; i < y; i++)
            {
                GameObject newPathTile = Instantiate(enemiesPathMapTile);
                pathTiles.Add(newPathTile);
                currentPosition = currentPosition + new Vector2(0, 1);
                newPathTile.transform.position = currentPosition;
            }
        }
        else if(y < 0)
        {
            for (int i = 0; i > y; i--)
            {
                GameObject newPathTile = Instantiate(enemiesPathMapTile);
                pathTiles.Add(newPathTile);
                currentPosition = currentPosition + new Vector2(0, -1);
                newPathTile.transform.position = currentPosition;
            }
        }
        return currentPosition;
    }
}
