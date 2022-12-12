using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

//Скрипт для движения объекта
public class FollowWaypoints: MonoBehaviour
{
    //ПОЛЯ
    [SerializeField] private float _speed = 50f;
    [SerializeField] private Rigidbody2D _body;

    public bool isMoving { get; private set; } 
    public Vector3 direction { get; private set; } 
    public Vector3[] waypoints { get; private set; } 
    public int _targetPointIndex { get; private set; } //Индекс промежуточной точки в waypoints, к которой движется этот объект сейчас



    //МЕТОДЫ MonoBehaviour
    void Start()
    {
        isMoving = false;
        _targetPointIndex = 0;
        waypoints = new Vector3[1] { transform.position };
    }

    private void FixedUpdate()
    {
        //Если объекту задали маршрут, то движем объект
        if (isMoving == true)
        {
            //Вычисляем шаг перемещения,передвигаем объект
            float step = _speed * Time.deltaTime;
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, waypoints[_targetPointIndex], step);
            _body.MovePosition(nextPosition);

            //Если достигли промежуточной точки и это не конечная точка, то выбираем следущую точку в маршруте
            if (transform.position == waypoints[_targetPointIndex] && waypoints[_targetPointIndex] != waypoints.Last())
            {
                _targetPointIndex++;

                //Вычисляем направление движения
                direction = (waypoints[_targetPointIndex] - transform.position).normalized;
            }
            //Иначе если объект в конечной точке, устанавливаем статус объекта - не двигается
            else if (transform.position == waypoints.Last())
            {
                isMoving = false;
            }

        }
    }



    //МЕТОДЫ класса

    //Устанавливаем путевые точки для объекта. Объект начинает двигаться по ним
    //Важно! Не передавать сюда ссылки на объекты, иначе координата "z" у передаваемых точек поменяется.
    public void SetWaypoints(Vector3[] waypoints)
    {
        //Установим точки на одном уровне z с объектом, чтобы тот смог до них "добраться"
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].z = transform.position.z;
        }
        this.waypoints = waypoints;
        _targetPointIndex = 0;
        direction = (waypoints[_targetPointIndex] - transform.position).normalized;
        isMoving = true;
    }

}
