using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public int height = 8;
    public int width = 8;

    public GameObject wall_l;
    public GameObject wall_L;
    public GameObject wall_r;
    public GameObject wall_R;
    public GameObject wall_1;
    public GameObject wall_2;
    public GameObject wall_3;
    public GameObject wall_4;
    public GameObject wall_Viertelgeviertstrich;
    public GameObject wall__;
    public GameObject floor;
    public GameObject regal_a;
    public GameObject regal_b;
    public GameObject regal_c;
    public GameObject regal_d;
    public GameObject player;
    public GameObject exit;
    public GameObject enemies;
    
    private List<Vector2> gridPositions = new();
    private Transform map;


    void SetUpList()
    {
        gridPositions.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridPositions.Add(new Vector2(x, y));
            }
        }

    }

    Vector2 GetRandomPositionOnMap()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void MapSetup()
    {
        map = new GameObject("Map").transform;

        for (int x = -1; x < width + 1; x++)
        {
            for (int y = -1; y < height + 1; y++)
            {
                GameObject toInstantiate;

                if (x == -1 || x == width || y == -1 || y == height)
                {
                    toInstantiate = wall_1;
                } else
                {
                    toInstantiate = floor;
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(map);
            }
        }
    }

    void MapSetup2(int level)
    {
        map = new GameObject("Map").transform;

        string[] f = GameManager.instance.levels[level].text.Split(new string[] {"\n", "\r", "\r\n"}, 
            System.StringSplitOptions.RemoveEmptyEntries);
        
        Array.Reverse(f);
        
        for (int y = 0; y < f.Length; y++)
        {
            for (int x = 0; x < f[y].Length; x++)
            {
                GameObject toInstantiate = null;

                switch (f[y][x])
                {
                    case 'l':
                        toInstantiate = wall_l;
                        break;
                    case 'L':
                        toInstantiate = wall_L;
                        break;
                    case 'r':
                        toInstantiate = wall_r;
                        break;
                    case 'R':
                        toInstantiate = wall_R;
                        break;
                    case '1':
                        toInstantiate = wall_1;
                        break;
                    case '2':
                        toInstantiate = wall_2;
                        break;
                    case '3':
                        toInstantiate = wall_3;
                        break;
                    case '4':
                        toInstantiate = wall_4;
                        break;
                    case '-':
                        toInstantiate = wall_Viertelgeviertstrich;
                        break;
                    case '_':
                        toInstantiate = wall__;
                        break;
                    case '.':
                        toInstantiate = floor;
                        break;
                    case 'a':
                        toInstantiate = regal_a;
                        break;
                    case 'b':
                        toInstantiate = regal_b;
                        break;
                    case 'c':
                        toInstantiate = regal_c;
                        break;
                    case 'd':
                        toInstantiate = regal_d;
                        break;
                    case 'P':
                        toInstantiate = floor;
                        Instantiate(player, new Vector3(x, y, 0f), Quaternion.identity);
                        break;
                    case 'B':
                        toInstantiate = floor;
                        Instantiate(exit, new Vector3(x, y, 0f), Quaternion.identity);
                        break;
                    case 'X':
                        toInstantiate = floor;
                        Instantiate(enemies, new Vector3(x, y, 0f), Quaternion.identity);
                        break;
                }

                if (toInstantiate != null)
                {
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(map);
                }
            }
        }
        
    }
    
    void AddItemOnRandomPosition(GameObject[] objectArray, int min, int max)
    {
        int count = Random.Range(min, max + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 position = GetRandomPositionOnMap();
            GameObject tiled = objectArray[Random.Range(0, objectArray.Length)];

            Instantiate(tiled, position, Quaternion.identity);
        }
    }

    public void Setup(int level)
    {
        //SetUpList();
        MapSetup2(level);
        //AddItemOnRandomPosition(enemies, 20, 20);
        //Instantiate(exit, new Vector2(width - 1, height - 1), Quaternion.identity);
    }
}
