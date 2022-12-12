using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Segment;
using Random = UnityEngine.Random;

//������ ���������� ��������� ����
public class GameController : MonoBehaviour
{
    //����
    [SerializeField] private Transform _players; //������ �� ����� ������� �������� ���� �������
    [SerializeField] private Transform _waypoints; //������ �� ����� ������� �������� ������� ����� (��������)
    [SerializeField] private Rigidbody _diceBody; 
    [SerializeField] private TextMeshProUGUI _curentPlayerNicknameText; //����� �������� �������� ������
    [SerializeField] private Button _rollDiceButton; 

    public PlayerController _selectedPlayer { get; private set; } //��������� �����, ������� ������ ���������
    public int _selectedPlayerIndex { get; private set; }  //������ ���������� ������ � _players 
    public int _currentPlayerPlace { get; private set; } //������� �����, ������� ������ ����� ����� �� �������� �������
    public int _rollResult { get; private set; }  //������� �������� ��������� �� ������
    public int _playersZ { get; private set; }  //���������� z �� ������� ����� ������������ ���������



    //������ MonoBehaviour
    void Start()
    {
        _currentPlayerPlace = 1;
        _playersZ = 0;

        //� ������ ���� ����� ��������� ����� � ����� � � ������ ���� ����������, �������� ��������� ��������� � ������
        if (LoadInformation.players.Any() == false)
            LoadInformation.players.Add(new Player() { nickname = "Test Player" });

        //����� ��������� ��� ����������
        Vector3 playerStartPosition = _waypoints.GetChild(0).position;
        playerStartPosition.z = _playersZ;

        //������� ���������. ��������� ������ �� ����� � ����������� �� ������ � ������
        foreach (Player player in LoadInformation.players)
        {
            GameObject newPlayerOnScene = Instantiate(
                Resources.Load( "Prefabs/Player_" + player.color),
                playerStartPosition, 
                Quaternion.identity,
                _players.transform) as GameObject;
            newPlayerOnScene.GetComponent<PlayerController>().playerInformation = player;
        }

        //�������� �������� ������������ ������
        _selectedPlayerIndex = 0;
        _selectedPlayer = _players.GetChild(_selectedPlayerIndex).GetComponent<PlayerController>();
        _curentPlayerNicknameText.text = _selectedPlayer.playerInformation.nickname;
    }



    //������ ������

    //����� ��� ������ RollDiceBtutton. �������� ��� ������
    public void RollDiceBtutton()
    {
        StartCoroutine(StartPlayerActions());
    }

    //�����������. �������� ��������� �������� ������. ����������� ����������� ����� ���������� ���� ��������
    public IEnumerator StartPlayerActions()
    {
        //������ ������ ����������
        _rollDiceButton.interactable = false;

        //������ �����, ���� ���� �� ������� ���������
        yield return StartCoroutine(RollDice());
        yield return new WaitForSeconds(1);

        //����������� ������, ���� ���� �� �� ������ �� ����� ����������
        yield return StartCoroutine(MovePlayerFurther(_rollResult, _selectedPlayer));
        yield return new WaitForSeconds(1);

        //���������� ������ �������� �� ������� ����������� �����, ���� ����� �������
        yield return StartCoroutine(ActivateSegment());

        //���������, ����������� �� ����� �� ������
        CheckSegmentForFinish();

        //������� ���������� �� ���-�� ��������� ����� � ������
        _selectedPlayer.playerInformation.numberOfMoves++;

        //�������� ���������� �� ������������� ������
        SetNextPlayer();
 
        //���������, �� ��� �� ������������, ����������� ����
        CheckEndGame();

        //������ ������ ��������
        _rollDiceButton.interactable = true;
    }

    //�����������. ����������� ����� ��������� ���������� "������" ������
    private IEnumerator RollDice()
    {
        //������������ ��� � ��������� �����������, ����, �������������
        float[] randomNumbers = new float[2];
        for(int i = 0; i<2; i++)
        {
            if (Math.Round(Random.value) == 0)
                randomNumbers[i] += Random.Range(4f, 16f);
            else
                randomNumbers[i] -= Random.Range(4f, 16f);
        }
        Vector3 randomVector = new Vector3(randomNumbers[0], randomNumbers[1], 0);
        _diceBody.AddTorque(randomVector, ForceMode.Impulse);
        yield return new WaitForSeconds(2f);
        _diceBody.angularVelocity = Vector3.zero;

        //��������� ���� ����� ������������� ������ ���� � ����, ������� ���������� � �������. ��������� ���������.
        //���������� ��� ��������� �������� 6 � �������
        List<float> listAngel = new List<float>();
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.back)); //������ ������� ���� - 1
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.up)); //������� ������� ���� - 2
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.left)); //����� ������� ���� - 3
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.right)); //������ ������� ���� - 4
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.down)); //������ ������� ���� - 5
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.forward)); //������� ������� ���� - 6

        //����������� ������� �� �������, � ������� ���������� ����.
        _rollResult = listAngel.IndexOf(listAngel.Min()) + 1;

        //����������� ��� �������� � ������ �����������
        if (_rollResult == 1)
            _diceBody.transform.rotation = Quaternion.LookRotation(Vector3.back);
        else if (_rollResult == 2)
            _diceBody.transform.rotation = Quaternion.LookRotation(Vector3.up);
        else if (_rollResult == 3)
            _diceBody.transform.rotation = Quaternion.LookRotation(Vector3.left);
        else if (_rollResult == 4)
            _diceBody.transform.rotation = Quaternion.LookRotation(Vector3.right);
        else if (_rollResult == 5)
            _diceBody.transform.rotation = Quaternion.LookRotation(Vector3.down);
        else if (_rollResult == 6)
            _diceBody.transform.rotation = Quaternion.LookRotation(Vector3.forward);

        Debug.Log(_rollResult);
    }

    //�����������. ����������� ����� ��������� ������������ ��������� �� �����
    private IEnumerator MovePlayerFurther(int segmentsCountToMove, PlayerController player)
    {
        //���� �������� ��������� ���� ������� ������ �� �����, �� ����������� ���������� ��������� ��� �����������
        if (player.�urrentSegmentIndex + segmentsCountToMove > _waypoints.childCount - 1)
        {
            segmentsCountToMove = (_waypoints.childCount - 1) - player.�urrentSegmentIndex;
        }

        if (segmentsCountToMove > 0)
        {
            Vector3[] waypoints = GetRangeInWaypoints((player.�urrentSegmentIndex + 1), segmentsCountToMove);
            //���� ���������� ������������ ������
            yield return StartCoroutine(player.MovePlayerByWaypoint(waypoints));
        }
    }

    //�����������. ����������� ����� ���������� ������� ��������
    private IEnumerator ActivateSegment()
    {
        Segment segment = _waypoints.GetChild(_selectedPlayer.�urrentSegmentIndex).GetComponent<Segment>();
        yield return StartCoroutine(segment.Activate(this));
    }

    //���� �������� ����� �� ������, �� ����������� ��� �����.
    private void CheckSegmentForFinish()
    {
        if(_selectedPlayer.�urrentSegmentType == SegmentType.Finish)
        {
            _selectedPlayer.playerInformation.place = _currentPlayerPlace;
            _currentPlayerPlace++;
        }
    }

    //������������� ����������, �� ��������������� ������, ������� ����� ���������
    private void SetNextPlayer()
    {
        for (int i = 0; i < _players.childCount; i++)
        {
            _selectedPlayerIndex++;

            if (_selectedPlayerIndex > _players.childCount - 1)
                _selectedPlayerIndex = 0;

            if (LoadInformation.players[_selectedPlayerIndex].place == 0)
                break;
        }

        _selectedPlayer = _players.GetChild(_selectedPlayerIndex).GetComponent<PlayerController>();
        _curentPlayerNicknameText.text = _selectedPlayer.playerInformation.nickname;
    }

    //��������� ��� �� ������ �� ������. ���� ��, �� ��������� ������� �����������
    private void CheckEndGame()
    {
        bool hasUnfinishedPlayers = false;

        foreach (Player player in LoadInformation.players)
        {
            if (player.place == 0)
            {
                hasUnfinishedPlayers = true;
                break;
            }
        }

        if (hasUnfinishedPlayers == false)
        {
            SceneManager.LoadScene(2);
        }
    }

    //�������� ������ ���������� ����� �� ��������, ������� � ������� �������.
    //���� �������� ������������� ���-��, �� ������� ����� � �������� ����������� �� �������� �������
    public Vector3[] GetRangeInWaypoints(int index, int count)
    {
        Vector3[] result = new Vector3[Math.Abs(count)];
        if (count >= 0)
        {   
            for (int i = 0; i < count; i++)
            {
                result[i] = _waypoints.GetChild(index + i).position;
            }
        }
        else
        {
            count *= -1;
            for (int i = 0; i < count; i++)
            {
                result[i] = _waypoints.GetChild(index - i).position;
            }
        }
        return result;
    }

    //�����������. ������ �������������� ��� ��� ������� ������. ����������� ����� ��������� ������������
    public IEnumerator ExtraMove(PlayerController player)
    {
        yield return StartCoroutine(RollDice());
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(MovePlayerFurther(_rollResult, player));
        yield return new WaitForSeconds(1);
    }

}

