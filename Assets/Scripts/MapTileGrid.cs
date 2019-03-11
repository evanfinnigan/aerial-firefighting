using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileGrid : MonoBehaviour {

    public int size;

    public MapTile mapTilePrefab;
    public MapTile[,] map;

    public float elevationMin;
    public float elevationMax;

    MapTileMesh mapTileMesh;

    private void Awake()
    {
        elevationMin = MapTileMetrics.elevationMin;
        elevationMax = MapTileMetrics.elevationMax;

        mapTileMesh = GetComponentInChildren<MapTileMesh>();

        GenerateMap();
    }

    private void Start()
    {
        mapTileMesh.Triangulate(map);
    }

    public MapTileGrid GenerateMap()
    {
        // Generate Map Size
        size = Mathf.FloorToInt(Random.Range(8, 12));//(15, 40));

        map = new MapTile[size,size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                MapTile t = Instantiate<MapTile>(mapTilePrefab);

                // position
                map[i, j] = t;
                float slope = //(i < size / 2 ? i : size - (i+1)) * (j < size / 2 ? j : size - (j+1));
                              (i < 2 ? i*0.25f : 1f) * (i > size - 3 ? (size - i - 1)*0.25f : 1f) 
                              * (j < 2 ? j * 0.25f : 1f) * (j > size - 3 ? (size - j - 1) * 0.25f : 1f);
                float elevation = Mathf.Lerp(elevationMin, elevationMax, Random.Range(0f, 1f)*slope /* (size*size*0.25f)*/);
                map[i, j].transform.position = new Vector3(i*MapTileMetrics.outerWidth, elevation, j*MapTileMetrics.outerWidth);

                // color
                float lerpVal = ((elevation - elevationMin) / elevationMax);
                float lerpVal2 = Mathf.Min((float)i, (float)j, (float)size - i - 1f, (float)size - j - 1f)/(size/2f);

                Color c1 = new Color(1f,1f,1f);
                Color c2 = new Color(0f, 160f / 255f,0f);

                map[i, j].color = Color.Lerp (c1,c2, lerpVal*0.5f + lerpVal2*0.5f);

                // set neighbors
                if (i > 0)
                {
                    map[i, j].SetNeighbor(MapTileMetrics.Direction.W, map[i - 1, j]);
                }

                if (j > 0)
                {
                    map[i, j].SetNeighbor(MapTileMetrics.Direction.S, map[i, j - 1]);
                }
            

                // type: 80% forest for now\
                if (elevation <= MapTileMetrics.oceanElevation)
                {
                    map[i, j].SetTileType(MapTile.TileType.water);
                }
                else if (Random.Range(0f, 1f) < 0.8)
                {
                    map[i, j].SetTileType(MapTile.TileType.forest);
                }
                else
                {
                    map[i, j].SetTileType(MapTile.TileType.town);
                }

                // health
                map[i, j].health = 40f;

                // test: start with about 10% forest tiles on fire
                if (map[i,j].tileType == MapTile.TileType.forest && Random.Range(0f,1f) < 0.3)
                {
                    map[i, j].SetOnFire();
                }
            }
        }

        return this;
    }

    public MapTile GetTileBelow(Vector3 position)
    {
        //map[i, j].transform.position = new Vector3(i*MapTileMetrics.outerWidth, elevation, j*MapTileMetrics.outerWidth);
        float x = position.x;
        float z = position.z;

        int i = Mathf.FloorToInt(x / MapTileMetrics.outerWidth);
        int j = Mathf.FloorToInt(z / MapTileMetrics.outerWidth);

        if (i < map.GetLength(0) && j < map.GetLength(1) && i >= 0 && j >= 0)
        {
            return map[i, j];
        }
        else
        {
            return null;
        }
    }

    public int GetTilesInState(MapTile.TileState s)
    {
        int count = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].tileState == s)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public int GetTilesInState(MapTile.TileState s, MapTile.TileType t)
    {
        int count = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i,j].tileType == t && map[i,j].tileState == s)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public int GetTilesOfType(MapTile.TileType t)
    {
        int count = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].tileType == t)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void OnDrawGizmos()
    {
        if (map != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i,j] != null)
                        Gizmos.DrawSphere(map[i, j].transform.position, 0.8f);
                }
            }
        }
    }


}
