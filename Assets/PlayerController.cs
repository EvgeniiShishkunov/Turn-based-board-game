using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Segment;

//Скрипт для управления персонажем
public class PlayerController : MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private FollowWaypoints _playerMovement; //Управление движением персонажа
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private TMPro.TextMeshProUGUI _nicknameTextFiled;
    [SerializeField] private BoxCollider2D _playerCollider;

    [HideInInspector] public FollowWaypoints playerMovement { get { return _playerMovement; } } //Управление движением персонажа
    [HideInInspector] public SegmentType сurrentSegmentType { get; private set; }
    [HideInInspector] public int сurrentSegmentIndex { get; private set; }
    public Player playerInformation 
    { 
        get 
        { 
            return _playerInformation; 
        } 
        set 
        { 
            if (_playerInformation == null)
            {
                _nicknameTextFiled.text = value.nickname;
                _playerInformation = value;
            }
        }
    } //Ссылка на информацию об игроке. Можно установить значение только 1 раз
    private Player _playerInformation;



    //МЕТОДЫ MonoBehaviour
    private void Update()
    {
        //Контроллируем анимацию бега и направление спрайта.
        if (playerMovement.isMoving == true)
        {
            //Поворачиваем спрайт в сторону бега
            if (playerMovement.direction.x > 0)
                _sprite.flipX = false;
            else if (playerMovement.direction.x < 0)
                _sprite.flipX = true;

            _animator.SetBool("Run", true);
        }
        else
        {
            _animator.SetBool("Run", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Проверяем, зашел ли игрок на сегмент. Обновляем данные о текущем сегменте
        if (collision.tag == "Segment" && collision.IsTouching(_playerCollider) == true)
        {
            Segment segment = collision.gameObject.GetComponent<Segment>();
            сurrentSegmentIndex = segment.segmentIndex;
            сurrentSegmentType = segment.segmentType;
        }
    }



    //МЕТОДЫ КЛАССА

    //Сопрограмма. Устанавливаем и двигаем персонажа по маршруту точек. Сопрограмма завершается после того как персонаж остановился (достиг конца маршрута)
    public IEnumerator MovePlayerByWaypoint(Vector3[] waypoints)
    {
        playerMovement.SetWaypoints(waypoints);
        yield return new WaitUntil(() => playerMovement.isMoving == false);
    }

}
