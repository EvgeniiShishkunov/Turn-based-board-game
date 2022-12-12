using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//������ ��� ������ � �������� �������
public class ScoreTable : MonoBehaviour
{
    //����
    [SerializeField] private GameObject _rowPrefab; //������ ������ � �������
    private List<Player> _playersInformation; //���������� �� �������. ���� ����� ����� �� ������ ���������� �� ������� � ������ ����



    //������ MonoBehaviour
    void Start()
    {
        //���������, ����������� � ��������� ���������� �� �������
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



    //������ ������

    //������. ������ ���� ������
    public void ReplayGameButton()
    {
        SceneManager.LoadScene(0);
    }

    //������� ������ � ������� �������
    private void CreateRow(int playerIndex)
    {
        GameObject row = Instantiate(_rowPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Content").transform) as GameObject;

        //�������� ��������� ���� � ������ � �������� ����� �� ������ ����������
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
