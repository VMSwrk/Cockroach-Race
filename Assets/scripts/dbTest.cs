using UnityEngine;
using System.Collections;

using System.Collections.Generic;
// using System.Data.SQLite;
using SQLite4Unity3d;

// Определяем структуру для таблицы LevelDB
public class LevelDB
{
    [PrimaryKey, AutoIncrement, Indexed]
    public int Id { get; set; }
    [Unique, Indexed]
    public int level_number { get; set; }
    [Indexed]
    public int level_type { get; set; }
    [Unique, Indexed]
    public string data_level { get; set; }
}

public class dbTest : MonoBehaviour
{
    void Start()
    {
        // Создаем новое подключение к базе данных
        using (var db = new SQLiteConnection(Application.dataPath + "/StreamingAssets/db.bytes"))
        {
            // Делаем запрос на выборку данных
            IEnumerable<LevelDB> list = db.Query<LevelDB>("SELECT * FROM levelDB");

            // Читаем и выводим результат
            foreach (LevelDB levelDB in list)
            {
                const string frmt = "level_number: {0}; level_type: {1}; data_level: {2};";
                Debug.Log(string.Format(frmt,
                    levelDB.level_number,
                    levelDB.level_type,
                    levelDB.data_level
                   ));
            }

            // const string frmt2 = "SELECT INTO levelDB (level_number, level_type, data_level) VALUES ({0}, {1}, {2})";

            // IEnumerable<LevelDB> list = db.Query<LevelDB>(frmt2);

            // И не забываем закрыть соединение
            db.Close();
        }
    }
}