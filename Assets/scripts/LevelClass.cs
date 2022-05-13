using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[SerializeField]
public class LevelClass : MonoBehaviour  // Структура для уровня
{
    [SerializeField]
    public int numberCurenLevel; // { get; set; } // Номер уровня

    // КП
    // [SerializeField]
    public int numberKP; // { get; set; } // Количество контрольных точек

    // [SerializeField]
    public List<Vector3> positionKP = new List<Vector3>();

    // [SerializeField]
    public List<int> valueKP = new List<int>();


    // Лабиринт
    [SerializeField]
    public int numberlabirint; // { get; set; } // Количество элементов лабиринта

    // [SerializeField]
    public List<Vector3> positionLabirint = new List<Vector3>();

    // [SerializeField]
    public List<Quaternion> rotationLabirint = new List<Quaternion>();

    // [SerializeField]
    public List<Vector3> scaleLabirint = new List<Vector3>();

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