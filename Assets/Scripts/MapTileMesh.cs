using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapTileMesh : MonoBehaviour {

    Mesh mapTileMesh;
    List<Vector3> vertices;
    List<int> triangles;
    MeshCollider meshCollider;
    List<Color> colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mapTileMesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        colors = new List<Color>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        // change this later to raycast down from the plane and do stuff ? 
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.LogFormat("Touch: {0}", hit.point);
            Debug.LogFormat("Map Tile: i={0}, j={1}", Mathf.Floor(hit.point.x / 10f), Mathf.Floor(hit.point.z / 10f));
        }
    }

    public void Triangulate(MapTile[,] map)
    {
        mapTileMesh.Clear();
        vertices.Clear();
        colors.Clear();
        triangles.Clear();

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                Triangulate(map[i, j]);
            }
        }

        mapTileMesh.vertices = vertices.ToArray();
        mapTileMesh.colors = colors.ToArray();
        mapTileMesh.triangles = triangles.ToArray();
        mapTileMesh.RecalculateNormals();

        meshCollider.sharedMesh = mapTileMesh;
    }

    public void Triangulate(MapTile tile)
    {

        float edgeOffset = (MapTileMetrics.outerWidth - MapTileMetrics.innerWidth) / 2;

        // Inner Square: Solid Color
        Vector3 tileOrigin = tile.transform.localPosition;
        AddTriangle(
            tileOrigin + new Vector3(edgeOffset, 0f, edgeOffset),
            tileOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset),
            tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset)
            );
        AddTriangleColor(tile.color, tile.color, tile.color);

        AddTriangle(
            tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset),
            tileOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
            tileOrigin + MapTileMetrics.corners[3] + new Vector3(edgeOffset, 0f, edgeOffset)
            );
        AddTriangleColor(tile.color, tile.color, tile.color);

        // Geometry between adjacent tiles
        MapTile neighbor;

        // Add North Quad
        if ((neighbor = tile.GetNeighbor(MapTileMetrics.Direction.N)) != null)
        {
            Vector3 neighborOrigin = neighbor.transform.localPosition;

            AddTriangle(
                tileOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset),
                neighborOrigin + new Vector3(edgeOffset, 0f, edgeOffset),
                neighborOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset)
                );
            AddTriangleColor(tile.color, neighbor.color, neighbor.color);

            AddTriangle(
                tileOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset),
                neighborOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
                tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset)
                );
            AddTriangleColor(tile.color, neighbor.color, tile.color);
        }

        // Add East Quad
        if ((neighbor = tile.GetNeighbor(MapTileMetrics.Direction.E)) != null)
        {
            Vector3 neighborOrigin = neighbor.transform.localPosition;

            AddTriangle(
                tileOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
                tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset),
                neighborOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset)
                );
            AddTriangleColor(tile.color, tile.color, neighbor.color);

            AddTriangle(
                tileOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
                neighborOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset),
                neighborOrigin + new Vector3(edgeOffset, 0f, edgeOffset)
                );
            AddTriangleColor(tile.color, neighbor.color, neighbor.color);
        }

        // Connect Corners
        MapTile north = tile.GetNeighbor(MapTileMetrics.Direction.N);
        MapTile east = tile.GetNeighbor(MapTileMetrics.Direction.E);
        MapTile northEast = (north != null ? north.GetNeighbor(MapTileMetrics.Direction.E) : (east != null ? east.GetNeighbor(MapTileMetrics.Direction.N) : null));

        if (north != null && east != null && northEast != null)
        {
            Vector3 northOrigin = north.transform.localPosition;
            Vector3 northEastOrigin = northEast.transform.localPosition;
            Vector3 eastOrigin = east.transform.localPosition;

            // For center point
            float elevationAvg = (tileOrigin.y + northOrigin.y + northEastOrigin.y + eastOrigin.y) / 4;
            Color colorAvg = 0.25f * (tile.color + north.color + northEast.color + east.color);

            Vector3 centerPoint = tileOrigin + MapTileMetrics.corners[1];
            centerPoint.y = elevationAvg;

            AddTriangle(
                tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset),
                northOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
                centerPoint
                );
            AddTriangleColor(tile.color, north.color, colorAvg);

            AddTriangle(
                northOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
                northEastOrigin + new Vector3(edgeOffset, 0f, edgeOffset),
                centerPoint
                );
            AddTriangleColor(north.color, northEast.color, colorAvg);

            AddTriangle(
                northEastOrigin + new Vector3(edgeOffset, 0f, edgeOffset),
                eastOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset),
                centerPoint
                );
            AddTriangleColor(northEast.color, east.color, colorAvg);

            AddTriangle(
                eastOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset),
                tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset),
                centerPoint
                );
            AddTriangleColor(east.color, tile.color, colorAvg);

        }
        else if (north != null && northEast != null)
        {
            Vector3 northOrigin = north.transform.localPosition;
            Vector3 northEastOrigin = northEast.transform.localPosition;

            AddTriangle(
                tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset),
                northOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
                northEastOrigin + new Vector3(edgeOffset, 0f, edgeOffset)
                );
            AddTriangleColor(tile.color, north.color, northEast.color);
        }
        else if (northEast != null && east != null)
        {
            Vector3 northEastOrigin = northEast.transform.localPosition;
            Vector3 eastOrigin = east.transform.localPosition;

            AddTriangle(
                northEastOrigin + new Vector3(edgeOffset, 0f, edgeOffset),
                eastOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset),
                tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset)
                );
            AddTriangleColor(northEast.color, east.color, tile.color);
        }
        else if (north != null && east != null)
        {
            Vector3 northOrigin = north.transform.localPosition;
            Vector3 eastOrigin = east.transform.localPosition;

            AddTriangle(
                tileOrigin + MapTileMetrics.corners[1] + new Vector3(-edgeOffset, 0f, -edgeOffset),
                northOrigin + MapTileMetrics.corners[2] + new Vector3(-edgeOffset, 0f, edgeOffset),
                eastOrigin + MapTileMetrics.corners[0] + new Vector3(edgeOffset, 0f, -edgeOffset)
                );
            AddTriangleColor(tile.color, north.color, east.color);
        }


    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }
}
