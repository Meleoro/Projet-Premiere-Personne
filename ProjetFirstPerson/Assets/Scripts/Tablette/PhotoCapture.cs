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
    public GameObject ScreenPhotoImage;
    private Rect ScreenRectTransform;

    [Header("Other Variables")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject ScreenDetectionLogs;
    [SerializeField] private LayerMask layerMask;
    public float maxDistance;

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

        ScreenRectTransform = ScreenPhotoImage.GetComponent<RectTransform>().rect;

        ScreenDetectionLogs = GameObject.Find("DetectPhotoScreen");
    }
   /* void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ScreenDetectionLogs.transform.position, ScreenDetectionLogs.transform.forward * maxDistance);
    } */

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!viewingPhoto && cameraTestEthan.isAiming)
            {
                StartCoroutine(CapturePhoto());
            }
            else
            {
                
            }
        }
    }

    IEnumerator CapturePhoto()
    {
         // Raycast
        RaycastHit hit;
        if (Physics.BoxCast(ScreenDetectionLogs.transform.position, ScreenDetectionLogs.transform.localScale, ScreenDetectionLogs.transform.forward, out hit, ScreenDetectionLogs.transform.localRotation, maxDistance, layerMask))
        {
            Debug.Log("Detection");
            var hitScript = hit.transform.GetComponent<SteleScript>();
            if(!hitScript.isAlreadyInLogs)
            {
                hitScript.isAlreadyInLogs = true;
                string theInfo = hitScript.myInfo;
                string theTitle = hitScript.titleLogs;
                GetComponent<LogsMenu>().AddLogsToContent(theInfo,theTitle);
            }
        }

        // Take Photos
        if(!uiManager.isUIActive)
        {
            cameraUI.SetActive(false);
            tabletteFrame.SetActive(false);
            viewingPhoto = true;

            yield return new WaitForEndOfFrame();

            Rect regionToRead = new Rect(0f, 0f, ScreenRectTransform.width, ScreenRectTransform.height);
            regionToRead.center = new Vector2(1920 / 2 , 1080 / 2);

            screenCapture.ReadPixels(regionToRead,0,0,false);
            screenCapture.Apply();

        ShowPhoto();
        yield return new WaitForSeconds(1.5f);
        RemovePhoto();
        }
    }

    void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f,0.0f, ScreenRectTransform.width, ScreenRectTransform.height), new Vector2(0.5f,0.5f), 100.0f);
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
        Sprite Mytex = Sprite.Create(tex, new Rect(0.0f,0.0f, ScreenRectTransform.width, ScreenRectTransform.height), new Vector2(0.5f,0.5f), 100.0f);

        // Add photo in scriptable object
        AddPhotos(MyPhotos.Count,player.position,Mytex);
    }
    
    public void AddPhotos(int index, Vector3 pos, Sprite sprite)
          {
            MyPhoto tmp = new MyPhoto();
            tmp.Myindex = index;
            tmp.MyposPhoto = pos;
            tmp.MyscreenPhoto = sprite;
            Debug.Log(pos);
            

            MyPhotos.Add(tmp);

            // Add Photo to album
            GameObject AlbumSlot = SlotAlbum;
            AlbumSlot.GetComponent<SlotAlbum>().SlotImage.sprite = sprite;
            AlbumSlot.GetComponent<SlotAlbum>().ValueX.text = "x : " + string.Format("{0:0.00}", pos.x);
            AlbumSlot.GetComponent<SlotAlbum>().ValueY.text = "y : " + string.Format("{0:0.00}", pos.z);
          //  AlbumSlot.GetComponent<SlotAlbum>().ValueX.text = "x :" + pos.x;
          //  AlbumSlot.GetComponent<SlotAlbum>().ValueY.text = "y :" + pos.y;
            boardMenu.AddElementOnBoard(AlbumSlot);
          }
}