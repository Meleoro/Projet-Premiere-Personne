using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : GenericSingletonClass<UIManager>
{
    [Header("References")]
    [SerializeField] private Image interactImage;

    
    #region SELECTION

    public void DisplayInteractIcon()
    {
        interactImage.gameObject.SetActive(true);
    }

    public void HideInteractIcon()
    {
        interactImage.gameObject.SetActive(false);
    }

    #endregion
}
