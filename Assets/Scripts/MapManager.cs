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

    public GameObject[] floors;
    public GameObject[] walls;
    public GameObject[] enemies;
    public GameObject player;
    public GameObject exit;

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
                    toInstantiate = walls[Random.Range(0, walls.Length)];
                } else
                {
                    toInstantiate = floors[Random.Range(0, floors.Length)];
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
                    case '#':
                        toInstantiate = walls[Random.Range(0, walls.Length)];
                        break;
                    case '.':
                        toInstantiate = floors[Random.Range(0, floors.Length)];
                        break;
                    case 'P':
                        toInstantiate = floors[Random.Range(0, floors.Length)];
                        Instantiate(player, new Vector3(x, y, 0f), Quaternion.identity);
                        break;
                    case 'X':
                        toInstantiate = floors[Random.Range(0, floors.Length)];
                        Instantiate(exit, new Vector3(x, y, 0f), Quaternion.identity);
                        break;
                    case 'E':
                        toInstantiate = floors[Random.Range(0, floors.Length)];
                        Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(x, y, 0f), Quaternion.identity);
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
