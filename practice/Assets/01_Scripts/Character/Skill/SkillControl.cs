using GoogleSheet.Core.Type;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ൿ ����
[System.Serializable]
public class SkillBehavior
{
    public ESkillBehaviorType type;
    //public SkillAbility ability;
    public float value; // �߰� ��ġ���� �ʿ��� ���
}


[UGS(typeof(ESkillBehaviorType))]
public enum ESkillBehaviorType
{
    None, // �ൿ ����
    OnHit, // ���� �� �ൿ
    OnCrit, // ũ��Ƽ�� ���� �� �ൿ
    OnKill, // �� óġ �� �ൿ
    Passive, // �������� �ൿ
    OnTimer // ���� �ð����� �ൿ
}

[UGS(typeof(ESkillUpgradeTier))]
public enum ESkillUpgradeTier // ��ų ���׷��̵� �ܰ�
{
    Acquisition,            // ó�� ȹ��
    Normal,       // �Ϲ� ��ȭ
    Advanced,    // ��� ��ȭ
    EvolveRequirement,// ���� ����
    Evolve            // ����
}

[UGS(typeof(ESkillUpgradeType))]
public enum ESkillUpgradeType // ��ų ���׷��̵� Ÿ��
{ 
    DamageUP,          // ������ ����
    ProjectileUp,      // ����ü ���� ����
    PierceUp,         // ���� ����
    AreaUp,           // ���� ����
}

[System.Serializable]
public class SkillUpgradeData
{
    public ESkillUpgradeTier upgradeType; // ���׷��̵� Ÿ��
    public float damageMultiplier;
    public float speedMultiplier;
    public float rangeMultiplier;
    public SkillBehavior[] newBehaviors; // ���ο� �ൿ��
    //public SkillBehavior[] modifiedBehaviors; // ���� �ൿ ����
    public string upgradeDescription;
}

public class SkillControl : MonoBehaviour
{
    private List<SkillBehavior> behaviors = new List<SkillBehavior>();
    private Dictionary<string, SkillUpgradeData> upgradeDict = new Dictionary<string, SkillUpgradeData>();

    public void UpgradeSkill(string id)
    {
        if(!upgradeDict.ContainsKey(id))
        {
            Debug.LogError($"Skill with ID {id} not found.");
            return;
        }
        var upgradeData = upgradeDict[id];

        // ���ο� �ൿ �߰�
        foreach (var behavior in upgradeData.newBehaviors)
        {
            behaviors.Add(behavior);
        }

        // ���� �ൿ ����
        //foreach (var modifiedBehavior in upgradeData.modifiedBehaviors)
        //{
        //    // ���� �ൿ ã�Ƽ� ��ü �Ǵ� ����
        //}
    }

    
}
