using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraComponent cameraComponent;

    [Header("UI Variables")]
    [SerializeField] private GameObject Album;
    public bool isUIActive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
             if (!Album.activeSelf)
            {
                cameraComponent.LockedCursor(false);
                Album.SetActive(true);
                isUIActive = true;
            }
            else
            {
                cameraComponent.LockedCursor(true);
                Album.SetActive(false);
                isUIActive = false;
            }
        } 
    }
}
