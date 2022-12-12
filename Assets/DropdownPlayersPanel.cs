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

//������ ��� ��������� ��������� ����������� ������ � ���-�� �������
public class DropdownPlayersPanel : MonoBehaviour
{
    //����
    [SerializeField] private GameObject _playerPanelPrefab; //������ ����������� �������
    [SerializeField] private GameObject _playersPanel; //������ �� ����� �������� ������ �������
    [SerializeField] private TMP_Dropdown _dropdown; //Dropdown ������� �� ����� ������� ������ ��������������



    //������ MonoBehaviour
    private void Start()
    {
        //�������� ������ �������, ���� ���� ���������� �� ������� � ������, �� �������� ������ ����������� �� ������
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



    //������ ������

    //����� ������� ������ � ��������� ����������� �� ������
    private void CreatePlayerPanelFromMemory(int playerIndex)
    {
        //������� ������, �������� �� ������ �����, �������� ����� �� ������ ����������
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

    //����� ������� ������ � ��������� ����������� �� ���������
    private void CreatePlayerPanel(int playerNumber)
    {
        //������� ������, �������� �� ������ �����, �������� ����� �� ������ ����������
        GameObject playerPanel = Instantiate(_playerPanelPrefab, Vector3.zero, Quaternion.identity, _playersPanel.transform) as GameObject;
        TextMeshProUGUI[] textsFields = playerPanel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textField in textsFields)
        {
            if (textField.text == "Player 1")
            {
                textField.text = "Player " + playerNumber.ToString();
            }

            //�������� inputField ����� �� ���������
            if (textField.text == "Enter text...")
            {
                textField.text = "Player " + playerNumber.ToString();
            }
        }
    }

    //����� ������������� ������ ���-�� ������� �� �����
    public void SetPlayerPanels(int index)
    {
        //�������� �������� ����� �� dropdawn ��������
        if (Int32.TryParse(_dropdown.options[index].text, out int numberOfPanels))
        if (numberOfPanels <= 0)
            return;

        //� ������ ���� ������ �� ����� ������ ������������ ����� �������, ������� �������� � �����
        if (_playersPanel.transform.childCount > numberOfPanels)
        {
            for (int i = _playersPanel.transform.childCount - 1; i > numberOfPanels - 1; i--)
            {
                Destroy(_playersPanel.transform.GetChild(i).GameObject());
            }
        }
        //� ������ ���� ������ �� ����� ������ ������������ ����� �������, ��������� ������
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
