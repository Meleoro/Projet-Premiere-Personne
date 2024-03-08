using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    [field : SerializeField] public string name { get; private set; }
    [field : SerializeField] public string description { get; private set; }
    [field : SerializeField] public ObjectFunction function { get; private set; }
    
    [field : Header("Heal")]
    [field : ShowIf("function",ObjectFunction.heal)] 
    [field : SerializeField] public int HPHealed { get; private set; }
    
    [field : Header("Adrenaline")]
    [field : ShowIf("function", ObjectFunction.adrenaline)] 
    [field : SerializeField] public float effectDuration { get; private set; }
    [field : ShowIf("function", ObjectFunction.adrenaline)] 
    [field : SerializeField] public float speedIncrease { get; private set; }
    [field : ShowIf("function", ObjectFunction.adrenaline)] 
    [field : SerializeField] public float stamCostDecrease { get; private set; }
    
    [field : Header("Battery")]
    [field : ShowIf("function", ObjectFunction.battery)] 
    [field : SerializeField] public float batteryRecovered { get; private set; }
}

public enum ObjectFunction
{
    heal,
    adrenaline,
    battery
}
