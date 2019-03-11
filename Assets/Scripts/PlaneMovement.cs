using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMovement : MonoBehaviour {

    public float m_Speed;

    float minSpeed = 10f;
    float maxSpeed = 70f;
    float acceleration = 15f;

    float max_time = 1f;
    float timer;

    public Transform rollCage;

	void Awake () {
        timer = max_time;
	}
	
	void Update ()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        MovePlane();
        RotatePlane(v, h, -h);

        if (Mathf.Abs(h) < 0.8f || Quaternion.Angle(rollCage.rotation, transform.rotation) > 45f)
        {
            Quaternion rotation = Quaternion.LookRotation(rollCage.forward, Vector3.up);
            rollCage.rotation = Quaternion.RotateTowards(rollCage.rotation, rotation, 1f);
        }
    }

    void MovePlane()
    {
        // accelerate based on orientation
        //m_Speed = Mathf.Clamp(m_Speed - transform.forward.normalized.y * acceleration * Time.deltaTime, minSpeed, maxSpeed);

        transform.position += transform.forward.normalized * m_Speed * Time.deltaTime;

        /*float minHeight = Terrain.activeTerrain.SampleHeight(transform.position);
        if (transform.position.y < minHeight)
        {
            transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
        }*/
    }

    void RotatePlane(float pitch, float yaw, float roll)
    {
        //transform.Rotate(pitch,yaw,0f);
        rollCage.RotateAround(rollCage.position, rollCage.forward, roll);

        transform.Rotate(0f, yaw, 0f);
    }
}
