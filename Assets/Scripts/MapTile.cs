using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

    public enum TileType
    {
        forest,
        town,
        water
    };

    public enum TileState
    {
        healthy,
        burning,
        dead,
        safe
    };

    public TileType tileType;
    public TileState tileState;

    public GameObject[] treePrefabs;
    public GameObject deadTreePrefab;

    public GameObject[] housePrefabs;
    public GameObject burntHousePrefab;

    public Color color;

    public float health = 20f; // how long to burn before dead

    [SerializeField]
    MapTile[] neighbors;

    public GameObject smokeParticlesPrefab;
    public GameObject fireParticlesPrefab;

    ParticleSystem smokeParticles;
    ParticleSystem fireParticles;

    List<GameObject> decorations;

    private void Awake()
    {
        GameObject smoke = Instantiate(smokeParticlesPrefab, transform);
        GameObject fire = Instantiate(fireParticlesPrefab, transform);

        smoke.transform.position += new Vector3(MapTileMetrics.outerWidth / 2, 0f, MapTileMetrics.outerWidth / 2);
        fire.transform.position += new Vector3(MapTileMetrics.outerWidth / 2, 0f, MapTileMetrics.outerWidth / 2);

        smokeParticles = smoke.GetComponent<ParticleSystem>();
        fireParticles = fire.GetComponent<ParticleSystem>();

        decorations = new List<GameObject>();
    }

    public void SetTileType(TileType t)
    {
        tileType = t;

        if (t == TileType.forest || t == TileType.town)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (t == TileType.forest)
                    {
                        // choose tree: 0 - 4
                        int val = Mathf.RoundToInt(Mathf.Lerp(0f, 4f, Random.Range(-0.1f, 0.1f) + (transform.position.y - MapTileMetrics.elevationMin) / MapTileMetrics.elevationMax));

                        GameObject decoration = Instantiate(treePrefabs[val], transform);
                        decoration.transform.RotateAround(decoration.transform.position, decoration.transform.right, 90f);
                        decoration.transform.position += new Vector3(
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2,
                                                           0f,
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2)
                                                        + new Vector3(
                                                            i * MapTileMetrics.innerWidth / 2,
                                                            0f,
                                                            j * MapTileMetrics.innerWidth / 2);

                        decorations.Add(decoration);
                    }

                    else if (t == TileType.town)
                    {
                        // choose house: 0 - 2
                        int val = Mathf.RoundToInt(Mathf.Lerp(0f, 2f, Random.Range(0f, 1f)));

                        GameObject decoration = Instantiate(housePrefabs[val], transform);
                        decoration.transform.RotateAround(decoration.transform.position, decoration.transform.right, 90f);
                        decoration.transform.position += new Vector3(
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2,
                                                           0f,
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2)
                                                        + new Vector3(
                                                            i * MapTileMetrics.innerWidth / 2,
                                                            0f,
                                                            j * MapTileMetrics.innerWidth / 2);

                        decorations.Add(decoration);
                    }
                }
            }
        }
    }

    public MapTile GetNeighbor(MapTileMetrics.Direction direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(MapTileMetrics.Direction direction, MapTile tile)
    {
        neighbors[(int)direction] = tile;
        tile.neighbors[(int)direction.Opposite()] = this;
    }

    public void SetOnFire()
    {
        tileState = TileState.burning;
        smokeParticles.Play();
        fireParticles.Play();
        InvokeRepeating("BurnTile", 0f, 2f);
    }

    void BurnTile()
    {
        health -= 2f;

        Debug.LogFormat("Burning Tile! x={0}, z={1}, Health={2}", transform.localPosition.x, transform.localPosition.z, health);

        if (health <= 0)
        {
            health = 0;
            KillTile();
            CancelInvoke("BurnTile");
        }

        // small chance of setting a neighbor on fire
        if (Random.Range(0f,1f) < 0.1)
        {
            // choose a neighbor
            MapTile n = neighbors[Mathf.FloorToInt(Random.Range(0f, 3.99f))];
            if (n.tileState == TileState.healthy && n.tileType != TileType.water)
            {
                n.SetOnFire();
            }
        }
    }

    public void KillTile()
    {
        tileState = TileState.dead;
        smokeParticles.Stop();
        fireParticles.Stop();

        foreach (GameObject decoration in decorations.ToArray())
        {
            decorations.Remove(decoration);
            Destroy(decoration);
        }
        
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tileType == TileType.forest)
                {
                    GameObject decoration = Instantiate(deadTreePrefab, transform);
                    decoration.transform.RotateAround(decoration.transform.position, decoration.transform.right, 90f);
                    decoration.transform.position += new Vector3(
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2,
                                                           0f,
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2)
                                                        + new Vector3(
                                                            i * MapTileMetrics.innerWidth / 2,
                                                            0f,
                                                            j * MapTileMetrics.innerWidth / 2);


                    decorations.Add(decoration);
                }
                
                else if (tileType == TileType.town)
                {
                    GameObject decoration = Instantiate(burntHousePrefab, transform);

                    decoration.GetComponent<ParticleSystem>().Play();

                    decoration.transform.RotateAround(decoration.transform.position, decoration.transform.right, 90f);
                    decoration.transform.position += new Vector3(
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2,
                                                           0f,
                                                           (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2)
                                                        + new Vector3(
                                                            i * MapTileMetrics.innerWidth / 2,
                                                            0f,
                                                            j * MapTileMetrics.innerWidth / 2);


                    decorations.Add(decoration);
                }
            }
        }
    }

    public void AddWater()
    {
        if (tileState != TileState.dead && tileState != TileState.healthy)
        {
            health += 1;
            if (health >= 20) 
            {
                health = 40f;
                ExtinguishFire();
            }
        }
    }

    public void ExtinguishFire()
    {
        CancelInvoke("BurnTile");
        smokeParticles.Stop();
        fireParticles.Stop();
        tileState = TileState.healthy;
    }

}
