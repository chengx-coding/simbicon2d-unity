using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    float tarY;
    private Vector3 initPos;

    private void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update () {
        float tarY = target.transform.position.y;
        if (tarY >= 5 || tarY <= -2)
        {
            tarY = target.transform.position.y;
        }
        else
        {
            tarY = transform.position.y;
        }
        //tarY = transform.position.y;
        Vector3 tarPos = new Vector3(target.transform.position.x, tarY, transform.position.z);
        //transform.position = tarPos;
        transform.position = Vector3.SmoothDamp(transform.position, tarPos, ref velocity, smoothTime);

        if (Input.GetKeyDown("r"))
        {
            transform.position = initPos;
        }

    }
}
