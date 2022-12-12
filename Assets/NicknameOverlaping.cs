using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Скрипт для обработки ситуации когда никнеймы игроков перекрывают друг друга.
//После пересечения, персонаж распологается в одной из свободных поизиций неподалеку.
//В результате если в одном сегменте скопиться много игроков, их позиции будут распологаться как в кирпичной кладке
public class NicknameOverlaping : MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private BoxCollider2D _playerCollider;
    [SerializeField] private BoxCollider2D _nicknameCollider;

    private List<PlayerController> _otherPlayers; //Список окружающих игроков
    private List<Vector3> _supposedPositions; //Предпологаемые позиции вокруг текущего сегмента, в которые можно преместиться
    private int _lastSegmentIndex; //Индекс сегмента, относительно которого рассчитаны предпологаемые позиции
    public Vector3 reservedPosition { get; private set; } //Зарезервированная позиция к которой в данный момент движется игрок


    //МЕТОДЫ MonoBehaviour
    private void Start()
    {
        _otherPlayers = new List<PlayerController>();
        CalculateNewPositions();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Проверяем наличие остановившихся и перекрывающих друг друга игроков
        if (collision.collider.tag == "Player" && collision.otherCollider.tag == "Player")
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer.playerMovement.isMoving == false && _playerController.playerMovement.isMoving == false)
            {
                //Проверим, сменился ли сегмент на котором стоит игрок
                if (_playerController.сurrentSegmentIndex != _lastSegmentIndex)
                    CalculateNewPositions();

                //Ищем свободную позицию
                Vector3 freeNewPosition = FindFreeNewPosition();

                //Если такова имеется, то перемещаемся и резервируем эту позицию
                if (freeNewPosition != Vector3.zero)
                {
                    reservedPosition = freeNewPosition;
                    _playerController.playerMovement.SetWaypoints(new Vector3[] { freeNewPosition });
                }
            }
        }
    }



    //МЕТОДЫ КЛАССА

    //Метод возвращает, либо поизицию которая не занята другим игроком, и встав на которую, игрок не заденет соседние сегменты,
    //либо таких позиций нет и метод вернет нулевой вектор
    private Vector3 FindFreeNewPosition()
    {
        bool isFreePostion = true;
        foreach (Vector3 position in _supposedPositions)
        {
            isFreePostion = CheckForOtherPlayer(position);
            if (isFreePostion == false)
                continue;

            isFreePostion = CheckForOtherSegments(position);
            if (isFreePostion == false)
                continue;

            return position;
        }
        return Vector3.zero;
    }

    //Метод вычисляет предпологаемые позиции вокруг текущего сегмента
    private void CalculateNewPositions()
    {
        _lastSegmentIndex = _playerController.сurrentSegmentIndex;
        //Определим предполагаемые позиции на основе текущего прямоугольного тела никнейма и текущего сегмента
        float x = (_nicknameCollider.size.x + 0.15f) / 2;
        float y = _nicknameCollider.size.y + 0.15f;
        Vector3 currentSegment = _playerController.transform.position;
        _supposedPositions = new List<Vector3>
        {
            new Vector3() { x = currentSegment.x, y = currentSegment.y, z = 0 },
            new Vector3() { x = currentSegment.x - x, y = currentSegment.y + y, z = 0 },
            new Vector3() { x = currentSegment.x + x, y = currentSegment.y - y, z = 0 },
            new Vector3() { x = currentSegment.x + x, y = currentSegment.y + y, z = 0 },
            new Vector3() { x = currentSegment.x - x, y = currentSegment.y - y, z = 0 },
            new Vector3() { x = currentSegment.x, y = currentSegment.y + y, z = 0 },
            new Vector3() { x = currentSegment.x, y = currentSegment.y - y, z = 0 },
            new Vector3() { x = currentSegment.x - x, y = currentSegment.y, z = 0 },
            new Vector3() { x = currentSegment.x + x, y = currentSegment.y, z = 0 }
        };
    }

    //Метод проверяет позицию на наличие других игроков
    private bool CheckForOtherPlayer(Vector3 position)
    {
        //Получаем информацию об окружающих игроках
        _otherPlayers.Clear();
        foreach (Collider2D otherPlayerColliders in Physics2D.OverlapCircleAll(_nicknameCollider.transform.position, 5f))
        {
            if (otherPlayerColliders.tag == "Player") 
                _otherPlayers.Add(otherPlayerColliders.GetComponent<PlayerController>());
        }

        //Проверяем, направляется ли хотя бы один из окружающих игроков в эту позицию
        bool isFreePosition = true;
        foreach (PlayerController otherPlayer in _otherPlayers)
        {
            if (otherPlayer.playerMovement.waypoints.Last() == position && otherPlayer != this)
            {
                isFreePosition = false;
                break;
            }
        }

        //Проверяем, не находится ли в данной позиции чей-то никнейм
        Vector2 sizeWithBuffer = _nicknameCollider.size + new Vector2() { x = 0.15f, y = 0.15f };
        Collider2D[] hitSegments = Physics2D.OverlapBoxAll(position + _nicknameCollider.transform.parent.localPosition, sizeWithBuffer, 0, LayerMask.GetMask("UI"));
        if (hitSegments.Any())
        {
            isFreePosition = false;
        }

        return isFreePosition;
    }

    //Метод проверяет возможность встать на данную позицию не задев соседние сегменты
    private bool CheckForOtherSegments(Vector3 position)
    {
        bool isFreePosition = true;
        Vector2 sizeWithBuffer = _playerCollider.size + new Vector2() { x = 0.15f, y = 0.15f };
        Collider2D[] hitSegments = Physics2D.OverlapBoxAll(position, sizeWithBuffer, 0f, LayerMask.GetMask("Segment"));
        if (hitSegments.Any() == false)
            return isFreePosition;

        foreach (Collider2D collider in hitSegments)
        {
            if (collider.GetComponent<Segment>().segmentIndex != _playerController.сurrentSegmentIndex)
                isFreePosition = false;
        }

        return isFreePosition;
    }
}