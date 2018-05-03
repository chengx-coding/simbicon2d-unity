using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastRun : MonoBehaviour
{
    Motion motion;

    float[] maxAngle;
    // Use this for initialization
    void Start()
    {
        motion = gameObject.GetComponent<Motion>();
        //motion.Init();
        State[] state = motion.state;
        Debug.Log(name + " states: " + state.Length);

        for (int i = 0; i < 2; i++)
        {
            state[i].torso = -0.2f;
            state[i].swingHip = 1.08f;
            state[i].swingKnee = -2.18f;
            state[i].swingAnkle = 0.2f;
            state[i].stanceKnee = -0.05f;
            state[i].stanceAnkle = 0.27f;
            state[i].duration = 0.15f;
            state[i].cd = 0;
            state[i].cv = 0.2f;

            if (i == 0)
            {
                state[i].isRightStance = true;
            }
            else
            {
                state[i].isRightStance = false;
            }
            state[i].Init();
        }

        motion.targetVelocity = 6f;

        motion.minAngle[0] = -Mathf.PI * 0.5f; //torso
        motion.maxAngle[0] = Mathf.PI * 0.5f;
        motion.minAngle[1] = -Mathf.PI * 0.5f; //right hip
        motion.maxAngle[1] = Mathf.PI * 0.5f;
        motion.minAngle[2] = -Mathf.PI * 0.5f; //left hip
        motion.maxAngle[2] = Mathf.PI * 0.5f;
        motion.minAngle[3] = -Mathf.PI + 0.05f; //right knee
        motion.maxAngle[3] = -0.00f;
        motion.minAngle[4] = -Mathf.PI + 0.05f; //left knee
        motion.maxAngle[4] = -0.00f;
        motion.minAngle[5] = -Mathf.PI * 0.5f + 0.05f; //right ankle
        motion.maxAngle[5] = Mathf.PI * 0.5f;
        motion.minAngle[6] = -Mathf.PI * 0.5f + 0.05f; //left ankle
        motion.maxAngle[6] = Mathf.PI * 0.5f;

    }

}
