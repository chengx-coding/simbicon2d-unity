using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootContact : MonoBehaviour {

    public bool isContact;
    public bool isFootContact;
    public Vector2[] contactPoints;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.name == "Ground")
        {
            isContact = true;
        }
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Ground")
        {
            isContact = true;
            contactPoints = new Vector2[coll.contacts.Length];
            for(int i = 0; i< coll.contacts.Length;i++)
            {
                contactPoints[i] = coll.contacts[i].point;
            }
            if(coll.contacts.Length == 2)
            {
                isFootContact = true;
            }
            else
            {
                isFootContact = false;
            }
            if(transform.eulerAngles.z < 15f || transform.eulerAngles.z > 345f)
            {
                isFootContact = true;
            }
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Ground")
        {
            isContact = false;
            isFootContact = false;
            contactPoints = null;
        }
    }
}
