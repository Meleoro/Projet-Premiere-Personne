using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    [Header("terrain")]
    public GameObject terrain;
    public Vector3 terrainDimensions;

    [Header("Map")]
    public Image mapImage;

    [Header("Value")]
    public RectTransform rect;
    public Vector2 localPoint;

    public GameObject cubetest;
    // Start is called before the first frame update
    void Start()
    {
        terrainDimensions = terrain.GetComponent<Terrain>().terrainData.size;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClickImage()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition ,null, out localPoint);
        Instantiate(cubetest, new Vector3(localPoint.x / 2 ,0,localPoint.y / 2), Quaternion.identity);
        Debug.Log(localPoint);
    }
}
