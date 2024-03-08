using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private float selectDuration;
    [Range(0f, 1f)] [SerializeField] private float addedAlphaSelect;
    
    [Header("Public Infos")]
    public ItemData currentSlotItem;
    
    [Header("Private Infos")] 
    private Coroutine currentCoroutine;
    private List<float> originalAlphas = new();
    private List<float> selectedAlphas = new();
    private bool isSelected;
    
    [Header("References")] 
    [SerializeField] private List<Image> slotImages = new();


    private void Start()
    {
        for (int i = 0; i < slotImages.Count; i++)
        {
            originalAlphas.Add(slotImages[i].color.a);
            selectedAlphas.Add(slotImages[i].color.a + addedAlphaSelect);
        }
    }



    #region SELECT FUNCTIONS

    public void SelectSlot()
    {
        isSelected = true;
        
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(SelectSlotCoroutine());
    }

    private IEnumerator SelectSlotCoroutine()
    {
        float timer = 0;

        while (timer < selectDuration)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < slotImages.Count; i++)
            {
                slotImages[i].color = new Color(slotImages[i].color.r, slotImages[i].color.g, slotImages[i].color.b,
                    Mathf.Lerp(originalAlphas[i], selectedAlphas[i], timer / selectDuration));
            }
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    

    public void UnselectSlot()
    {
        isSelected = false;
        
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(UnselectSlotCoroutine());
    }
    
    private IEnumerator UnselectSlotCoroutine()
    {
        float timer = 0;

        while (timer < selectDuration)
        {
            timer += Time.deltaTime;
            
            for (int i = 0; i < slotImages.Count; i++)
            {
                slotImages[i].color = new Color(slotImages[i].color.r, slotImages[i].color.g, slotImages[i].color.b,
                    Mathf.Lerp(selectedAlphas[i], originalAlphas[i], timer / selectDuration));
            }
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    #endregion
    
    
    
}
