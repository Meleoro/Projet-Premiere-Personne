using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ArrowObject;
    [SerializeField] private bool isFollowCursor;

    // Start is called before the first frame update
    void Start()
    {
        isFollowCursor = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            isFollowCursor = false;
        }
        if(isFollowCursor)
        {
            Vector3 dir = (ArrowObject.transform.position - Input.mousePosition).normalized;
            float distance = Vector3.Distance(ArrowObject.transform.position,Input.mousePosition);
            ArrowObject.transform.right = -dir;
            ArrowObject.transform.GetComponent<RectTransform>().sizeDelta  = new Vector2 (distance, 100);
        }
    }
}
