using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[SerializeField]
public class LevelClass : MonoBehaviour  // Структура для уровня
{
    [SerializeField]
    public int numberCurenLevel { get; set; } // Номер уровня

    [SerializeField]
    public string levelJSON { get; set; }

    // КП
    [SerializeField]
    public int numberKP { get; set; } // Количество контрольных точек

    [SerializeField]
    public string positionKPJSON { get; set; }

    [SerializeField]
    public string valueKPJSON { get; set; }


    // Лабиринт
    [SerializeField]
    public int numberlabirint { get; set; } // Количество элементов лабиринта

    [SerializeField]
    public string positionLabirintJSON { get; set; }

    [SerializeField]
    public string rotationLabirintJSON { get; set; }

    [SerializeField]
    public string scaleLabirintJSON { get; set; }

}

[Serializable]
[SerializeField]
public class KPElement
{
    [SerializeField]
    public int numberKP;

    [SerializeField]
    public int valueKP;

    [SerializeField]
    public GameObject kp;
    // public List<GameObject> kpList = new List<GameObject>();

}