using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ൿ ����
[System.Serializable]
public class SkillBehavior
{
    public SkillBehaviorType type;
    //public SkillAbility ability;
    public float value; // �߰� ��ġ���� �ʿ��� ���
}

public enum SkillBehaviorType
{
    OnHit,
    OnCrit,
    OnKill,
    Passive,
    ActiveSkill
}

public enum SkillUpgradeType
{
    Acquisition,            // ó�� ȹ��
    BasicEnhancement,       // �Ϲ� ��ȭ
    AdvancedEnhancement,    // ��� ��ȭ
    BreakthroughRequirement,// ���� ����
    Breakthrough            // ����
}

[System.Serializable]
public class SkillUpgradeData
{
    public SkillUpgradeType upgradeType; // ���׷��̵� Ÿ��
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
