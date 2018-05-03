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



        motion.minAngle[0] = -Mathf.PI * 0.5f; //torso
        motion.maxAngle[0] = Mathf.PI * 0.5f;
        motion.minAngle[1] = -Mathf.PI * 0.5f; //right hip
        motion.maxAngle[1] = Mathf.PI * 0.5f;
        motion.minAngle[2] = -Mathf.PI * 0.5f; //left hip
        motion.maxAngle[2] = Mathf.PI * 0.5f;
        motion.minAngle[3] = -Mathf.PI + 0.05f; //right knee
        motion.maxAngle[3] = 0f;
        motion.minAngle[4] = -Mathf.PI + 0.05f; //left knee
        motion.maxAngle[4] = 0f;
        motion.minAngle[5] = -Mathf.PI * 0.5f + 0.05f; //right ankle
        motion.maxAngle[5] = Mathf.PI * 0.5f;
        motion.minAngle[6] = -Mathf.PI * 0.5f + 0.05f; //left ankle
        motion.maxAngle[6] = Mathf.PI * 0.5f;

    }
}
