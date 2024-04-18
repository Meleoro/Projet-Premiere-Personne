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

        Rect ScreenToRead = new Rect(0f, 0f, Screen.width, Screen.height);
        ScreenToRead.center = new Vector2(1920 / 2 , 1080 / 2);
        float vecDis = Vector3.Distance(ScreenToRead.center, Input.mousePosition);
        Vector3 direction = (new Vector3(ScreenToRead.center.x,ScreenToRead.center.y,0) - Input.mousePosition);
        Vector3 finalDir = -direction.normalized * vecDis;
       // Debug.Log(Input.mousePosition);
       // Debug.Log(direction.normalized);
      //  Debug.Log(vecDis);
        Debug.Log(finalDir);

        transform.GetComponent<RectTransform>().pivot = finalDir.normalized;
       
       /// transform.GetComponent<RectTransform>().pivot = Input.mousePosition.normalized;
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(1,0,0) * Time.deltaTime * speed);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-1,0,0) * Time.deltaTime * speed);
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.localScale = new Vector3(1,1,0) * Time.deltaTime * speed;
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            transform.localScale = new Vector3(-1,-1,0) * Time.deltaTime * speed;
        }
    }
}
