using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;


public class SimBiCon : MonoBehaviour
{

    bool flag = false;
    public int torqueMode;
    public bool pause;
    public bool start;
    public bool stand;
    public bool smallContact;
    public bool virtualForceOn;
    public float vituralForce;
    public float smallContactDuration;

    public float kp;
    public float kd;
    public float kVirtualForce;
    //public float kpFoot = 30f;
    //public float kdFoot = 3f;
    public float torqueLimit;
    public float ankleLimit;

    public float forceArm = 0.1f;
    public int currentState = 0;
    public float currentElapsedTime;
    public int numOfStates;

    public int currentMotionIndex;
    public Motion currentMotion;

    // the hip joint (torso joint) can ne used to as a simple and effective proxy for
    // estimating the postion and velocity of the Center Of Mas (COM)
    public float comPos; // distance from stance ankle to torso
    public float comLastPos;
    public float comSpeed;

    // 0: torso
    // 1: right upper leg - right hip
    // 2: left uppper leg - left hip
    // 3: right lower leg - right knee
    // 4: left lower leg - left knee
    // 5: right foot - right ankle
    // 6: left foot - left ankle
    public GameObject[] body = new GameObject[7];
    public float[] angles = new float[7];
    public float desiredAngle;
    public float[] lastAngles = new float[7];
    public float[] angularVelocities = new float[7];
    public float[] targetAngles = new float[7];
    public float[] torques = new float[7];
    public float[] computedTorques = new float[7];

    public GameObject[] motionObj = new GameObject[7];

    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    public GameObject stancePoint;
    public GameObject arrow;
    public GameObject[] ground = new GameObject[2];
    public int currentGround;

    public Text motionText;

    public float externalForce;

    public float t0;
    public float t1;
    public float t2;
    public float t3;

    public GameObject[] bodyPrefabs = new GameObject[7];
    public GameObject cameraFollow;

    State[] state;

    static int torsoIndex = 0;
    static int rUpperLegIndex = 1;
    static int lUpperLegIndex = 2;
    static int rLowerLegIndex = 3;
    static int lLowerLegIndex = 4;
    static int rFootIndex = 5;
    static int lFootIndex = 6;

    public int swingHipIndex;
    public int stanceHipIndex;
    public int swingKneeIndex;
    public int stanceKneeIndex;
    public int swingAnkleIndex;
    public int stanceAnkleIndex;

    // Use this for initialization
    void Start()
    {
        //Time.timeScale = 10;
        //motionObj = new GameObject[2];
        currentMotionIndex = stand ? 0 : currentMotionIndex;
        currentState = 0;
        currentElapsedTime = 0;
        forceArm = 0.1f;
        torqueMode = 0;

        kp = 1200f;
        kd = 120f;
        kVirtualForce = 300;
        torqueLimit = 1000f * 2f;
        ankleLimit = torqueLimit;
        //ankleLimit = 900f;

        //kpFoot = 10f;
        //kdFoot = 1f;

        externalForce = 150f;
        t0 = 330f;
        t1 = 250f;
        t2 = 200f;
        t3 = 200f;

        flag = false;

        currentMotion = motionObj[0].GetComponent<Motion>();
        state = currentMotion.state;
        numOfStates = state.Length;

        //if (flag) RotateJoints(state[0]);
        if (stand) RotateJoints(state[0]);

        SetSwingStanceIndex(state[currentState].isRightStance);
        comPos = body[torsoIndex].transform.position.x - body[stanceAnkleIndex].transform.position.x;

        motionText.text = "Current Motion: " + currentMotion.name;

        stancePoint = Instantiate(stancePoint, transform.position, Quaternion.identity);
        arrow = Instantiate(arrow, transform.position, Quaternion.identity);
        arrow.SetActive(false);
        ground[0] = Instantiate(ground[0], transform.position, Quaternion.identity);
        ground[1] = Instantiate(ground[1], transform.position, Quaternion.identity);
        ground[currentGround].SetActive(true);
        ground[1 - currentGround].SetActive(false);

        //SetBody();
        //cameraFollow.GetComponent<CameraFollow>().target = body[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
        }
        SwitchMotion();
        SwitchGround();
        ResetCharacter();
    }

    void FixedUpdate()
    {
        if (pause)
        {
            Time.timeScale = 0;
        }

        KeyboardControl();

        if (!start) return;

        SetMotion(); //use "m" to swith motion

        SetSwingStanceIndex(state[currentState].isRightStance);
        MarkStanceLeg();

        SetCOM();
        SetAngles();
        SetAngularVelocities();

        ComputeTargetAngles();

        ComputeTorques();

        for (int i = 0; i < 7; i++)
        {
            computedTorques[i] = torques[i];
        }

        LimitTorques();

        ApplyTorques();

        currentState = CheckState();

        if (!stand) ApplyVirtualForce();

        ApplyExternalForce();


        //Vector2 from = new Vector2(0, 0);
        //Vector2 to = new Vector2(-2, -2);
        //DrawArrow(from, to, Color.red);

        //JointLog(rFootIndex);

        if (Input.GetKey("t"))
        {
            currentState = (currentState + 1) % state.Length;
        }


    }

    void RotateJoints(State s)
    {
        for (int i = 0; i < 7; i++)
        {
            if (i <= 2)
                body[i].transform.rotation = Quaternion.Euler(Vector3.forward * s.joint[i] * Mathf.Rad2Deg);
            else
                body[i].transform.Rotate(Vector3.forward * s.joint[i] * Mathf.Rad2Deg);
        }
    }

    void KeyboardControl()
    {
        if (Input.GetKey("1"))
        {
            ApplyTorque(body[torsoIndex], t0);
        }
        if (Input.GetKey("2"))
        {
            ApplyTorque(body[torsoIndex], -t0);
        }
        if (Input.GetKey("3"))
        {
            body[torsoIndex].GetComponent<Rigidbody2D>().AddForce(new Vector2(0, t0));
        }
        if (Input.GetKey("q"))
        {
            ApplyTorque(body[lUpperLegIndex], t1);
        }
        if (Input.GetKey("w"))
        {
            ApplyTorque(body[lUpperLegIndex], -t1);
        }
        if (Input.GetKey("a"))
        {
            ApplyTorque(body[lLowerLegIndex], t2);
            //ApplyInteractionTorque(body[lLowerLegIndex], body[lUpperLegIndex], t2);
        }
        if (Input.GetKey("s"))
        {
            ApplyTorque(body[lLowerLegIndex], -t2);
            //ApplyInteractionTorque(body[lLowerLegIndex], body[lUpperLegIndex], -t2);
        }
        if (Input.GetKey("z"))
        {
            ApplyTorque(body[lFootIndex], t3);
            //ApplyInteractionTorque(body[lFootIndex], body[lLowerLegIndex], t3);
        }
        if (Input.GetKey("x"))
        {
            ApplyTorque(body[lFootIndex], -t3);
            //ApplyInteractionTorque(body[lFootIndex], body[lLowerLegIndex], -t3);
        }

        if (Input.GetKey("["))
        {
            ApplyTorque(body[rUpperLegIndex], t1);
        }
        if (Input.GetKey("]"))
        {
            ApplyTorque(body[rUpperLegIndex], -t1);
        }
        if (Input.GetKey(";"))
        {
            ApplyTorque(body[rLowerLegIndex], t2);
            //ApplyInteractionTorque(body[rLowerLegIndex], body[rUpperLegIndex], t2);
        }
        if (Input.GetKey("'"))
        {
            ApplyTorque(body[rLowerLegIndex], -t2);
            //ApplyInteractionTorque(body[rLowerLegIndex], body[rUpperLegIndex], -t2);
        }
        if (Input.GetKey("."))
        {
            ApplyTorque(body[rFootIndex], t3);
            //ApplyInteractionTorque(body[rFootIndex], body[rLowerLegIndex], t3);
        }
        if (Input.GetKey("/"))
        {
            ApplyTorque(body[rFootIndex], -t3);
            //ApplyInteractionTorque(body[rFootIndex], body[rLowerLegIndex], -t3);
        }
    }

    void SetBody()
    {
        for (int i = 0; i < 7; i++)
        {
            body[i] = Instantiate(bodyPrefabs[i], transform.position, Quaternion.identity);
        }
    }

    void SwitchMotion()
    {
        int num = motionObj.Length - 2;
        if (Input.GetKeyDown("m"))
        {
            currentMotionIndex = (currentMotionIndex + 1) % num;
            currentState = 0;
        }
        motionText.text = "Current Motion: " + currentMotion.name;

    }
    void SetMotion()
    {
        if (currentMotionIndex == 0) stand = true;
        else stand = false;

        currentMotion = motionObj[currentMotionIndex].GetComponent<Motion>();
        state = currentMotion.state;
    }

    void SwitchGround()
    {
        if(Input.GetKeyDown("v"))
        {
            ground[currentGround].SetActive(false);
            currentGround = 1 - currentGround;
            ground[currentGround].SetActive(true);
            body[0].transform.position = Vector3.zero;
        }

    }

    void ResetCharacter()
    {
        if(Input.GetKeyDown("r"))
        {
            Time.timeScale = 0;
            body[0].transform.position = Vector3.zero;
            body[1].transform.localPosition = new Vector3(0, 0, -1);
            body[2].transform.localPosition = new Vector3(0, 0, 1);
            body[3].transform.localPosition = new Vector3(0, -0.9f, 0);
            body[4].transform.localPosition = new Vector3(0, -0.9f, 0);
            body[5].transform.localPosition = new Vector3(-0.06f, -0.9f, 0);
            body[6].transform.localPosition = new Vector3(-0.06f, -0.9f, 0);
            for (int i = 0; i < 7; i++)
            {
                body[i].transform.rotation = Quaternion.identity;
            }
            //for (int i = 0; i < 7; i++)
            //{
            //    Destroy(body[i]);
            //}
            //SetBody();
            //cameraFollow.GetComponent<CameraFollow>().target = body[0];


            currentElapsedTime = 0;
            currentMotionIndex = 0;
            currentState = 0;
            Time.timeScale = 1;
        }
    }

    void SetSwingStanceIndex(bool isRightStance)
    {
        if (isRightStance)
        {
            stanceHipIndex = rUpperLegIndex;
            stanceKneeIndex = rLowerLegIndex;
            stanceAnkleIndex = rFootIndex;
            swingHipIndex = lUpperLegIndex;
            swingKneeIndex = lLowerLegIndex;
            //swingAnkleIndex = lFootIndex;
            swingAnkleIndex = lFootIndex;
        }
        else
        {
            stanceHipIndex = lUpperLegIndex;
            stanceKneeIndex = lLowerLegIndex;
            stanceAnkleIndex = lFootIndex;
            swingHipIndex = rUpperLegIndex;
            swingKneeIndex = rLowerLegIndex;
            //swingAnkleIndex = rFootIndex;
            swingAnkleIndex = rFootIndex;
        }
    }

    void SetCOM()
    {
        comLastPos = comPos;
        comPos = body[0].transform.position.x - body[stanceAnkleIndex].transform.position.x;
        //comSpeed = comPos - comLastPos;
        comSpeed = body[0].GetComponent<Rigidbody2D>().velocity.x;
    }

    void SetAngles()
    {
        for (int i = 0; i < 7; i++)
        {
            lastAngles[i] = angles[i];
        }
        for (int i = 0; i <= 2; i++)
        {
            angles[i] = Deg2Rad(body[i].transform.eulerAngles.z);
        }
        for (int i = 3; i < 7; i++)
        {
            angles[i] = Deg2Rad(body[i].transform.localEulerAngles.z);
        }
    }

    void SetAngularVelocities()
    {
        for (int i = 0; i < 7; i++)
        {
            //angularVelocities[i] = angles[i] - lastAngles[i];
            //angularVelocities[i] = angularVelocities[i] / Time.fixedDeltaTime;
            angularVelocities[i] = body[i].GetComponent<Rigidbody2D>().angularVelocity * Mathf.Deg2Rad;
        }
    }

    void ComputeTargetAngles()
    {
        // swing hip
        desiredAngle = BalanceFeedback(state[currentState].joint[swingHipIndex],
                                             state[currentState].cd, state[currentState].cv, comPos, comSpeed);

        // target angles
        for (int i = 0; i < 7; i++)
        {
            targetAngles[i] = state[currentState].joint[i];
        }
        if (!stand)
        {
            targetAngles[stanceHipIndex] = 0;
        }

        //targetAngles[swingHipIndex] = desiredAngle;
    }

    void ComputeTorques()
    {
        for (int i = 0; i < 7; i++)
        {
            torques[i] = PDControl(targetAngles[i], angles[i], angularVelocities[i]);
        }

        if (!stand)
        {
            torques[stanceHipIndex] = -torques[0] - torques[swingHipIndex];
        }

    }

    float ComputeConstrainedTorque(float mass, float angularVel, float length, float timeStep)
    {
        return (1f / 3f) / timeStep * mass * angularVel * length * length;
    }

    void ApplyTorque(GameObject obj, float torque) //in 3D, torque will be vector3
    {
        BodyPart bp = obj.GetComponent<BodyPart>();
        Vector2 defaultDirection = bp.direction;
        float angle = obj.transform.eulerAngles.z;
        //Vector2 direction = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        // direction in global
        Vector2 direction = RotateVector2(defaultDirection, angle);
        //direction.Normalize();

        // pivot in global
        Vector2 pivot = new Vector2(obj.transform.position.x, obj.transform.position.y);
        pivot += RotateVector2(bp.pivot, angle);

        Vector2 force = new Vector2(-direction.y, direction.x) * torque / forceArm;

        if (torqueMode == 0)
        {
            obj.GetComponent<Rigidbody2D>().AddForceAtPosition(force * 0.5f, pivot + direction * forceArm);
            obj.GetComponent<Rigidbody2D>().AddForceAtPosition(force * -0.5f, pivot - direction * forceArm);
        }
        else if (torqueMode == 1)
        {
            obj.GetComponent<Rigidbody2D>().AddForceAtPosition(force, pivot + direction * forceArm);
            obj.GetComponent<Rigidbody2D>().AddForceAtPosition(force * -1f, pivot);
        }
        else if (torqueMode == 2)
        {
            obj.GetComponent<Rigidbody2D>().AddForceAtPosition(force, pivot + direction * forceArm);
        }
        else
        {
            obj.GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Force);
        }


        //if (obj.name == "FootR")
        //{
        //    p1.transform.position = pivot + direction * forceArm;
        //    p2.transform.position = pivot - direction * forceArm;
        //    p1.transform.position = new Vector3(p1.transform.position.x, p1.transform.position.y, -5);
        //    p2.transform.position = new Vector3(p2.transform.position.x, p2.transform.position.y, -5);
        //}

        //if (obj.name == "FootL")
        //{
        //    p3.transform.position = pivot + direction * forceArm;
        //    p4.transform.position = pivot - direction * forceArm;
        //    p3.transform.position = new Vector3(p3.transform.position.x, p3.transform.position.y, -5);
        //    p4.transform.position = new Vector3(p4.transform.position.x, p4.transform.position.y, -5);
        //}

    }

    void ApplyInteractionTorque(GameObject obj, GameObject obj2, float torque) //in 3D, torque will be vector3
    {
        BodyPart bp = obj.GetComponent<BodyPart>();
        Vector2 defaultDirection = bp.direction;
        float angle = obj.transform.eulerAngles.z;
        //Vector2 direction = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        // direction in global
        Vector2 direction = RotateVector2(defaultDirection, angle);
        direction.Normalize();

        // pivot in global
        Vector2 pivot = new Vector2(obj.transform.position.x, obj.transform.position.y);
        pivot += RotateVector2(bp.pivot, angle);

        Vector2 force = new Vector2(-direction.y, direction.x) * torque / forceArm;

        //BodyPart bp2 = obj2.GetComponent<BodyPart>();
        //float angle2 = obj2.transform.eulerAngles.z;
        //Vector2 direction2 = RotateVector2(-bp2.direction, angle2);
        //direction2.Normalize();
        //Vector2 force2 = new Vector2(direction2.y, -direction2.x) * torque / forceArm;

        obj.GetComponent<Rigidbody2D>().AddForceAtPosition(-force, pivot + direction * forceArm);
        obj2.GetComponent<Rigidbody2D>().AddForceAtPosition(force, pivot + direction * forceArm);

        obj.GetComponent<Rigidbody2D>().AddForceAtPosition(force, pivot);
        obj2.GetComponent<Rigidbody2D>().AddForceAtPosition(-force, pivot);

    }

    void ApplyConstrainedTorque(GameObject obj1, GameObject obj2, float torque)
    {
        BodyPart bp = obj2.GetComponent<BodyPart>();
        Vector2 defaultDirection = bp.direction;
        float angle = obj2.transform.eulerAngles.z;
        //Vector2 direction = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        // direction in global
        Vector2 direction = RotateVector2(defaultDirection, angle);
        direction.Normalize();

        // pivot in global
        Vector2 pivot = new Vector2(obj2.transform.position.x, obj2.transform.position.y);
        pivot += RotateVector2(bp.pivot, angle);

        Vector2 force = new Vector2(-direction.y, direction.x) * torque / forceArm;

        obj1.GetComponent<Rigidbody2D>().AddForceAtPosition(-force, pivot - direction * forceArm);
        obj1.GetComponent<Rigidbody2D>().AddForceAtPosition(force, pivot);

        obj2.GetComponent<Rigidbody2D>().AddForceAtPosition(force, pivot + direction * forceArm);
        obj2.GetComponent<Rigidbody2D>().AddForceAtPosition(-force, pivot);


        //obj2.GetComponent<Rigidbody2D>().AddForceAtPosition(force2 * 0.5f, pivot + direction2 * forceArm);

    }

    void LimitTorques()
    {
        for (int i = 0; i < 7; i++)
        {
            if (angles[i] > currentMotion.maxAngle[i])
            {
                torques[i] = PDControl(currentMotion.maxAngle[i], angles[i], angularVelocities[i]);
                //torques[i] = -torqueLimit;
                //torques[i] = -ComputeConstrainedTorque(
                //    body[i].GetComponent<Rigidbody2D>().mass,
                //    angularVelocities[i],
                //    body[i].GetComponent<SpriteRenderer>().size.y,
                //    Time.fixedDeltaTime);
                //Debug.Log("size:" + body[i].name + body[i].GetComponent<SpriteRenderer>().size.ToString());
                //Debug.Log("time:" + Time.fixedDeltaTime);
            }
            else if (angles[i] < currentMotion.minAngle[i])
            {
                torques[i] = PDControl(currentMotion.minAngle[i], angles[i], angularVelocities[i]);
                //torques[i] = torqueLimit;
                //torques[i] = -ComputeConstrainedTorque(
                //    body[i].GetComponent<Rigidbody2D>().mass,
                //    angularVelocities[i],
                //    body[i].GetComponent<SpriteRenderer>().size.y,
                //    Time.fixedDeltaTime);

            }
            torques[i] = BoundRange(torques[i], -torqueLimit, torqueLimit);
        }

        // ankles
        for (int i = 5; i < 7; i++)
        {
            torques[i] = BoundRange(torques[i], -ankleLimit, ankleLimit);
        }
    }

    void ApplyTorques()
    {
        //Apply torques
        for (int i = 0; i < 7; i++)
        {
            ApplyTorque(body[i], torques[i]);
        }
    }

    int CheckState()
    {
        if (state[currentState].duration <= 0)
        {
            //currentElapsedTime = 0;
            if (state[currentState].isRightStance)
            {
                if (smallContact)
                {
                    if (body[lFootIndex].GetComponent<FootContact>().isContact)
                    {
                        if (AdvanceTime(smallContactDuration))
                        {
                            currentElapsedTime = 0;
                            return (currentState + 1) % state.Length;
                        }
                    }
                }
                else
                {
                    if (body[lFootIndex].GetComponent<FootContact>().isFootContact)
                    {
                        currentElapsedTime = 0;
                        return (currentState + 1) % state.Length;
                    }

                    if (AdvanceTime(smallContactDuration))
                    {
                        currentElapsedTime = 0;
                        return (currentState + 1) % state.Length;
                    }
                }

            }
            else
            {
                if (smallContact)
                {
                    if (body[rFootIndex].GetComponent<FootContact>().isContact)
                    {
                        if (AdvanceTime(smallContactDuration))
                        {
                            currentElapsedTime = 0;
                            return (currentState + 1) % state.Length;
                        }
                    }
                }
                else
                {
                    if (body[rFootIndex].GetComponent<FootContact>().isFootContact)
                    {
                        currentElapsedTime = 0;
                        return (currentState + 1) % state.Length;
                    }

                    if (AdvanceTime(smallContactDuration))
                    {
                        currentElapsedTime = 0;
                        return (currentState + 1) % state.Length;
                    }

                }

            }
            //return (currentState + 1) % state.Length;
        }
        else
        {
            if (AdvanceTime(state[currentState].duration))
            {
                currentElapsedTime = 0;
                return (currentState + 1) % state.Length;
            }
        }

        //if(!(body[stanceAnkleIndex].GetComponent<FootContact>().isContact))
        //{
        //    return (currentState + 1) % state.Length;
        //}

        return currentState;
    }

    bool AdvanceTime(float limit)
    {
        currentElapsedTime += Time.fixedDeltaTime;
        if (currentElapsedTime >= limit)
        {
            currentElapsedTime = 0;
            return true;
        }
        return false;
    }

    void ApplyVirtualForce()
    {
        if (virtualForceOn)
        {
            //torques[0] = 0;
            vituralForce = kVirtualForce * (currentMotion.targetVelocity - comSpeed);
            body[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(vituralForce, 0));
        }

    }

    void ApplyExternalForce()
    {
        if (Input.GetKey("f") || Input.GetKey("g"))
        {
            BodyPart bp = body[0].GetComponent<BodyPart>();
            Vector2 defaultDirection = bp.direction;
            float angle = body[0].transform.eulerAngles.z;
            // direction in global
            Vector2 direction = RotateVector2(defaultDirection, angle);
            //direction.Normalize();

            // pivot in global
            Vector2 pivot = new Vector2(body[0].transform.position.x, body[0].transform.position.y);

            // position to apply external force
            Vector2 pos = pivot + body[0].GetComponent<SpriteRenderer>().size.y * direction;

            arrow.SetActive(true);

            if (Input.GetKey("g"))
            {
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                arrow.transform.position = pos;
                body[0].GetComponent<Rigidbody2D>().AddForceAtPosition(new Vector2(externalForce, 0), pos);
            }
            else
            {
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                arrow.transform.position = pos;
                body[0].GetComponent<Rigidbody2D>().AddForceAtPosition(new Vector2(-externalForce, 0), pos);
            }

        }
        else
        {
            arrow.SetActive(false);
        }
    }

    void MarkStanceLeg()
    {
        // mark the stance leg
        float markX = body[stanceKneeIndex].transform.position.x;
        float markY = body[stanceKneeIndex].transform.position.y;
        float markZ = body[stanceKneeIndex].transform.position.z - 2f;
        stancePoint.transform.position = new Vector3(markX, markY, markZ);
    }

    Vector2 RotateVector2(Vector2 v, float angle)
    {
        Vector3 result = Quaternion.Euler(new Vector3(0, 0, angle)) * new Vector3(v.x, v.y, 0);
        return new Vector2(result.x, result.y);
    }

    float PDControl(float desiredAngle, float angle, float angularVelocities)
    {
        //Debug.Log("current result: " + (kp * (desiredAngle - angle) - kd * angularVelocities).ToString("F4"));
        //Debug.Log("PD " + (kp * (desiredAngle - angle) - kd * angularVelocities).ToString("F4"));
        return kp * (desiredAngle - angle) - kd * angularVelocities;
    }

    float PDControl(float desiredAngle, float kpf, float kdf, float angle, float angularVelocities)
    {
        //Debug.Log("current result: " + (kp * (desiredAngle - angle) - kd * angularVelocities).ToString("F4"));
        //Debug.Log("PD " + (kp * (desiredAngle - angle) - kd * angularVelocities).ToString("F4"));
        return kpf * (desiredAngle - angle) - kdf * angularVelocities;
    }

    float BalanceFeedback(float defaultAngle, float cd, float cv, float d, float v)
    {
        return defaultAngle + cd * d + cv * v;
    }

    float BoundRange(float value, float min, float max)
    {
        if (value > max)
            return max;
        else if (value < min)
            return min;
        else
            return value;
    }

    float Deg2Rad(float x)
    {
        while (x > 180)
        {
            x -= 360;
        }
        while (x < -180)
        {
            x += 360;
        }
        return x * Mathf.Deg2Rad;
    }

    void JointLog(int i)
    {
        Debug.Log(body[i].name + ": angle = " + angles[i].ToString("F4") +
            " | angle speed = " + angularVelocities[i].ToString("F4") + " | torque = " + torques[i].ToString("F4") +
            " | angle degree = " + (angles[i] * Mathf.Rad2Deg).ToString("F4"));
    }

    void DrawArrow(Vector2 from, Vector2 to, Color color)
    {
        //Vector3 vf = new Vector3(from.x, from.y, 0);
        //Vector3 vt = new Vector3(to.x, to.y, 0);
        //Vector3[] v = { vf, vt };
        //Handles.BeginGUI();
        //Handles.color = color;
        //Handles.DrawAAPolyLine(v);
        //Vector2 v0 = from - to;
        //v0 *= 10 / v0.magnitude;
        //Vector2 v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
        //Vector2 v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f); ;
        //Handles.DrawAAPolyLine(3, to + v1, to, to + v2);
        //Handles.EndGUI();
    }

}
