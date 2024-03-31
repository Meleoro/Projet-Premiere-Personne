using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    [SerializeField] private GameObject cameraUI;
    private Texture2D screenCapture;
    private bool viewingPhoto;

    [Header("Other Variables")]
    [SerializeField] private Transform player;
    public List<Sprite> photoList;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!viewingPhoto)
            {
                StartCoroutine(CapturePhoto());
            }

            else
            {
                RemovePhoto();
            }
        }
    }

    IEnumerator CapturePhoto()
    {
        cameraUI.SetActive(false);
        viewingPhoto = true;

        yield return new WaitForEndOfFrame();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead,0,0,false);
        screenCapture.Apply();
        ShowPhoto();
    }

    void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f,0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f,0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;
        photoFrame.SetActive(true);
    
        photoList.Add(photoSprite);

     //   SaveScreenShot();

   /*     byte[] photoByte = screenCapture.EncodeToPNG();
        string fileName = DateTime.Now.ToString("MyPhoto" + ".png");
        string filePath = AssetDatabase.GenerateUniqueAssetPath("Assets/Scripts/Tablette/Data/MyPhotoClass");
        File.WriteAllBytes(filePath,photoByte); */

        // Add photo in scriptable object
        PhotoClass currentPhoto = ScriptableObject.CreateInstance<PhotoClass>();
        currentPhoto.index = photoList.Count;
        currentPhoto.posPhoto = player.position;
        currentPhoto.screenPhoto = photoList[photoList.Count];

        var uniqueFileName =  AssetDatabase.GenerateUniqueAssetPath("Assets/Scripts/Tablette/Data/PhotoClass.asset");
        AssetDatabase.CreateAsset(currentPhoto, uniqueFileName);
    }

    void RemovePhoto()
    {
        viewingPhoto = false;
        photoFrame.SetActive(false);
        cameraUI.SetActive(true);
    }

  /*  void SaveScreenShot()
    {
        string fileName = Path.GetRandomFileName() + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        byte[] pngShot = screenCapture.EncodeToPNG();

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllBytes(filePath, pngShot);

        Debug.Log("Screen saved to : " + filePath);
    } */
}