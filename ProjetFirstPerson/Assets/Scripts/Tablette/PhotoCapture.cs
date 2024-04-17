using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraComponent cameraComponent;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BoardMenu boardMenu;
    [SerializeField] private CameraTestEthan cameraTestEthan;

    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    [SerializeField] private GameObject cameraUI;
    [SerializeField] private GameObject tabletteFrame;
    private Texture2D screenCapture;
    private bool viewingPhoto;

    [Header("Other Variables")]
    [SerializeField] private Transform player;

[Serializable]
    public class MyPhoto
{
    public int Myindex;
    public Vector3 MyposPhoto;
    public Sprite MyscreenPhoto;
}
public List<MyPhoto> MyPhotos = new List<MyPhoto>();

    [Header("Album Variables")]
    [SerializeField] private GameObject Album;
    [SerializeField] private GameObject SlotAlbum;

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
            if(!viewingPhoto && cameraTestEthan.isIn)
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
        if(!uiManager.isUIActive)
        {
            cameraUI.SetActive(false);
            tabletteFrame.SetActive(false);
            viewingPhoto = true;

            yield return new WaitForEndOfFrame();

            Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

            screenCapture.ReadPixels(regionToRead,0,0,false);
            screenCapture.Apply();

        ShowPhoto();
        }
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
        tabletteFrame.SetActive(true);
    }

    void SaveScreenShot()
    {
        // Enregistrement de la photo dans le dossier
        Directory.CreateDirectory(Application.dataPath + "/Scripts/Tablette/Data/Resources");

        string fileName = Path.GetRandomFileName();
        string filePath = Path.Combine(Application.dataPath + "/Scripts/Tablette/Data/Resources/" + fileName);
        byte[] pngShot = screenCapture.EncodeToPNG();

        File.WriteAllBytes(filePath + ".png", pngShot);

        // Chargement du fichier pour pouvoir le manipuler ensuite
        byte[] bytes = File.ReadAllBytes(filePath + ".png");
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        AddPhotoInScritableObject(texture);
    }

    void AddPhotoInScritableObject(Texture2D tex)
    {
        // Transformation de la texture en sprite
        Sprite Mytex = Sprite.Create(tex, new Rect(0.0f,0.0f, 1920, 1080), new Vector2(0.5f,0.5f), 100.0f);

        // Add photo in scriptable object
        AddPhotos(MyPhotos.Count,player.position,Mytex);
    }
    
    public void AddPhotos(int index, Vector3 pos, Sprite sprite)
          {
            MyPhoto tmp = new MyPhoto();
            tmp.Myindex = index;
            tmp.MyposPhoto = pos;
            tmp.MyscreenPhoto = sprite;

            MyPhotos.Add(tmp);

            // Add Photo to album
            GameObject AlbumSlot = SlotAlbum;
            AlbumSlot.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            boardMenu.AddElementOnBoard(AlbumSlot);
          }
}