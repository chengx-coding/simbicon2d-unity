using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour {

    // rad angle
    public float torso; //global
    public float swingHip; //global
    public float swingKnee; //local
    public float swingAnkle; //local
    public float stanceHip; //global
    public float stanceKnee; //local
    public float stanceAnkle; //local

    public float[] joint = new float[7];

    public float duration;

    public float cd;
    public float cv;

    public bool isRightStance;

    // Use this for initialization
    void Start () {
        Init();
    }

    public void Init()
    {
        if (isRightStance)
        {
            //rHip = 0;
            //rKnee = stanceKnee;
            //rAnkle = stanceAnkle;
            //lHip = swingHip;
            //lKnee = swingKnee;
            //lAnkle = swingAnkle;

            joint[0] = torso;
            joint[1] = stanceHip; // right hip
            joint[2] = swingHip; // left hip
            joint[3] = stanceKnee; // right knee
            joint[4] = swingKnee; //left knee
            joint[5] = stanceAnkle; //right ankle
            joint[6] = swingAnkle; //left ankle
        }
        else
        {
            //lHip = 0;
            //lKnee = stanceKnee;
            //lAnkle = stanceAnkle;
            //rHip = swingHip;
            //rKnee = swingKnee;
            //rAnkle = swingAnkle;

            joint[0] = torso;
            joint[1] = swingHip; // right hip
            joint[2] = stanceHip; // left hip
            joint[3] = swingKnee; // right knee
            joint[4] = stanceKnee; //left knee
            joint[5] = swingAnkle; //right ankle
            joint[6] = stanceAnkle; //left ankle
        }

    }

}
