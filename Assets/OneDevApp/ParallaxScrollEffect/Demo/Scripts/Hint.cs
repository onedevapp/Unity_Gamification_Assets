using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{

    public float playerSpeed = 0f;

    // Update is called once per frame
    void Update()
    {
        //Player speed assinging based on directional arrows
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerSpeed = 15.0f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            playerSpeed = -15.0f;
        }
        else
        {
            playerSpeed = 0.0f;
        }
    }
}
