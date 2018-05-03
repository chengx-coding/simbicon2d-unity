using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

    public bool isCollide;
    public GameObject contactObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        isCollide = true;
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        isCollide = true;
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        isCollide = false;
    }

}
