using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

//Скрипт для обработки поведения выпадающего списка о кол-ве игроков
public class DropdownPlayersPanel : MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private GameObject _playerPanelPrefab; //Префаб создаваемых панелей
    [SerializeField] private GameObject _playersPanel; //Объект на сцене хранящий панели игроков
    [SerializeField] private TMP_Dropdown _dropdown; //Dropdown элемент на сцене который сейчас обрабатывается



    //МЕТОДЫ MonoBehaviour
    private void Start()
    {
        //Создадим панели игроков, если есть информация об игроках в памяти, то заполним панели инофрмацией из памяти
        if (LoadInformation.players.Any())
        {
            _dropdown.SetValueWithoutNotify(LoadInformation.players.Count - 2);
            for (int i = 0; i < LoadInformation.players.Count; i++)
            {
                CreatePlayerPanelFromMemory(i);
            }
        }
        else
        {
            for (int i = 1; i < 3; i++)
            {
                CreatePlayerPanel(i);
            }
        }
    }



    //МЕТОДЫ КЛАССА

    //Метод создает панель и заполняет информацией из памяти
    private void CreatePlayerPanelFromMemory(int playerIndex)
    {
        //Создаем панель, получаем из панели текст, заменяем текст на нужную информацию
        GameObject playerPanel = Instantiate(_playerPanelPrefab, Vector3.zero, Quaternion.identity, _playersPanel.transform) as GameObject;
        TextMeshProUGUI[] textsFields = playerPanel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textField in textsFields)
        {
            if (textField.text == "Player 1")
            {
                textField.text = "Player " + (playerIndex + 1).ToString();
            }
        }

        TMP_InputField inputField = playerPanel.GetComponentInChildren<TMP_InputField>();
        inputField.text = LoadInformation.players[playerIndex].nickname;
    }

    //Метод создает панель и заполняет информацией по умолчанию
    private void CreatePlayerPanel(int playerNumber)
    {
        //Создаем панель, получаем из панели текст, заменяем текст на нужную информацию
        GameObject playerPanel = Instantiate(_playerPanelPrefab, Vector3.zero, Quaternion.identity, _playersPanel.transform) as GameObject;
        TextMeshProUGUI[] textsFields = playerPanel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textField in textsFields)
        {
            if (textField.text == "Player 1")
            {
                textField.text = "Player " + playerNumber.ToString();
            }

            //Заполним inputField ником по умолчанию
            if (textField.text == "Enter text...")
            {
                textField.text = "Player " + playerNumber.ToString();
            }
        }
    }

    //Метод устанавливает данное кол-во панелей на сцене
    public void SetPlayerPanels(int index)
    {
        //Пытаемся получить число из dropdawn элемента
        if (Int32.TryParse(_dropdown.options[index].text, out int numberOfPanels))
        if (numberOfPanels <= 0)
            return;

        //В случае если сейчас на сцене больше необходимого числа панелей, удаляем ненужные с конца
        if (_playersPanel.transform.childCount > numberOfPanels)
        {
            for (int i = _playersPanel.transform.childCount - 1; i > numberOfPanels - 1; i--)
            {
                Destroy(_playersPanel.transform.GetChild(i).GameObject());
            }
        }
        //В случае если сейчас на сцене меньше необходимого числа панелей, добавляем панели
        else if (_playersPanel.transform.childCount < numberOfPanels)
        {
            int addNumber = numberOfPanels - _playersPanel.transform.childCount;
            for (int i = 0; i < addNumber; i++)
            {
                CreatePlayerPanel(_playersPanel.transform.childCount + 1);
            }
        }

    }
}
