using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainGame : MonoBehaviour
{
    public int maxCountCocroach = 1; // Максимальное количество игроков (тараканов)
    public int curentLevel; // Текущий урвоень игры

    // Перечисление стадий игры
    public enum gameStageEnum
    {
        inicialization, // Инициализация игры (может лишнее и удалить)
        menu, // Работа с меню
        setting, // Работа с настройками
        ferma, // Тараканья ферма
        route, // Рисование маршрута
        wait, // Ожидание соперников
        run, // Забег
        finish // Окончания забега
    }
    public gameStageEnum gameStage;

    void Awake()
    {
        gameStage = gameStageEnum.inicialization;
        curentLevel = 1;

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
