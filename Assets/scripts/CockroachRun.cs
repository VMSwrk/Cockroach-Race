using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockroachRun : MonoBehaviour
{
    public GameObject cockroachObject; // переменная хранящая ссылку на таракана
    public GameObject routeObject; // Объект маршрута
    public GameObject fieldObject; // Объект поля
    public Vector3 routeForce;
    // Первоначальные параметры
    public Vector3 nextPoint; // Позиция точки, куда будем двигаться
    public Vector3 currentPoint; // Текущая позиции таракана
    public Vector3 targetPoint;
    // public Vector3 routeForce; // Направление движение таракана
    public int numberPoint; // Номер текущей точки
    private int maxNumberPoint; // Максимальное количество точек в текущем маршруте
    public float deltaDistancePoint; // Минимальное растояние между точкой и положением таракана
    public float deltaRotatePoint; // Минимальное отклонение угла между точкой и положением таракана

    // Характеристики таракана, пока жесткие, потом будут менятся 
    public float speed; // скорость движения
    public float rotatingSpeed; // скорость поворота
    private List<Vector3> routeCockroach = new List<Vector3> { };
    private Rigidbody rb; // Необходимо для правильного движения
    private float heightPosToField; // Высота выгрузки таракана - берется из скрипта route потом удалить
    public int cockroachID; // ID маршрута
    public float biasFinisPosition = 20f; // Смещение таракана относительно стартовой и финишной позиции


    public enum stageGameEnum
    {
        move, // Режим движения
        wait, // Режим ожидания начала игры
        finish, // Режим когад таракан на финише
        route // Режим начертания маршрута
    }
    public stageGameEnum stageGame;

    // Переменные счета
    public int totalScore; // Общий счет
    public int curentScore; // Счет за текущую игру
    public bool finishScore; //Если истина, то значит разорвали финишную ленту и перестаем считать очки, но до последней точки добираемся 


    public bool[] occupationPoint; // Массив занятия точки, если true, то уже был на данной точке
    /////
    void Start()
    {
        fieldObject = GameObject.Find("Field");
        cockroachObject = (GameObject)this.gameObject;
        routeObject = GameObject.Find("route" + cockroachID.ToString());

        stageGame = stageGameEnum.wait; // Включили режим ожидания
        numberPoint = 1;

        // Создаем массив для отследвания занятий тараканом КП
        int numberKP = fieldObject.GetComponent<LevelMain>().numberKP;
        occupationPoint = new bool[numberKP];
        for (int i = 0; i < numberKP; i++)
        {
            occupationPoint[i] = false;
        }
        finishScore = false;

        // Начальные параметры движения
        routeCockroach = routeObject.GetComponent<Route>().route2;

        maxNumberPoint = routeCockroach.Count - 1;
        currentPoint = cockroachObject.transform.position; // Задали текущую позицию

        nextPoint = routeCockroach[numberPoint];

        // Корректируем последнюю точку, что бы убрать таракана за финишную ленту
        Vector3 cor = routeCockroach[maxNumberPoint];
        cor.z = biasFinisPosition;
        routeCockroach[maxNumberPoint] = cor;
        //////


        heightPosToField = routeObject.GetComponent<Route>().heightPosToField;

        // Очки
        curentScore = 0; // Сбросили счет
        fieldObject.GetComponent<LevelMain>().timeStart = Time.time; // Засекли время
    }

    void Update()
    {

    }
    void FixedUpdate()
    {
        switch (stageGame)
        {
            case stageGameEnum.route:
                break;
            case stageGameEnum.move:
                MoveCockroach();
                break;
            case stageGameEnum.wait:
                WaitCockroach();
                break;
            case stageGameEnum.finish:
                FinishCockroach();
                break;
        }
    }

    void WaitCockroach()
    {
        // Очки
        curentScore = 0; // Сбросили счет
        fieldObject.GetComponent<LevelMain>().timeStart = Time.time; // Засекли время
    }

    public void StartMove()
    {
        // Убираем стартовую преграду
        // GameObject.Find("Start partition").GetComponent<Renderer>().enabled = false;
        // GameObject.Find("Start partition").GetComponent<BoxCollider>().enabled = false;

        stageGame = stageGameEnum.move;
    }

    // Движение по маршруту
    void MoveCockroach()
    {
        // проверяем дельту до следующую точку, дельта меньше, то задаем следующую точку. далее поворачиваем к точке и бредем к ней

        if ((Mathf.Abs(currentPoint.x - nextPoint.x) < deltaDistancePoint) && // Проверяем расстояние до цели, если меньше, то меняем ориентир
            (Mathf.Abs(currentPoint.z - nextPoint.z) < deltaDistancePoint))
        {
            if (numberPoint == maxNumberPoint)
            {
                stageGame = stageGameEnum.finish;
                return;
            }
            else
            {
                numberPoint += 1;

                nextPoint = routeCockroach[numberPoint];
                nextPoint.y = heightPosToField;
            }
        }

        Vector3 targetDirection = nextPoint - currentPoint;
        targetDirection.y = heightPosToField;
        routeForce = Vector3.RotateTowards(transform.forward, targetDirection, rotatingSpeed * Time.fixedDeltaTime, 0.0f); // Расчитали куда повернуть
        routeForce.y = 0;
        cockroachObject.transform.rotation = Quaternion.LookRotation(routeForce); // Поверули

        targetPoint = Vector3.MoveTowards(currentPoint, nextPoint, speed * Time.fixedDeltaTime); // Расчитали куда бежать
        targetPoint.y = heightPosToField;
        cockroachObject.GetComponent<Rigidbody>().MovePosition(targetPoint); // Передвинулись

        currentPoint = cockroachObject.transform.position;
        currentPoint.y = heightPosToField;
    }

    void FinishCockroach() // Завершение игры
    {
        // cockroachObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        // Уползаем в угол

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.name == "KP(Clone)" && !finishScore) // Если прошли КТ
        {
            // ПРибавление балов, если прошли КТ
            int numberKP = col.GetComponent<KP>().kpID;

            if (!occupationPoint[numberKP])
            {
                curentScore = curentScore + col.gameObject.GetComponent<KP>().valueKP;
                occupationPoint[numberKP] = true; // Помечаем точку, как пройденую
                                                  // Надо вызвать функцию изменения времени?

                // ЗАпускаем анимацию
                col.gameObject.GetComponent<Animation>().Play("KP");
                col.gameObject.GetComponent<Animation>().Play("numberKP");
            }

        }

        if (col.transform.name == "FinishPoint")
        {
            // Прекращаем делать расчет, хотя до точки пусть добирается
            finishScore = true;
        }


    }

}