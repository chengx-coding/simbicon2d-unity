using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stand : MonoBehaviour {

    Motion motion;

    float[] maxAngle;
    // Use this for initialization
    void Start()
    {
        motion = gameObject.GetComponent<Motion>();
        //motion.Init();
        State[] state = motion.state;
        Debug.Log(state.Length);

        int i = 0;

        state[i].isRightStance = true;
        state[i].torso = 0;
        state[i].duration = 0;
        state[i].cd = 0;
        state[i].cv = 0f;

        state[i].swingHip = 0.2f;
        state[i].swingKnee = -0.3f;
        state[i].swingAnkle = 0f;

        state[i].stanceHip = -0.2f;
        state[i].stanceKnee = -0.2f;
        state[i].stanceAnkle = 0.3f; //0.2f

        // 0.1 rad = 5.73 degree
        // 0.2 rad = 11.46 degree

        motion.minAngle[0] = Mathf.Deg2Rad * -90f; //torso
        motion.maxAngle[0] = Mathf.Deg2Rad * 90f;
        motion.minAngle[1] = Mathf.Deg2Rad * -90f; //right hip
        motion.maxAngle[1] = Mathf.Deg2Rad * 90f;
        motion.minAngle[2] = Mathf.Deg2Rad * -90f; //left hip
        motion.maxAngle[2] = Mathf.Deg2Rad * 90f;
        motion.minAngle[3] = Mathf.Deg2Rad * -165f; //right knee
        motion.maxAngle[3] = Mathf.Deg2Rad * -5f;
        motion.minAngle[4] = Mathf.Deg2Rad * -165f; //left knee
        motion.maxAngle[4] = Mathf.Deg2Rad * -5f;
        motion.minAngle[5] = Mathf.Deg2Rad * -85f; //right ankle
        motion.maxAngle[5] = Mathf.Deg2Rad * 85f;
        motion.minAngle[6] = Mathf.Deg2Rad * -85f; //left ankle
        motion.maxAngle[6] = Mathf.Deg2Rad * 85f;
    }
}
