using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//Скрипт для работы с таблицей игроков
public class ScoreTable : MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private GameObject _rowPrefab; //Префаб строки в таблице
    private List<Player> _playersInformation; //Информация об игроках. Поле нужно чтобы не менять информацию об игроках в памяти игры



    //МЕТОДЫ MonoBehaviour
    void Start()
    {
        //Скопируем, отсортируем и отобразим информацию об игроках
        _playersInformation = new List<Player>(LoadInformation.players);
        _playersInformation.Sort();
        if (_playersInformation.Any())
        {
            for (int i = _playersInformation.Count - 1; i >= 0; i--)
            {
                CreateRow(i);
            }
        }
    }



    //МЕТОДЫ КЛАССА

    //Кнопка. Начать игру заново
    public void ReplayGameButton()
    {
        SceneManager.LoadScene(0);
    }

    //Создать строку в таблице игроков
    private void CreateRow(int playerIndex)
    {
        GameObject row = Instantiate(_rowPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Content").transform) as GameObject;

        //Получаем текстовые поля в строке и заменяем текст на нужную информацию
        TMPro.TextMeshProUGUI[] textFieldsInRaw = row.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (TextMeshProUGUI textField in textFieldsInRaw)
        {
            switch (textField.text)
            {
                case "Place":
                    textField.text = _playersInformation[playerIndex].place.ToString();
                    break;

                case "Nickname":
                    textField.text = _playersInformation[playerIndex].nickname.ToString();
                    break;

                case "Number of moves ":
                    textField.text = _playersInformation[playerIndex].numberOfMoves.ToString();
                    break;

                case "Number of bonus":
                    textField.text = _playersInformation[playerIndex].numberOfBonus.ToString();
                    break;

                case "Number of fines":
                    textField.text = _playersInformation[playerIndex].numberOfFines.ToString();
                    break;

                default: break;
            }
        }
    }

}
