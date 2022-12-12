using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

//������ ��� �������� �������
public class FollowWaypoints: MonoBehaviour
{
    //����
    [SerializeField] private float _speed = 50f;
    [SerializeField] private Rigidbody2D _body;

    public bool isMoving { get; private set; } 
    public Vector3 direction { get; private set; } 
    public Vector3[] waypoints { get; private set; } 
    public int _targetPointIndex { get; private set; } //������ ������������� ����� � waypoints, � ������� �������� ���� ������ ������



    //������ MonoBehaviour
    void Start()
    {
        isMoving = false;
        _targetPointIndex = 0;
        waypoints = new Vector3[1] { transform.position };
    }

    private void FixedUpdate()
    {
        //���� ������� ������ �������, �� ������ ������
        if (isMoving == true)
        {
            //��������� ��� �����������,����������� ������
            float step = _speed * Time.deltaTime;
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, waypoints[_targetPointIndex], step);
            _body.MovePosition(nextPosition);

            //���� �������� ������������� ����� � ��� �� �������� �����, �� �������� �������� ����� � ��������
            if (transform.position == waypoints[_targetPointIndex] && waypoints[_targetPointIndex] != waypoints.Last())
            {
                _targetPointIndex++;

                //��������� ����������� ��������
                direction = (waypoints[_targetPointIndex] - transform.position).normalized;
            }
            //����� ���� ������ � �������� �����, ������������� ������ ������� - �� ���������
            else if (transform.position == waypoints.Last())
            {
                isMoving = false;
            }

        }
    }



    //������ ������

    //������������� ������� ����� ��� �������. ������ �������� ��������� �� ���
    //�����! �� ���������� ���� ������ �� �������, ����� ���������� "z" � ������������ ����� ����������.
    public void SetWaypoints(Vector3[] waypoints)
    {
        //��������� ����� �� ����� ������ z � ��������, ����� ��� ���� �� ��� "���������"
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
