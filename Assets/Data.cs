using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//Класс для передачи информации между сценами
public static class LoadInformation
{
    public static List<Player> players = new List<Player>();
}


//Класс описывает информацию об игроке
public class Player: IComparable<Player>
{
    public string nickname;
    public string color = "Blue";
    public int place; //Занятое место, если 0, то никакое место еще не занято
    public int numberOfMoves; //Кол-во ходов
    public int numberOfBonus; //Кол-во бонусных секторов
    public int numberOfFines; //Кол-во штрафных секторов

    public int CompareTo(Player player)
    {
        if (player is null) throw new ArgumentException("null player");
        return place - player.place;
    }
}
