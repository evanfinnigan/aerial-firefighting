using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    MapTileGrid map;
    private void Awake()
    {
        map = FindObjectOfType<MapTileGrid>();
    }

    private void Update()
    {
        if (transform.position.y < 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("water"))
        {
            MapTile m = map.GetTileBelow(transform.position);
            if (m != null)
            {
                m.AddWater();
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
