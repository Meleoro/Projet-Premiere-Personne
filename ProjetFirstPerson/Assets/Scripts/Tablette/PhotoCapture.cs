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
    [SerializeField] private MoveComponent moveComponent;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BoardMenu boardMenu;
    [SerializeField] private CameraTestEthan cameraTestEthan;
    [SerializeField] private LogsMenu logsMenu;
    [SerializeField] private CanvasGroup photoGC;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Image SteleChargeImage;
    [SerializeField] private float ChargeLogsSpeed;
    [SerializeField] private float photoFadeOutDuration;

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
    private bool soundDone;

    private void Start()
    {
        soundDone = false;
        logsMenu = GetComponent<LogsMenu>();
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Suppression et création du dossier data
        if(Directory.Exists(Application.dataPath + "/Scripts/Tablette/Data"))
        {
            Directory.Delete(Application.dataPath + "/Scripts/Tablette/Data", true);
        }
        Directory.CreateDirectory(Application.dataPath + "/Scripts/Tablette/Data");

        ScreenRectTransform = ScreenPhotoImage.GetComponent<RectTransform>().rect;
        SteleChargeImage.fillAmount = 0;

      //  ScreenDetectionLogs = GameObject.Find("DetectPhotoScreen");
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCam.transform.position, ScreenDetectionLogs.transform.forward * maxDistance);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!viewingPhoto && cameraTestEthan.isAiming)
            {
                StartCoroutine(CapturePhoto());
            }
        }

        // Stele // Raycast
        if (cameraTestEthan.isAiming)
        {
            RaycastHit hit;
            if (Physics.BoxCast(mainCam.transform.position, ScreenDetectionLogs.transform.localScale,
                    ScreenDetectionLogs.transform.forward, out hit, mainCam.transform.localRotation, maxDistance,
                    layerMask))
            {
                //  Debug.Log("Detection Stele");
                var hitScript = hit.transform.GetComponent<SteleScript>();
                if (!hitScript.isAlreadyInLogs)
                {
                    if (!soundDone)
                    {
                        AudioManager.Instance.PlaySoundOneShot(1,20,0);
                        soundDone = true;
                    }
                    SteleChargeImage.gameObject.SetActive(true);
                    SteleChargeImage.fillAmount += ChargeLogsSpeed * Time.deltaTime;
                    
                    if (SteleChargeImage.fillAmount >= 1)
                    {
                        //hitScript.isAlreadyInLogs = true;
                        StartCoroutine(WaitAndLoopPopUp());
                        StartCoroutine(hitScript.ChangeShader());
                        hitScript.activationVFX.Play();
                        hitScript.fumeVFX.SetActive(false);
                        logsMenu.logPopUpAnim.clip = logsMenu.logPopUpAnim["NewLogAnim"].clip;
                        logsMenu.logPopUpAnim.Play();
                        AudioManager.Instance.PlaySoundOneShot(1, 17, 0);
                        string theInfo = hitScript.myInfo;
                        string theTitle = hitScript.titleLogs;
                        List<string> wordList = hitScript.ColorWordList;
                        GetComponent<LogsMenu>().TraductionButton.gameObject.SetActive(true);
                        GetComponent<LogsMenu>().AddLogsToContent(theInfo, theTitle,false,wordList);
                        hitScript.isAlreadyInLogs = true;
                        SteleChargeImage.fillAmount = 0;

                        if (hitScript.isFinalStele)
                        {
                            StartCoroutine(AutoGoToLog());
                        }
                    }
                }
            }
            else if (SteleChargeImage.fillAmount > 0)
            {
                SteleChargeImage.fillAmount -= ChargeLogsSpeed * Time.deltaTime;
                soundDone = false;
            }
        }
        else
        {
            SteleChargeImage.fillAmount = 0;
            soundDone = false;
        }
    }

    IEnumerator WaitAndLoopPopUp()
    {
        yield return new WaitForSeconds(logsMenu.logPopUpAnim.clip.length);
        logsMenu.logPopUpAnim.clip = logsMenu.logPopUpAnim["PopUpLogIdle"].clip;
        logsMenu.logPopUpAnim.Play();
    }
    
    IEnumerator AutoGoToLog()
    {
        uiManager.canMenu = false;
        cameraComponent.canRotate = false;
        cameraComponent.canMove = false;
        moveComponent.canMove = false;
        cameraTestEthan.AutoQuitPhoto();
        yield return new WaitForSeconds(1.5f);
        uiManager.canMenu = true;
        StartCoroutine(UIManager.Instance.OpenLogMenu());
    }
    
    IEnumerator CapturePhoto()
    {
        // Take Photos
        if(!uiManager.isUIActive)
        {
            SteleChargeImage.gameObject.SetActive(false);
            AudioManager.Instance.PlaySoundOneShot(1,19,0);
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
        //SteleChargeImage.gameObject.SetActive(true);
        StartCoroutine(FadeOutPhoto());
        }
    }

    void ShowPhoto()
    {
        photoGC.alpha = 1;
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f,0.0f, ScreenRectTransform.width, ScreenRectTransform.height), new Vector2(0.5f,0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;
        photoFrame.SetActive(true);

        SaveScreenShot();
    }
    

    IEnumerator FadeOutPhoto()
    {
        float timer = 0;
        while (timer < photoFadeOutDuration)
        {
            photoGC.alpha = Mathf.Lerp(photoGC.alpha, 0, timer / photoFadeOutDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        photoGC.alpha = 0;
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