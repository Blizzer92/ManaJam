using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int height = 8;
    public int width = 8;

    public GameObject[] floors;
    public GameObject[] walls;
    public GameObject[] enemies;


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

    public void Setup()
    {
        SetUpList();
        MapSetup();
        AddItemOnRandomPosition(enemies, 1, 1);
    }
}
