using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}
