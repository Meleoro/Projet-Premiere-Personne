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
    [SerializeField] private UiManager uiManager;

    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    [SerializeField] private GameObject cameraUI;
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

        // Désactivation de l'album
        Album.SetActive(false);
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

      // Test Voir Photo
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            photoDisplayArea.sprite = MyPhotos[0].MyscreenPhoto;
            photoFrame.SetActive(true);
            Debug.Log(MyPhotos[0].MyposPhoto);
        }
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            photoDisplayArea.sprite = MyPhotos[1].MyscreenPhoto;
            photoFrame.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            photoDisplayArea.sprite = MyPhotos[2].MyscreenPhoto;
            photoFrame.SetActive(true);
        } 
    }

    IEnumerator CapturePhoto()
    {
        if(!uiManager.isUIActive)
        {
            cameraUI.SetActive(false);
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
        Debug.Log(Mytex);

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
            GameObject AlbumSlot = Instantiate(SlotAlbum, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity, Album.transform);
            AlbumSlot.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            AlbumSlot.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(Album.GetComponent<AlbumSlot>().SelectSlot);
          }

   /* public void RefreshAlbum(Sprite TheSprite)
          {
            for (int i = 0; i <= MyPhotos.Count ; i++)
            {
                GameObject AlbumSlot = Instantiate(SlotAlbum, Vector3.zero, Quaternion.identity, Album.transform);
                AlbumSlot.transform.GetChild(0).GetComponent<Image>().sprite = TheSprite;
            }
          } */
}