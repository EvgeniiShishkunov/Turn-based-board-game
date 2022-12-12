using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������� ���� ���������
public enum SegmentType
{
    Usual,
    Bad,
    Good,
    Finish
}

//������ ����������� ������� �� ������� ��������� ������.
public class Segment : MonoBehaviour
{
    //����
    [SerializeField] private SegmentType _segmentType; 
    [SerializeField] private int _segmentIndex;  
    [SerializeField] private Color _usualSegmentColor; 
    [SerializeField] private Color _goodSegmentColor; 
    [SerializeField] private Color _badSegmentColor; 
    [SerializeField] private Color _finishSegmentColor;

    public SegmentType segmentType { get { return _segmentType; } } 
    public int segmentIndex { get { return _segmentIndex; } } 

    private int _stepsBackCount = 3; //���������� ��������� ������� ��������� ����� �� ��������� �� �������� ��������


    //�����������. ����������� ����� ���������� ������� ��������
    public IEnumerator Activate(GameController gameController)
    {
        switch (_segmentType)
        {
            case SegmentType.Bad:
                //������� ���������� �� ���-�� �������� ��������� � ������
                gameController._selectedPlayer.playerInformation.numberOfFines++;
                //���� ���������� ������� ��������
                yield return StartCoroutine(BadSegmentActivate(gameController));
                break;

            case SegmentType.Good:
                //������� ���������� �� ���-�� �������� ��������� � ������
                gameController._selectedPlayer.playerInformation.numberOfBonus++;
                //���� ���������� ������� ��������
                yield return StartCoroutine(GoodSegmentActivate(gameController));
                break;

            default:
                break;
        }
    }

    //�����������. ����������� ����� ������������ ��������� �� ��������� ��������� �����
    private IEnumerator BadSegmentActivate(GameController gameController)
    {
        if (gameController._selectedPlayer.playerMovement.isMoving == false)
        {
            int stepsBack = _stepsBackCount;
            //���� �������� ����� ������� ��������� �� ������ ������� � ��������, �� ����������� ���������� ��������� ��� �����������
            if (gameController._selectedPlayer.�urrentSegmentIndex - _stepsBackCount < 0)
                stepsBack = _stepsBackCount - (_stepsBackCount - gameController._selectedPlayer.�urrentSegmentIndex);

            Vector3[] waypoints = gameController.GetRangeInWaypoints(gameController._selectedPlayer.�urrentSegmentIndex - 1, stepsBack * -1);
            yield return StartCoroutine(gameController._selectedPlayer.MovePlayerByWaypoint(waypoints));
        }
    }

    //�����������. ����������� ����� ���������� ������ ������ � ������������ ��������� �� �������� ���������
    private IEnumerator GoodSegmentActivate(GameController gameController)
    {
        yield return StartCoroutine(gameController.ExtraMove(gameController._selectedPlayer));
    }


#if UNITY_EDITOR
    //��������� ����� ������� �������� ��� ��������� ����� ��������
    private void OnValidate()
    {
        //��������������, ��� ������� ��������� � ���������� ��������� � ��������
        if (transform.parent != null)
        {
            //������ ������� ������ �� �������� �� ���������� ����� � ���������
            _segmentIndex = transform.GetSiblingIndex();
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (_segmentIndex + 1).ToString();
        }
        else
        {
            //���� ������� �� � ���������, �� ������ ������� ������ �������� �� ������������� � ��������� 
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (_segmentIndex + 1).ToString();
        }

        //������ ��� �������� �� ���, ������� ���������� � ���������
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
