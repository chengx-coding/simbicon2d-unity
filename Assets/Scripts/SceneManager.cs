using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    static readonly int GroundCount = 2;
    public GameObject[] groundPrefabs = new GameObject[GroundCount];
    public int currentGroundIdx;
    private GameObject[] groundObjs = new GameObject[GroundCount];

    private void Awake()
    {
        if(currentGroundIdx < 0 || currentGroundIdx > 1)
        {
            currentGroundIdx = 0;
        }
        for(int i = 0; i < GroundCount; i++)
        {
            groundObjs[i] = Instantiate(groundPrefabs[i], this.transform.position, Quaternion.identity);
            groundObjs[i].transform.parent = this.transform;
            if (i == currentGroundIdx)
            {
                groundObjs[i].SetActive(true);
            }
            else
            {
                groundObjs[i].SetActive(false);
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SwitchGround();
    }

    void SwitchGround()
    {
        if (Input.GetKeyDown("v"))
        {
            groundObjs[currentGroundIdx].SetActive(false);
            currentGroundIdx = (currentGroundIdx + 1) % GroundCount;
            groundObjs[currentGroundIdx].SetActive(true);
        }
    }


}
