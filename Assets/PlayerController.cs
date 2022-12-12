using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Segment;

//������ ��� ���������� ����������
public class PlayerController : MonoBehaviour
{
    //����
    [SerializeField] private FollowWaypoints _playerMovement; //���������� ��������� ���������
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private TMPro.TextMeshProUGUI _nicknameTextFiled;
    [SerializeField] private BoxCollider2D _playerCollider;

    [HideInInspector] public FollowWaypoints playerMovement { get { return _playerMovement; } } //���������� ��������� ���������
    [HideInInspector] public SegmentType �urrentSegmentType { get; private set; }
    [HideInInspector] public int �urrentSegmentIndex { get; private set; }
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
    } //������ �� ���������� �� ������. ����� ���������� �������� ������ 1 ���
    private Player _playerInformation;



    //������ MonoBehaviour
    private void Update()
    {
        //������������� �������� ���� � ����������� �������.
        if (playerMovement.isMoving == true)
        {
            //������������ ������ � ������� ����
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
        //���������, ����� �� ����� �� �������. ��������� ������ � ������� ��������
        if (collision.tag == "Segment" && collision.IsTouching(_playerCollider) == true)
        {
            Segment segment = collision.gameObject.GetComponent<Segment>();
            �urrentSegmentIndex = segment.segmentIndex;
            �urrentSegmentType = segment.segmentType;
        }
    }



    //������ ������

    //�����������. ������������� � ������� ��������� �� �������� �����. ����������� ����������� ����� ���� ��� �������� ����������� (������ ����� ��������)
    public IEnumerator MovePlayerByWaypoint(Vector3[] waypoints)
    {
        playerMovement.SetWaypoints(waypoints);
        yield return new WaitUntil(() => playerMovement.isMoving == false);
    }

}
