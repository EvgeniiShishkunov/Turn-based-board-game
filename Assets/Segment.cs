using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Возможные типы сегментов
public enum SegmentType
{
    Usual,
    Bad,
    Good,
    Finish
}

//Скрипт описывающий сегмент по которым двигаются игроки.
public class Segment : MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private SegmentType _segmentType; 
    [SerializeField] private int _segmentIndex;  
    [SerializeField] private Color _usualSegmentColor; 
    [SerializeField] private Color _goodSegmentColor; 
    [SerializeField] private Color _badSegmentColor; 
    [SerializeField] private Color _finishSegmentColor;

    public SegmentType segmentType { get { return _segmentType; } } 
    public int segmentIndex { get { return _segmentIndex; } } 

    private int _stepsBackCount = 3; //Количество отходимых игроком сегмнетов назад за остановку на штрафном сегменте


    //Сопрограмма. Завершается после выполнения эффекта сегмента
    public IEnumerator Activate(GameController gameController)
    {
        switch (_segmentType)
        {
            case SegmentType.Bad:
                //Обновим информацию об кол-ве штрафных сегментов у игрока
                gameController._selectedPlayer.playerInformation.numberOfFines++;
                //Ждем выполнения эффекта сегмента
                yield return StartCoroutine(BadSegmentActivate(gameController));
                break;

            case SegmentType.Good:
                //Обновим информацию об кол-ве бонусных сегментов у игрока
                gameController._selectedPlayer.playerInformation.numberOfBonus++;
                //Ждем выполнения эффекта сегмента
                yield return StartCoroutine(GoodSegmentActivate(gameController));
                break;

            default:
                break;
        }
    }

    //Сопрограмма. Завершается после передвижения персонажа на несколько сегментов назад
    private IEnumerator BadSegmentActivate(GameController gameController)
    {
        if (gameController._selectedPlayer.playerMovement.isMoving == false)
        {
            int stepsBack = _stepsBackCount;
            //Если движение назад выведет персонажа за первый сегмент в маршруте, то пересчитаем количество сегментов для перемещения
            if (gameController._selectedPlayer.сurrentSegmentIndex - _stepsBackCount < 0)
                stepsBack = _stepsBackCount - (_stepsBackCount - gameController._selectedPlayer.сurrentSegmentIndex);

            Vector3[] waypoints = gameController.GetRangeInWaypoints(gameController._selectedPlayer.сurrentSegmentIndex - 1, stepsBack * -1);
            yield return StartCoroutine(gameController._selectedPlayer.MovePlayerByWaypoint(waypoints));
        }
    }

    //Сопрограмма. Завершается после повторного броска кубика и передвижения персонажа на выпавший результат
    private IEnumerator GoodSegmentActivate(GameController gameController)
    {
        yield return StartCoroutine(gameController.ExtraMove(gameController._selectedPlayer));
    }


#if UNITY_EDITOR
    //Валидация полей скрипта сегмента при изменении через редактор
    private void OnValidate()
    {
        //Предпологается, что сегмент находится в однотипной коллекции у родителя
        if (transform.parent != null)
        {
            //Меняем надпись номера на сегменте на порядковый номер в коллекции
            _segmentIndex = transform.GetSiblingIndex();
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (_segmentIndex + 1).ToString();
        }
        else
        {
            //Если сегмент не в коллекции, то меняем надпись номера сегмента на установленный в редакторе 
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (_segmentIndex + 1).ToString();
        }

        //Меняем тип сегмента на тот, который установили в редакторе
        switch (_segmentType)
        {
            case SegmentType.Usual:
                GetComponent<SpriteRenderer>().color = _usualSegmentColor;
                break;
            case SegmentType.Bad:
                GetComponent<SpriteRenderer>().color = _badSegmentColor;
                break;
            case SegmentType.Good:
                GetComponent<SpriteRenderer>().color = _goodSegmentColor;
                break;
            case SegmentType.Finish:
                GetComponent<SpriteRenderer>().color = _finishSegmentColor;
                break;
        }
    }
#endif

}
