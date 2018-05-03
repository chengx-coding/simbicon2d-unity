using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour {

    public Vector2 direction;
    public Vector2 pivot;
	// Use this for initialization
	void Start () {
        direction.Normalize();
        HingeJoint2D hinge = gameObject.GetComponent<HingeJoint2D>();
        if (hinge)
        {
            pivot = hinge.anchor;
        }
        else
        {
            pivot = Vector2.zero;
        }
        string name = gameObject.name;
        if(name.Equals("Torso"))
        {
            pivot = Vector2.zero;
        }
	}
	
}
