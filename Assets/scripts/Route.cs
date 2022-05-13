using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    // Параметры пути
    public LineRenderer routeLine;
    public Color routeColor = Color.red;
    public float lineWidth = 0.04f;
    public float minimumVertexDistance = 10;
    // -------
    public GameObject fieldObject; // Объект - игровое поле
    private GameObject canvasRoute; // Для работы с маршрутами
    private Vector3 startFieldSize = new Vector3(); // Размер стартовой позиции
    private Vector3 loadPosition = new Vector3(); // Позиция выгрузки таракана

    public GameObject pref_cockroach; // Объект таракана для высадки на стартовую позицию
    private Vector3 fieldSize = new Vector3(); // размеры игрового поля 

    private bool isRouteStarted; // Обозначает, что кнопка нажата

    public bool stageRoute = true; // переменная, которая говорит о том, идет построение маршрута или проигрывание игры
    static int maxCountStepRoute = 1; // максимальное количество шагов маршрута
    // public int stepRoute; // шаг, через который сохраняются точки маршрута;
    public int numberPointRoute = 0; // Обозначает номер точки в текущем шаге маршрута
    private Vector2 selectStartPoint; // Выбраная точка старта
    private int currentStep = 0; // Текущий шаг маршрута
    public Vector3 startPoint; // Стартовая точка таракана
    public float heightPosToField = 1.54f; // Высота выгрузки таракана и рисования пути
    public float biasStartPos = -20f; // Смещение таракана относительно стартовой и финишной позиции
    public List<Vector3> route2 = new List<Vector3> { };
    public int routeID; // ID маршрута
    public int numberLevel; // Номер уровня, для которого делается маршрут

    // Start is called before the first frame update
    void Start()
    {
        // Начальные значения маршрута
        routeLine.startColor = routeColor;
        routeLine.endColor = routeColor;
        routeLine.startWidth = lineWidth;
        routeLine.endWidth = lineWidth;
        isRouteStarted = false;
        stageRoute = true;
        // -------

        currentStep = 0;
        fieldObject = GameObject.Find("Field");
        // canvasRoute = GameObject.Find("CanvasRoute");
        // canvasRoute.SetActive(false);

        // Получаем углы поля для контроля не выхода маршрута за его пределы
        fieldSize = fieldObject.GetComponent<MeshRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stageRoute)
        {
            return;
        }
        numberLevel = fieldObject.GetComponent<LevelMain>().numberCurenLevel;

        if (Input.GetMouseButtonDown(0) & stageRoute)
        {
            Vector3 mousePos = GetWorldCoordinate(Input.mousePosition);
            mousePos.y = heightPosToField; // Корреткируем позицию над уровнем поля

            // Проверяем, что тыкнули в точке старта
            // Надо взять объект - старт, получить его координаты, и сверить. Можно и лучик пустить, но это пока лишнее
            // Если промахнулись, выводим префаб ошибочной точки, гасим её, и выходим
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Сам луч
            RaycastHit hitRay; // Место или объект попадания луча
            if (Physics.Raycast(ray, out hitRay) && (hitRay.transform.gameObject.name == "StartPoint"))
            {
                routeLine.SetPosition(0, mousePos);
                routeLine.positionCount = 1;

                numberPointRoute = 0; // Задаем, что маршрут только начинается
                mousePos.z = biasStartPos; // Задали смещение на первую точку, что бы все стартовали с одного места
                route2.Add(mousePos); // Добавили первую точку - стартовую позицию

                isRouteStarted = true; // Тригер нажатия кнопки
            }


        }

        if (Input.GetMouseButton(0) && isRouteStarted)
        {
            Vector3 mousePos = GetWorldCoordinate(Input.mousePosition);
            mousePos.y = heightPosToField; // Корреткируем позицию над уровнем поля

            // if (currentStep == 1) { selectStartPoint = mousePos; } // Задали стартовую точку
            float distance = Vector3.Distance(mousePos, routeLine.GetPosition(routeLine.positionCount - 1));

            // Проверяем насколько далеко точка (если рядом, то не ставим), и входит ли точка в экран (поле)
            if ((distance > minimumVertexDistance) &&
                (mousePos.x < (fieldSize.x / 2)) &&
                (mousePos.x > ((-1) * fieldSize.x / 2)) &&
                (mousePos.z < (fieldSize.z / 2)) &&
                (mousePos.z > ((-1) * fieldSize.z / 2)))
            {
                // Рисуем линию
                routeLine.positionCount++;
                numberPointRoute += 1;

                route2.Add(mousePos); // Добавляем точку в список
                routeLine.SetPosition(routeLine.positionCount - 1, mousePos); // Рисуем точку
            }
        }

        // Если кнопка отжата, то сбрасывавем флаг и переводим шаг маршрута
        if (Input.GetMouseButtonUp(0))
        {
            // Проверяем, что конечная точка равна финишу, если нет, то сбрасываем маршрут и все снова
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Сам луч
            RaycastHit hitRay; // Место или объект попадания луча
            if (Physics.Raycast(ray, out hitRay) && (hitRay.transform.gameObject.name == "FinishPoint"))
            {
                isRouteStarted = false;

                if (numberPointRoute > maxCountStepRoute) // Блокируем чертить этап, если превышено кол=во максмальных этапов
                {
                    currentStep += 1; // задаем увеличение текущего шага
                    if (currentStep <= maxCountStepRoute)
                    {
                        // Debug.Log("canvasRoute.SetActive(true)");
                        stageRoute = false;
                        // if (!canvasRoute.activeSelf)
                        // {
                        //     canvasRoute.SetActive(true);

                        // }
                    }

                }
            }
            else
            {
                route2.Clear(); // Очистили список
                routeLine.positionCount = 1;
            }

        }
    }

    private Vector3 GetWorldCoordinate(Vector3 mousePosition) // Получаем мировые координаты
    {
        Vector3 mousePos = new Vector3(mousePosition.x, mousePosition.y, 1);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}