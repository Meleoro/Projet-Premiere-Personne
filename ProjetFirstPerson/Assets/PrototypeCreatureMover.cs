using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeCreatureMover : MonoBehaviour
{
    
    private float timer;
    
    void Update()
    {
        timer += Time.deltaTime;

        /*if (timer > 5)
        {
            timer = 0;

            transform.position += new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        }*/


        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (Physics.Raycast(CameraManager.Instance.transform.position, CameraManager.Instance.transform.forward,
                    out RaycastHit hit,
                    100, LayerManager.Instance.groundLayer))
            {
                transform.position = hit.point;
            }
        }
    }
}
