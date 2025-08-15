using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
	//������ ������ (������ �ְ� ���� �����͸� ���⿡ �ۼ�)
	public PlayerValue Value = new PlayerValue();

	public void LoadData()
	{
		TestLoadPlayerValue();
    }

	private Void TestLoadPlayerValue()
	{
		// �׽�Ʈ�� ������ �ε�
		Value.MaxEnergy = 30;
		Value.CurrentEnergy = 5;
		Value.Gold = 1000;
		Value.Crystal = 200;
		return default;
    }
}