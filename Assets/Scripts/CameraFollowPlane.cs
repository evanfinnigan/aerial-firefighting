using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlane : MonoBehaviour {

    public Transform target;
    public Vector3 offset;

    public float lookAhead;

    public float moveTime;
    float time;

    float bias = 0.96f;

    Vector3 regularOffset;
    float regularLookAhead;

    private void Awake()
    {
        regularOffset = offset;
        regularLookAhead = lookAhead;
    }

    // Update is called once per frame
    void Update () {
        transform.position = bias*transform.position + (1f-bias)*(target.position + target.forward.normalized * offset.z + Vector3.up * offset.y);
        transform.LookAt(target.position + target.forward.normalized * lookAhead);

        // move camera if dropping water...
        if (Input.GetKey(KeyCode.DownArrow))
        {
            time += Time.deltaTime;
            if (time > moveTime) time = moveTime;
        }
        else
        {
            time -= Time.deltaTime;
            if (time < 0f) time = 0f;
        }

        offset = Vector3.Lerp(regularOffset, new Vector3(0f, 80f, -20f), time / moveTime);
        lookAhead = Mathf.Lerp(regularLookAhead, -10, time / moveTime);
    }
}
