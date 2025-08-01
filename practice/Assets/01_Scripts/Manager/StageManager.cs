
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StageManager : BasicSingleton<StageManager>
{
    [ReadOnly] public int currentStage;
    [ReadOnly] public int level;
    [ReadOnly] public int exp;

    [ShowInInspector]
    private StageValue stageValue;

    public SkillChoiceUI skillChoiceUI;

    private Queue<LevelUpRequest> levelUpQueue = new Queue<LevelUpRequest>();
    private bool isLevelUpChoiceActive = false;

    public void StageStart()
    {
        InitStage();
        // ���� ���۽� 2�� ���� ���� ó��
        RequestLevelUp();
        RequestLevelUp();
    }

    private void InitStage()
    {
        InGameHeroManager.Instance.InstantiateHero();

        currentStage = PlayerManager.Instance.currentStage;
        Debug.Log($"stage {currentStage} started");
        if (DataBase.TryGetStageValue(currentStage, out var value))
        {
            stageValue = value;
        }
        else
        {
            Debug.Log($"id : {currentStage}�� �ش��ϴ� �����͸� �ҷ����µ� ����");
        }

        level = 1;
        exp = 0;
    }

    public void RequestLevelUp()
    {
        levelUpQueue.Enqueue(new LevelUpRequest());
        TryStartNextLevelUp(); // �̸��� ��Ȯ�ϰ� �ٲ㺽
    }

    private void TryStartNextLevelUp()
    {
        // ���� ������ �������� �ƴ� ���� ����!
        if (isLevelUpChoiceActive || levelUpQueue.Count == 0)
            return;

        isLevelUpChoiceActive = true;
        Time.timeScale = 0f;

        var choices = GenerateSkillChoices();
        skillChoiceUI.Show(choices, OnLevelUpChoiceMade);
    }

    private void OnLevelUpChoiceMade(SkillUpgradeValue chosen)
    {
        ApplyUpgrade(chosen);
        Time.timeScale = 1f;
        isLevelUpChoiceActive = false;
        levelUpQueue.Dequeue();

        // ������ ���� ���� �ٷ� ���� ť�� ���������� ����
        TryStartNextLevelUp();
    }

    // ���� ��ȭ/���� ��ȯ ����
    private void ApplyUpgrade(SkillUpgradeValue selected)
    {
        if (selected.Tier == ESkillUpgradeTier.Summon)
            InGameHeroManager.Instance.HeroSummonedIDs.Add(selected.ID);
        else
            /* ��ȭ ���� ���� */
            ;
    }

    // ������ 3�� ����
    private List<SkillUpgradeValue> GenerateSkillChoices()
    {
        var result = new List<SkillUpgradeValue>();
        var summonCandidates = InGameHeroManager.Instance.allSkillUpgrades
            .FindAll(x => x.Tier == ESkillUpgradeTier.Summon && !InGameHeroManager.Instance.HeroSummonedIDs.Contains(x.ID));

        // �̼�ȯ ���� �켱 3������
        var summonShuffle = new List<SkillUpgradeValue>(summonCandidates);
        Shuffle(summonShuffle);
        for (int i = 0; i < 3 && i < summonShuffle.Count; i++)
            result.Add(summonShuffle[i]);

        // �����ϸ� ��ȭ��
        if (result.Count < 3)
        {
            var otherCandidates = InGameHeroManager.Instance.allSkillUpgrades
                .FindAll(x => x.Tier != ESkillUpgradeTier.Summon)
                .FindAll(x => InGameHeroManager.Instance.HeroSummonedIDs.Contains(x.ID) /* or ��Ÿ ���� */);

            Shuffle(otherCandidates);

            int n = 0;
            while (result.Count < 3 && n < otherCandidates.Count)
                result.Add(otherCandidates[n++]);
        }
        return result;
    }

    // Fisher-Yates Shuffle
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T t = list[i];
            list[i] = list[j];
            list[j] = t;
        }
    }

    // �ܺο��� ���� �� ȣ���� ���� ����
    public void OnPlayerLevelUp()
    {
        RequestLevelUp();
    }

    // ������ Ÿ�� �� Ȯ���� �� �Ʒ� ���� Ȱ�� ����
    public class LevelUpRequest { /* Ȯ�� ����: � �������� ������? �� ���� �߰� ���� */ }
}
