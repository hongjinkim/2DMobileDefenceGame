using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameDataManager : BasicSingleton<GameDataManager>
{
    // �������� �ҷ����� �÷��̾� ������
    public PlayerData playerData = new PlayerData();



    public static DataBase DataBase => DataBase.Instance;
    public static PlayerData PlayerData => Instance.playerData;

}
