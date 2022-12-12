using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//������ ��� ��������� �������� ����� �������� ������� ����������� ���� �����.
//����� �����������, �������� ������������� � ����� �� ��������� �������� ����������.
//� ���������� ���� � ����� �������� ��������� ����� �������, �� ������� ����� ������������� ��� � ��������� ������
public class NicknameOverlaping : MonoBehaviour
{
    //����
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private BoxCollider2D _playerCollider;
    [SerializeField] private BoxCollider2D _nicknameCollider;

    private List<PlayerController> _otherPlayers; //������ ���������� �������
    private List<Vector3> _supposedPositions; //�������������� ������� ������ �������� ��������, � ������� ����� ������������
    private int _lastSegmentIndex; //������ ��������, ������������ �������� ���������� �������������� �������
    public Vector3 reservedPosition { get; private set; } //����������������� ������� � ������� � ������ ������ �������� �����


    //������ MonoBehaviour
    private void Start()
    {
        _otherPlayers = new List<PlayerController>();
        CalculateNewPositions();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //��������� ������� �������������� � ������������� ���� ����� �������
        if (collision.collider.tag == "Player" && collision.otherCollider.tag == "Player")
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer.playerMovement.isMoving == false && _playerController.playerMovement.isMoving == false)
            {
                //��������, �������� �� ������� �� ������� ����� �����
                if (_playerController.�urrentSegmentIndex != _lastSegmentIndex)
                    CalculateNewPositions();

                //���� ��������� �������
                Vector3 freeNewPosition = FindFreeNewPosition();

                //���� ������ �������, �� ������������ � ����������� ��� �������
                if (freeNewPosition != Vector3.zero)
                {
                    reservedPosition = freeNewPosition;
                    _playerController.playerMovement.SetWaypoints(new Vector3[] { freeNewPosition });
                }
            }
        }
    }



    //������ ������

    //����� ����������, ���� �������� ������� �� ������ ������ �������, � ����� �� �������, ����� �� ������� �������� ��������,
    //���� ����� ������� ��� � ����� ������ ������� ������
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

    //����� ��������� �������������� ������� ������ �������� ��������
    private void CalculateNewPositions()
    {
        _lastSegmentIndex = _playerController.�urrentSegmentIndex;
        //��������� �������������� ������� �� ������ �������� �������������� ���� �������� � �������� ��������
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

    //����� ��������� ������� �� ������� ������ �������
    private bool CheckForOtherPlayer(Vector3 position)
    {
        //�������� ���������� �� ���������� �������
        _otherPlayers.Clear();
        foreach (Collider2D otherPlayerColliders in Physics2D.OverlapCircleAll(_nicknameCollider.transform.position, 5f))
        {
            if (otherPlayerColliders.tag == "Player") 
                _otherPlayers.Add(otherPlayerColliders.GetComponent<PlayerController>());
        }

        //���������, ������������ �� ���� �� ���� �� ���������� ������� � ��� �������
        bool isFreePosition = true;
        foreach (PlayerController otherPlayer in _otherPlayers)
        {
            if (otherPlayer.playerMovement.waypoints.Last() == position && otherPlayer != this)
            {
                isFreePosition = false;
                break;
            }
        }

        //���������, �� ��������� �� � ������ ������� ���-�� �������
        Vector2 sizeWithBuffer = _nicknameCollider.size + new Vector2() { x = 0.15f, y = 0.15f };
        Collider2D[] hitSegments = Physics2D.OverlapBoxAll(position + _nicknameCollider.transform.parent.localPosition, sizeWithBuffer, 0, LayerMask.GetMask("UI"));
        if (hitSegments.Any())
        {
            isFreePosition = false;
        }

        return isFreePosition;
    }

    //����� ��������� ����������� ������ �� ������ ������� �� ����� �������� ��������
    private bool CheckForOtherSegments(Vector3 position)
    {
        bool isFreePosition = true;
        Vector2 sizeWithBuffer = _playerCollider.size + new Vector2() { x = 0.15f, y = 0.15f };
        Collider2D[] hitSegments = Physics2D.OverlapBoxAll(position, sizeWithBuffer, 0f, LayerMask.GetMask("Segment"));
        if (hitSegments.Any() == false)
            return isFreePosition;

        foreach (Collider2D collider in hitSegments)
        {
            if (collider.GetComponent<Segment>().segmentIndex != _playerController.�urrentSegmentIndex)
                isFreePosition = false;
        }

        return isFreePosition;
    }
}