using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SQLite4Unity3d;

public class LevelMain : MonoBehaviour
{
    // Основные параметры и загрузка уровня
    public int maxCountCocroach = 1; // Количество игроков, временное занчение, потом возьму из main
    public int numberCurenLevel; // Номер уровня
    public int numberKP; // Количество контрольных точек
    public int numberlabirint; // Количество элементов лабиринта
    public List<Vector3> positionKP; // Массив положений КП
    public List<int> valueKP; // Массив значений КП
    public List<Vector3> positionLabirint;
    public List<Vector3> scaleLabirint;
    public List<Quaternion> rotationLabirint;

    // Расчет времени
    public float timeStart; // Время старта
    public float timeScore; // Время пробега
    public string timeScoreText; // Время в текстовом выражении

    // Объекты
    public GameObject kpPref; // Префаб контрольной точки
    public GameObject cockroachPref; // Префаб таракана
    public GameObject roureUserPref; // Префаб маршрута пользователя
    public GameObject roureAIPref; // Префаб маршрута AI
    public GameObject labirintPref; // Преф лабиринта
    public TMP_Text TMP_timeScore;
    public TMP_Text TMP_curentScore;
    public bool modeGameRoute = false; // Флаг, показывающий, что мы работаем с маршрутами 

    public GameObject[] kp; // Создали массив, далее numberKP должно из сохранения приходить
    public GameObject[] labirint;
    GameObject[] cockroach;
    GameObject[] route;
    public GameObject uiRoute; // Работа с маршрутами

    public enum stageGameEnum
    {
        move, // Режим движения
        wait, // Режим ожидания начала игры после конструирования уровня
        finish, // Режим когад таракан на финише
        route, // Режим начертания маршрута
        over // уничтожение объектов и следующий уровень
    }
    public stageGameEnum stageGame;

    // public lavelStruct lavel; // Объявление структуры
    private string nameObject; // Для переименования
    [SerializeField]
    public string jsonObject;

    [SerializeField]
    public List<string> levelData;

    public LevelClass level { get; set; } // Объявили объект

    async void Start()
    {

        stageGame = stageGameEnum.wait; // Включили режим ожидания

        levelData = new List<string>();

        InicObject(); // Инициация данных

        DefaultLevelValues(); // Загрузили дфолтные параметры
        InstallationLevel();

        // ButtonDownLoad();
        HideUIRoute();

    }

    void HideUIRoute()
    {// скрываем управление маршрутом, если оно открыто
        if (uiRoute.activeSelf)
        {
            uiRoute.SetActive(false);
        }
    }

    void InicObject()
    {
        kp = new GameObject[numberKP]; // Создали массив, далее numberKP должно из сохранения приходить
        // positionKP = new Vector3[numberKP]; // Массив положений точек, далее будем загружать
        // valueKP = new int[numberKP];
        labirint = new GameObject[numberlabirint];
        // positionLabirint = new Vector3[numberlabirint];
        // rotationLabirint = new Quaternion[numberlabirint];
        // scaleLabirint = new Vector3[numberlabirint];

        level = new LevelClass();
        positionKP = new List<Vector3>();
        valueKP = new List<int>();
        positionLabirint = new List<Vector3>();
        rotationLabirint = new List<Quaternion>();
        scaleLabirint = new List<Vector3>();
    }
    void InstallationLevel()
    {

        for (int i = 0; i < level.numberKP; i++)
        {
            kp[i] = Instantiate(kpPref, positionKP[i], kpPref.transform.rotation, GameObject.Find("KP").transform); // Выгружаем КП

            kp[i].GetComponent<KP>().kpID = i;
            kp[i].GetComponent<KP>().valueKP = valueKP[i];
            nameObject = "KP" + i.ToString();
            kp[i].name = nameObject;

        }

        for (int i = 0; i < level.numberlabirint; i++)
        {
            labirint[i] = Instantiate(labirintPref, positionLabirint[i], rotationLabirint[i], GameObject.Find("labirint").transform);
            labirint[i].transform.localScale = scaleLabirint[i];
            nameObject = "labirint" + i.ToString();
            labirint[i].name = nameObject;
        }

        //     LoadUser();
        // }
        // void LoadUser()
        // {
        // Создаем участников
        cockroach = new GameObject[maxCountCocroach];
        route = new GameObject[maxCountCocroach];
        for (int i = 0; i < maxCountCocroach; i++)
        {
            if (i == 0) // для нулевого таракана роуте человека
            {
                route[i] = Instantiate(roureUserPref, new Vector3(0, 0, 0), roureUserPref.transform.rotation, GameObject.Find("RouteCocrkroach").transform);
            }
            else // Для АИ тараканов
            {
                // route[i] = Instantiate(roureAIPref, new Vector3(0, 0, 0), roureAIPref.transform.rotation, GameObject.Find("Unit").transform);

            }
            route[i].GetComponent<Route>().routeID = i;
            nameObject = "route" + i.ToString();
            route[i].name = nameObject; // Переименовываем маршрут

            cockroach[i] = Instantiate(cockroachPref, new Vector3(0, 0, 0), cockroachPref.transform.rotation, GameObject.Find("Unit").transform);
            cockroach[i].GetComponent<CockroachRun>().cockroachID = i;
            nameObject = "cocroach" + i.ToString();
            cockroach[0].name = nameObject; // Переименовали пользовательского таракана
        }

        // stageGame = stageGameEnum.wait;
    }

    void Update()
    {
        switch (stageGame)
        {
            case stageGameEnum.wait:
                WaitCockroach();
                break;
            case stageGameEnum.route:
                RouteCockroach();
                break;
            case stageGameEnum.move:
                MoveCockroach();
                break;
            case stageGameEnum.finish:
                FinishCockroach();
                break;
            case stageGameEnum.over:
                OverCockrouch();
                break;
        }
    }

    public void StartMove()
    {
        HideUIRoute(); // Убираем меню работы с маршрутом, если оно есть

        // Убираем стартовую преграду
        GameObject.Find("Start partition").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Start partition").GetComponent<BoxCollider>().enabled = false;

        for (int i = 0; i < maxCountCocroach; i++)
        {
            // cockroach[i].GetComponent<CockroachRun>().stageGame = CockroachRun.stageGameEnum.move;
            cockroach[i].GetComponent<CockroachRun>().StartMove();
        }
        stageGame = stageGameEnum.move;
    }

    void RouteCockroach()
    {
        route[0].SetActive(true);
        // Проверяем, сколько тараканов выстроило маршрут, и если все, то переходим в режим передвижения
        int numberCompliteRouter = 0;
        for (int i = 0; i < maxCountCocroach; i++)
        {
            if (!route[i].GetComponent<Route>().stageRoute)
            {
                numberCompliteRouter = numberCompliteRouter + 1;

            }
        }
        if (numberCompliteRouter == maxCountCocroach)
        {
            // Тут надо выгрузить таракана
            for (int i = 0; i < maxCountCocroach; i++)
            {
                // Debug.Log("i: " + i);
                // Debug.Log("cockroach[i].name: " + cockroach[i].name);
                // Debug.Log("route[i].name" + route[i].name);
                cockroach[i].transform.position = route[i].GetComponent<Route>().route2[0]; // Потом поменять на route2[0];
                cockroach[i].SetActive(true); // Показали
            }

            // Убираем у стартовой позиции колайдер
            GameObject.Find("StartPoint").GetComponent<BoxCollider>().enabled = false;


            stageGame = stageGameEnum.wait;
        }
    }
    void MoveCockroach()
    {
        // Расчет времени
        int finishScore = 0;
        for (int i = 0; i < maxCountCocroach; i++)
        {
            if (cockroach[i].GetComponent<CockroachRun>().finishScore)
            { finishScore = +1; }
        }


        if (finishScore < maxCountCocroach)
        {
            timeScore = Time.time - timeStart; // Подсчитали, сколько пробежали
                                               // Перевод в время в текст с :
            float seconds = Mathf.FloorToInt(timeScore);
            float miliseconds = (timeScore - seconds) * 100;
            timeScoreText = string.Format("{0:0}:{1:00}", seconds, miliseconds);
        }
        else
        {
            stageGame = stageGameEnum.finish;
            // Перевод в время в текст с :
            // Отображение времени и счета
            TMP_timeScore.text = timeScoreText;
            TMP_curentScore.text = cockroach[0].GetComponent<CockroachRun>().curentScore.ToString();

        }
    }

    void WaitCockroach()
    {
        // stageGame = stageGameEnum.route;
        // Сбросили текущее время и очки
        timeScore = 0f;
        timeScoreText = "0:00";

        // Показываем мень работы с маршрутом
        uiRoute.SetActive(true);
    }

    void FinishCockroach()
    {
        stageGame = stageGameEnum.over;
    }

    void OverCockrouch()
    {
        // Удяляем созданные объекты
        for (int i = 0; i < maxCountCocroach; i++)
        {
            // Destroy(route[i]);
            Destroy(cockroach[i]);
        }
        for (int i = 0; i < numberKP; i++)
        {
            Destroy(kp[i]);
        }
        for (int i = 0; i < numberlabirint; i++)
        {
            Destroy(labirint[i]);
        }

    }

    void GreatGameObject(int numberKP, int numberlabirint)
    {
        // Вынесли отдлеьно, т.к. создавать массивы можно только полсу получения информации об их количестве
        kp = new GameObject[numberKP]; // Создали массив, далее numberKP должно из сохранения приходить
        // positionKP = new Vector3[numberKP]; // Массив положений точек, далее будем загружать
        // valueKP = new int[numberKP];

        // Заполняем уровень лабиринтом
        labirint = new GameObject[numberlabirint];
        // positionLabirint = new Vector3[numberlabirint];
        // rotationLabirint = new Quaternion[numberlabirint];
        // scaleLabirint = new Vector3[numberlabirint];

    }
    // void SaveLevel()
    // {
    //     level = new LevelClass(numberKP, numberlabirint);

    //     level.numberCurenLevel = numberCurenLevel;
    //     level.numberKP = numberKP;
    //     level.numberlabirint = numberlabirint;

    //     valueKP = new int[level.numberKP];
    //     positionKP = new Vector3[level.numberKP];

    //     for (int i = 0; i < level.numberKP; i++)
    //     {
    //         valueKP[i] = kp[i].GetComponent<KP>().valueKP;
    //         positionKP[i] = kp[i].transform.position;
    //     }

    //     positionLabirint = new Vector3[level.numberlabirint];
    //     rotationLabirint = new Quaternion[level.numberlabirint];
    //     scaleLabirint = new Vector3[level.numberlabirint];

    //     for (int i = 0; i < level.numberlabirint; i++)
    //     {
    //         positionLabirint[i] = labirint[i].transform.position;
    //         rotationLabirint[i] = labirint[i].transform.rotation;
    //         scaleLabirint[i] = labirint[i].transform.localScale;
    //     }

    //     // level.routeAI = new List<Vector3>;
    //     // level.routeAI.Add(route[0]);

    //     // формуируем JSON объекты

    //     for (int i = 0; i < numberKP; i++)
    //     {
    //         level.positionKPJSON = JsonUtility.ToJson(positionKP[i]);
    //         level.valueKPJSON = JsonUtility.ToJson(valueKP[i]);
    //     }
    //     for (int i = 0; i < numberlabirint; i++)
    //     {
    //         level.positionLabirintJSON = JsonUtility.ToJson(positionLabirint[i]);
    //         level.rotationLabirintJSON = JsonUtility.ToJson(rotationLabirint[i]);
    //         level.scaleLabirintJSON = JsonUtility.ToJson(scaleLabirint[i]);
    //     }

    //     // jsonObject = JsonUtility.ToJson(level);
    //     // должны получить выборку по этому номеру, и если в выборке есть знаечние, то спросить перезаписать или отменить
    //     // var loadDb = Application.dataPath + "/StreamingAssets/" + "db.bytes";
    //     // var dbs = new SQLiteConnection(loadDb); // Получили базу

    //     var dbs = new DBService(); // Получили базу
    //     dbs.CreateLevel(level);

    //     // var levelInDB = dbs._connection.Query<LevelClass>(query) as List<LevelClass>;
    //     // Debug.Log("count: " + levelInDB.Count);



    //     // using (var db = new SQLiteConnection("db.bytes", true))
    //     // {
    //     //     levelInDB = db.Get <
    //     // }



    //     // foreach (var LevelClass in levelInDB)
    //     // {
    //     //     Debug.Log(levelInDB.ToString());
    //     // }

    //     // Debug.Log("level: " + JsonUtility.ToJson(level));
    //     // Debug.Log("levelInDB: " + JsonUtility.ToJson(levelInDB));
    //     // if (JsonUtility.ToJson(level) == JsonUtility.ToJson(levelInDB))
    //     // {
    //     //     Debug.Log("Данный уровень уже сохранен");
    //     // }
    //     // else
    //     // {
    //     //     Debug.Log("Что, уровни не равны?");
    //     // }


    //     // if (jsonObject != levelData[numberCurenLevel - 1])
    //     // {
    //     //     levelData.Add(jsonObject);
    //     //     Debug.Log("Добавлен уровень: " + (numberCurenLevel - 1));
    //     // }
    //     // else
    //     // {
    //     //     Debug.Log("Урвоень не отличается");
    //     // }


    //     // BinaryFormatter bf = new BinaryFormatter();
    //     // FileStream file = File.Create(Path.Combine(Application.persistentDataPath, "MySaveData.json"));
    //     // // Debug.Log("path: " + Path.Combine(Application.persistentDataPath, "MySaveData.json"));

    //     // bf.Serialize(file, levelData);
    //     // file.Close();

    void DefaultLevelValues()
    {
        level.numberCurenLevel = 1;
        level.numberKP = 4;
        level.numberlabirint = 2;

        positionKP.Add(new Vector3(5.75f, 0.56f, -3.71f));
        positionKP.Add(new Vector3(10.43f, 0.56f, -13.09f));
        positionKP.Add(new Vector3(-10.59f, 0.56f, -0.87f));
        positionKP.Add(new Vector3(1.45f, 0.56f, -8.28f));
        valueKP.Add(1);
        valueKP.Add(1);
        valueKP.Add(2);
        valueKP.Add(3);

        positionLabirint.Add(new Vector3(3.09f, 1.5f, -8f));
        positionLabirint.Add(new Vector3(-3.84f, 1.5f, -0.80f));
        rotationLabirint.Add(new Quaternion(0f, -0.29f, 0f, 0.96f));
        rotationLabirint.Add(new Quaternion(0f, 0.39f, 0f, 0.92f));
        scaleLabirint.Add(new Vector3(0.5f, 2f, 11f));
        scaleLabirint.Add(new Vector3(0.5f, 2f, 14f));

    }

    public void ButtonDownLoad()
    {
        // Очищаем объекты поля
        OverCockrouch();

        var dbs = new DBService(); // Получили базу

        level = dbs.GetLevel(numberCurenLevel);

        InstallationLevel();

    }

    private void FillLevel()
    {

    }

    public void ButtonDownSave()
    {
        var dbs = new DBService(); // Получили базу
                                   // Debug.Log("levelJSON: " + JsonUtility.ToJson(level));
                                   // level.levelJSON = JsonUtility.ToJson(level);

        level = new LevelClass();
        level.positionKPJSON = JsonUtility.ToJson(positionKP);
        level.valueKPJSON = JsonUtility.ToJson(valueKP);
        level.positionLabirintJSON = JsonUtility.ToJson(positionLabirint);
        level.rotationLabirintJSON = JsonUtility.ToJson(rotationLabirint);
        level.scaleLabirintJSON = JsonUtility.ToJson(scaleLabirint);
        level.levelJSON = JsonUtility.ToJson(level);

        Debug.Log(level);
        dbs.CreateLevel(level);
        // level.test = new Vector3[numberKP];
        // positionKP = new List<Vector3>();

        // Debug.Log("kp:" + JsonUtility.ToJson(level.kp));
        // Debug.Log("test:" + JsonUtility.ToJson(level.test));
        // Debug.Log("positionKP (list):" + JsonUtility.ToJson(positionKP));



        // level.ToJSON();

    }
    public void ButtonDownGreat()
    {

    }
}

// Объекты для сохранения (серилизации)

[Serializable]
[SerializeField]
public class RouteClass // Структура для уровня
{
    [SerializeField]
    public int numberLevel; // Номер уровня, для которого применяется маршрут 
    [SerializeField]
    public Vector3[] routePoint; // Массив точек маршрута

}