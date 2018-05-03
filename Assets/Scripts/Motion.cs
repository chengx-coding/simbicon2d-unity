using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour {

    public int stateNum;
    public State[] state;
    public float[] minAngle = new float[7];
    public float[] maxAngle = new float[7];
    public float targetVelocity;
    // Use this for initialization
    //void Start()
    //{
    //    Init();
    //}
    //public void Init()
    void Awake()
    {
        Debug.Log("Set Motion: " + gameObject.name);
        stateNum = transform.childCount;
        state = new State[stateNum];
        for (int i = 0; i < stateNum; i++)
        {
            state[i] = transform.GetChild(i).gameObject.GetComponent<State>();
        }

        //if(gameObject.name.Equals("Walk"))
        //{
        //    Debug.Log("it is walk");
        //}

    }



    // Update is called once per frame
    void Update () {
		
	}
}
