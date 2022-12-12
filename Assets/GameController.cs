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

//Скрипт управления процессом игры
public class GameController : MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private Transform _players; //Объект на сцене который содержит всех игроков
    [SerializeField] private Transform _waypoints; //Объект на сцене который содержит путевые точки (сегменты)
    [SerializeField] private Rigidbody _diceBody; 
    [SerializeField] private TextMeshProUGUI _curentPlayerNicknameText; //Текст никнейма текущего игрока
    [SerializeField] private Button _rollDiceButton; 

    public PlayerController _selectedPlayer { get; private set; } //Выбранный игрок, которым сейчас управляют
    public int _selectedPlayerIndex { get; private set; }  //Индекс выбранного игрока в _players 
    public int _currentPlayerPlace { get; private set; } //Текущее место, которое займет игрок встав на финишный сегмент
    public int _rollResult { get; private set; }  //Текущий выпавший результат на кубике
    public int _playersZ { get; private set; }  //Координата z на которой будут существовать персонажи



    //МЕТОДЫ MonoBehaviour
    void Start()
    {
        _currentPlayerPlace = 1;
        _playersZ = 0;

        //В случае если сразу запускаем сцену с игрой и в памяти нету персонажей, создадим тестового персонажа в памяти
        if (LoadInformation.players.Any() == false)
            LoadInformation.players.Add(new Player() { nickname = "Test Player" });

        //Точка появления для персонажей
        Vector3 playerStartPosition = _waypoints.GetChild(0).position;
        playerStartPosition.z = _playersZ;

        //Создаем персонажа. Связываем игрока на сцене с информацией об игроке в памяте
        foreach (Player player in LoadInformation.players)
        {
            GameObject newPlayerOnScene = Instantiate(
                Resources.Load( "Prefabs/Player_" + player.color),
                playerStartPosition, 
                Quaternion.identity,
                _players.transform) as GameObject;
            newPlayerOnScene.GetComponent<PlayerController>().playerInformation = player;
        }

        //Выбираем текущего управляемого игрока
        _selectedPlayerIndex = 0;
        _selectedPlayer = _players.GetChild(_selectedPlayerIndex).GetComponent<PlayerController>();
        _curentPlayerNicknameText.text = _selectedPlayer.playerInformation.nickname;
    }



    //МЕТОДЫ КЛАССА

    //Метод для кнопки RollDiceBtutton. Начинаем ход игрока
    public void RollDiceBtutton()
    {
        StartCoroutine(StartPlayerActions());
    }

    //Сопрограмма. Начинаем выполнять действия игрока. Сопрограмма завершается после завершения всех действий
    public IEnumerator StartPlayerActions()
    {
        //Делаем кнопку неактивной
        _rollDiceButton.interactable = false;

        //Крутим кубик, ждем пока не получим результат
        yield return StartCoroutine(RollDice());
        yield return new WaitForSeconds(1);

        //Передвигаем игрока, ждем пока он не дойдет до точки назначения
        yield return StartCoroutine(MovePlayerFurther(_rollResult, _selectedPlayer));
        yield return new WaitForSeconds(1);

        //Активируем эффект сегмента на котором остановился игрок, ждем конца эффекта
        yield return StartCoroutine(ActivateSegment());

        //Проверяем, остановился ли игрок на финише
        CheckSegmentForFinish();

        //Обновим информацию об кол-ве сделанных ходов у игрока
        _selectedPlayer.playerInformation.numberOfMoves++;

        //Выбираем следующего не финишировшего игрока
        SetNextPlayer();
 
        //Проверяем, не все ли финишировали, заканчиваем игру
        CheckEndGame();

        //Делаем кнопку активной
        _rollDiceButton.interactable = true;
    }

    //Сопрограмма. Завершается после получения результата "броска" кубика
    private IEnumerator RollDice()
    {
        //Раскручиваем куб в случайном направлении, ждем, останавливаем
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

        //Вычисляем углы между направлениями сторон куба и осью, которая направлена в монитор. Заполняем коллекцию.
        //Изначально куб направлен стороной 6 в монитор
        List<float> listAngel = new List<float>();
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.back)); //задняя сторона куба - 1
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.up)); //верхняя сторона куба - 2
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.left)); //левая сторона куба - 3
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.right)); //правая сторона куба - 4
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.down)); //нижняя сторона куба - 5
        listAngel.Add(Vector3.Angle(_diceBody.transform.forward, Vector3.forward)); //лицевая сторона куба - 6

        //Результатом считаем ту сторону, у которой наименьший угол.
        _rollResult = listAngel.IndexOf(listAngel.Min()) + 1;

        //Выравниваем куб стороной с нужным результатом
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

    //Сопрограмма. Завершается после окончания передвижения персонажа на сцене
    private IEnumerator MovePlayerFurther(int segmentsCountToMove, PlayerController player)
    {
        //Если выпавший результат куба выведет игрока за финиш, то пересчитаем количество сегментов для перемещения
        if (player.сurrentSegmentIndex + segmentsCountToMove > _waypoints.childCount - 1)
        {
            segmentsCountToMove = (_waypoints.childCount - 1) - player.сurrentSegmentIndex;
        }

        if (segmentsCountToMove > 0)
        {
            Vector3[] waypoints = GetRangeInWaypoints((player.сurrentSegmentIndex + 1), segmentsCountToMove);
            //Ждем завершения передвижения игрока
            yield return StartCoroutine(player.MovePlayerByWaypoint(waypoints));
        }
    }

    //Сопрограмма. Завершается после выполнения эффекта сегмента
    private IEnumerator ActivateSegment()
    {
        Segment segment = _waypoints.GetChild(_selectedPlayer.сurrentSegmentIndex).GetComponent<Segment>();
        yield return StartCoroutine(segment.Activate(this));
    }

    //Если персонаж стоит на финише, то присваиваем ему место.
    private void CheckSegmentForFinish()
    {
        if(_selectedPlayer.сurrentSegmentType == SegmentType.Finish)
        {
            _selectedPlayer.playerInformation.place = _currentPlayerPlace;
            _currentPlayerPlace++;
        }
    }

    //Устанавливаем следующего, не финишировавшего игрока, которым будем управлять
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

    //Проверяем все ли игроки на финише. Если да, то загружаем таблицу результатов
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

    //Получаем данное количество точек из маршрута, начиная с данного индекса.
    //Если передать отрицательное кол-во, то получим точки в обратном направлении от текущего индекса
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

    //Сопрограмма. Делаем дополнительный ход для данного игрока. Завершается после окончания передвижения
    public IEnumerator ExtraMove(PlayerController player)
    {
        yield return StartCoroutine(RollDice());
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(MovePlayerFurther(_rollResult, player));
        yield return new WaitForSeconds(1);
    }

}

