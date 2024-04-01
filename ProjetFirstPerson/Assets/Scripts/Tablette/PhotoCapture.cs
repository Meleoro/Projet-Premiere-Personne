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
    public List<PhotoClass> MyPhotos;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Suppression et création du dossier data
        Directory.Delete(Application.dataPath + "/Scripts/Tablette/Data", true);
        Directory.CreateDirectory(Application.dataPath + "/Scripts/Tablette/Data");
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

      // Test
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            photoDisplayArea.sprite = MyPhotos[0].screenPhoto;
            photoFrame.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            photoDisplayArea.sprite = MyPhotos[1].screenPhoto;
            photoFrame.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            photoDisplayArea.sprite = MyPhotos[2].screenPhoto;
            photoFrame.SetActive(true);
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

        SaveScreenShot();
    }

    void RemovePhoto()
    {
        viewingPhoto = false;
        photoFrame.SetActive(false);
        cameraUI.SetActive(true);
    }

    void SaveScreenShot()
    {
        // Enregistrement de la photo dans le dossier
        Directory.CreateDirectory(Application.dataPath + "/Scripts/Tablette/Data/Resources");

        string fileName = Path.GetRandomFileName();
        string filePath = Path.Combine(Application.dataPath + "/Scripts/Tablette/Data/Resources/" + fileName);
        byte[] pngShot = screenCapture.EncodeToPNG();

        File.WriteAllBytes(filePath + ".png", pngShot);
        UnityEditor.AssetDatabase.Refresh();

        // Chargement du fichier pour pouvoir le manipuler ensuite
        var texture = Resources.Load<Texture2D>(fileName);
        AddPhotoInScritableObject(texture);
    }

    void AddPhotoInScritableObject(Texture2D tex)
    {
        // Transformaion de la texture en sprite
        Sprite Mytex = Sprite.Create(tex, new Rect(0.0f,0.0f, 2048, 1024), new Vector2(0.5f,0.5f), 100.0f);

        // Add photo in scriptable object
        PhotoClass currentPhoto = ScriptableObject.CreateInstance<PhotoClass>();
        MyPhotos.Add(currentPhoto);
        currentPhoto.index = MyPhotos.Count;
        currentPhoto.posPhoto = player.position;
        currentPhoto.screenPhoto = Mytex;

        var uniqueFileName =  AssetDatabase.GenerateUniqueAssetPath("Assets/Scripts/Tablette/Data/MyPhoto.asset");
        AssetDatabase.CreateAsset(currentPhoto, uniqueFileName);
        
    }
}