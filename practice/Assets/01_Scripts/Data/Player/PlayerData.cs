using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    //������ ������ (������ �ְ� ���� �����͸� ���⿡ �ۼ�)
    [TabGroup("Tabs", "PlayerValue"), HideLabel][InlineProperty] public PlayerValue Value = new PlayerValue();
    [TabGroup("Tabs", "Hero"), HideLabel][InlineProperty] public PlayerHeroValue Hero = new PlayerHeroValue();


    // ������ �ε�, ���� �������� �޾ƿ��� �κ� ���� ����
    public void LoadData()
	{
		TestLoadPlayerValue();
		TestLoadPlayerValue();
    }

	private void TestLoadPlayerValue()
	{
		// �׽�Ʈ�� ������ �ε�
		Value.MaxEnergy = 30;
		Value.CurrentEnergy = 5;
		Value.Gold = 1000;
		Value.Crystal = 200;
    }
	private void TestLoadPlayerHeroValue()
	{

    }
}