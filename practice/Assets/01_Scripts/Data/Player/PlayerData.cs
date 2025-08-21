using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
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
		TestLoadPlayerHeroValue();
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
		var master = DataBase.GetHeroData();

        // 1) ����(�÷���) Ű ���� (������ ��ü�� Hidden���� �ʱ�ȭ)
        Hero.EnsureCollectionKeys(master);

        // 2) �׽�Ʈ�� ���� ����: ������ ��ü�� Seen���� ����
        Hero.SetAllSeen(master);

        // 3) �׽�Ʈ�� ���� ����: �������� �� 3���� ȹ���Ѵٰ� ����
        var first3 = master.HeroDict.Keys.Take(3).ToList();
        if (first3.Count >= 1) Hero.Acquire(first3[0], level: 5, star: 1);
        if (first3.Count >= 2) Hero.Acquire(first3[1], level: 10, star: 2);
        if (first3.Count >= 3) Hero.Acquire(first3[2], level: 1, star: 3);

        // 4) �� ����(������ �ֵ鸸, �ߺ� ����, �ִ� 5ĭ)
        Hero.SetDeck("Team1", first3, maxSize: 5, ownedOnly: true);
    }
}