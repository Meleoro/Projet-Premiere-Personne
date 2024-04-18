using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPanel : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
       Vector2 PosCenter =  new Vector2 (Screen.width / 2, Screen.height / 2).normalized;
       Debug.Log(PosCenter);
    }

    // Update is called once per frame
    void Update()
    {
    //    transform.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
        Debug.Log(Input.mousePosition.normalized);
        transform.GetComponent<RectTransform>().pivot = Input.mousePosition.normalized;
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(1,0,0) * Time.deltaTime * speed);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-1,0,0) * Time.deltaTime * speed);
        }
    }
}
