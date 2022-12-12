using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Linq;

//������ ��� ���������� ������� ���� ����
public class MenuController : MonoBehaviour
{
    //����
    [SerializeField] private GameObject _playersPanel; //������ �� ����� �������� ������ �������



    //������ ������

    //������. ������ ����
    public void StartGameButton()
    {
        LoadInformation.players = new List<Player>();

        //�������� ���������� �� ������� � ������ ����
        for (int i=0; i < _playersPanel.transform.childCount; i++)
        {
            TMP_InputField inputFiled = _playersPanel.transform.GetChild(i).GetComponentInChildren<TMP_InputField>();
            //���� nickname �� ��� ������, �� ��������� ��� �� ���������
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