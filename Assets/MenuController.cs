using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Linq;

//Скрипт для управления главным меню игры
public class MenuController : MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private GameObject _playersPanel; //Объект на сцене хранящий панели игроков



    //МЕТОДЫ КЛАССА

    //Кнопка. Начать игру
    public void StartGameButton()
    {
        LoadInformation.players = new List<Player>();

        //Загрузим информацию из панелей в память игры
        for (int i=0; i < _playersPanel.transform.childCount; i++)
        {
            TMP_InputField inputFiled = _playersPanel.transform.GetChild(i).GetComponentInChildren<TMP_InputField>();
            //Если nickname не был введен, то установим ник по умолчанию
            if (inputFiled.text.Trim(' ') == "")
            {
                inputFiled.text = "Player " + (i+1).ToString(); 
            }

            ToggleGroup colorToggleGroup = _playersPanel.transform.GetChild(i).GetComponentInChildren<ToggleGroup>();

            LoadInformation.players.Add(new Player() { nickname = inputFiled.text , color = colorToggleGroup.ActiveToggles().FirstOrDefault().GetComponentInChildren<Text>().text });
        }

        SceneManager.LoadScene(1);
    }
}