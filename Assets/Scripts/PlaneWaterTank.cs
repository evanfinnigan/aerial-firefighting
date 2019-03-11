using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneWaterTank : MonoBehaviour {

    public MapTileGrid map;
    public GameObject waterPrefab;

    // litres of water the plane can hold
    public float waterCapacity = 100;
    public float fillRate = 30;
    public float emptyRate = 10;

    public Slider waterSlider;

    float waterInTank;

    bool filling = false;

    private void FixedUpdate()
    {
        waterSlider.value = waterInTank;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            FillTank();
        }
        else if (transform.position.y < 40)
        {
            transform.Translate(new Vector3(0f,1f,0f));
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            DropWater();
        }
    }

    void FillTank()
    {
        MapTile tileBelow = map.GetTileBelow(transform.position);

        if (tileBelow == null || tileBelow.tileType == MapTile.TileType.water)
        {
            if (transform.position.y > 10f)
            {
                transform.Translate(new Vector3(0f, -1f, 0f));
            }
            waterInTank += fillRate * Time.deltaTime;
            if (waterInTank > waterCapacity) waterInTank = waterCapacity;
        }
        else if (transform.position.y < 40f)
        {
            transform.Translate(new Vector3(0f, 1f, 0f));
        }
    }

    void DropWater()
    {
        Debug.Log("Dropping Water!");

        if (waterInTank > 0f)
        {
            Destroy(Instantiate(waterPrefab, transform.position - new Vector3(0f, 1f, 0f), transform.rotation), 10f);
        }

        waterInTank -= emptyRate * Time.deltaTime;
        if (waterInTank < 0f) waterInTank = 0f;
    }
}
