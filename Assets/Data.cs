using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//����� ��� �������� ���������� ����� �������
public static class LoadInformation
{
    public static List<Player> players = new List<Player>();
}


//����� ��������� ���������� �� ������
public class Player: IComparable<Player>
{
    public string nickname;
    public string color = "Blue";
    public int place; //������� �����, ���� 0, �� ������� ����� ��� �� ������
    public int numberOfMoves; //���-�� �����
    public int numberOfBonus; //���-�� �������� ��������
    public int numberOfFines; //���-�� �������� ��������

    public int CompareTo(Player player)
    {
        if (player is null) throw new ArgumentException("null player");
        return place - player.place;
    }
}
