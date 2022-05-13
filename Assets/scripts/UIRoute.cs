using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoute : MonoBehaviour
{
    public GameObject field;

    public void SaveRoute()
    {

    }

    public void ClearRoute()
    {

    }

    public void DeleteRoute()
    {

    }

    public void LoadRoute()
    {

    }

    public void StartRoute()
    {
        Debug.Log("key Start");
        field.GetComponent<LevelMain>().StartMove();
    }
}
