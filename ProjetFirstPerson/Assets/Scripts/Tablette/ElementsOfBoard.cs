using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsOfBoard : MonoBehaviour
{
    [SerializeField] public GameObject MyMovingObject;
    [SerializeField] public GameObject DeleteButton;
    [SerializeField] public GameObject FavoriteButton;
    [SerializeField] public bool isSelectedOneTime = false;
    [SerializeField] public bool isFavorite;
}
