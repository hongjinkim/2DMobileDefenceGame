using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
	//������ ������ (������ �ְ� ���� �����͸� ���⿡ �ۼ�)
	public PlayerValue Value = new PlayerValue();
	public PlayerHeroValue Hero = new PlayerHeroValue();


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