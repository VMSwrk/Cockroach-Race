using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KP : MonoBehaviour
{
    public int valueKP; // Значение контрольной точки
    public int kpID; // ID контрольной точки
    public GameObject text;
    public GameObject sphere;
    // public Material material1, material2, material3, material4, material5;
    public Color color1, color2, color3, color4, color5, color0;

    void Start()
    {

        // Пишем начальный текст
        text.GetComponent<TextMesh>().text = valueKP.ToString();
        // Расскарашиваем точки
        Color color;
        switch (valueKP)
        {
            case 1:
                color = color1;
                break;
            case 2:
                color = color2;
                break;
            case 3:
                color = color3;
                break;
            case 4:
                color = color4;
                break;
            case 5:
                color = color5;
                break;
            default:
                color = color0;
                break;
        }

        sphere.GetComponent<MeshRenderer>().materials[0].color = color;

    }

}
