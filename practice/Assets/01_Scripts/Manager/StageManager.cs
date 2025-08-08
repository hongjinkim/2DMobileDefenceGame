
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageManager : BasicSingleton<StageManager>
{
    [ReadOnly] public int currentStage;
    [ReadOnly] public int level;
    [ReadOnly] public int exp;

    [ShowInInspector]
    private StageValue stageValue;

    public SkillChoiceUI skillChoiceUI;

    private bool _spawnStarted = false;
    private bool _choiceConsumed = false;   // �̹� ���� 1ȸ�� ���

    private Queue<LevelUpRequest> levelUpQueue = new Queue<LevelUpRequest>();
    private bool isLevelUpFlowActive = false;

    public void StageStart()
    {
        InitStage();
        _spawnStarted = false;
        StartCoroutine(StartAfterTwoLevelUpsOnce());
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

    private IEnumerator StartAfterTwoLevelUpsOnce()
    {
        if (_spawnStarted) yield break;

        RequestLevelUp();
        RequestLevelUp();

        // �̹� �÷ο찡 ���� ���۵� ������ ���
        yield return new WaitUntil(() => isLevelUpFlowActive);
        // �׸��� ���� ������ ���
        yield return new WaitUntil(() => !isLevelUpFlowActive);

        if (_spawnStarted) yield break;
        _spawnStarted = true;

        yield return StartCoroutine(StageSpawn());
    }

    private IEnumerator StageSpawn()
    {
        // Wave ID ��������
        foreach (var kv in stageValue.WaveValueDict)
        {
            var waveId = kv.Key;
            var wave = kv.Value;

            yield return StartCoroutine(RunOneWave(waveId, wave.SpawnDatas));
        }
    }

    private IEnumerator RunOneWave(int waveId, List<EnemySpawnData> waveList)
    {
        Debug.Log($"Wave {waveId} started, count:{waveList?.Count}");

        // �Һ� ����Ʈ�� �����ؼ� �����ϰ� ��ȸ
        var list = new List<EnemySpawnData>(waveList ?? new List<EnemySpawnData>());

        for (int i = 0; i < list.Count; i++)
        {
            var sd = list[i];
            Debug.Log($"[Wave {waveId}] {i}/{list.Count - 1} Spawn start: id={sd.EnemyID}, count={sd.SpawnCount}, delay={sd.SpawnDelay}");

            // fire-and-forget ��Ÿ�� (SpawnEnemy�� ��� �����ϴ� ����)
            EnemyManager.Instance.SpawnEnemy(sd.SpawnPattern, sd.SpawnCount, sd.EnemyID);

            // Ȥ�� EnemyManager�� busy �÷��׷� �ߺ� ȣ���� �����Ѵٸ� �� ������ �纸�غ�����
            yield return null;

            if (sd.SpawnDelay > 0f)
                yield return new WaitForSecondsRealtime(sd.SpawnDelay);
        }

        Debug.Log($"Wave {waveId} done.");
    }

    public void RequestLevelUp()
    {
        levelUpQueue.Enqueue(new LevelUpRequest());
        if (!isLevelUpFlowActive)
            StartLevelUpFlow();
    }

    private void StartLevelUpFlow()
    {
        if (levelUpQueue.Count == 0) return;

        isLevelUpFlowActive = true;
        Time.timeScale = 0f;

        ShowNextSelection();
    }

    private void ShowNextSelection()
    {
        if (levelUpQueue.Count == 0)
        {
            EndLevelUpFlow();
            return;
        }

        _choiceConsumed = false;            // �� â�� ��� ������ ����

        // ���⼭�� Dequeue ���� ����! ���� �Ϸ� ������ Dequeue
        var choices = GenerateSkillChoices();
        skillChoiceUI.Show(choices, OnLevelUpChoiceMade);
    }

    private void OnLevelUpChoiceMade(SkillUpgradeValue chosen)
    {
        // ���� ������/�ߺ� Ŭ�� ����
        if (_choiceConsumed) return;
        _choiceConsumed = true;

        ApplyUpgrade(chosen);
        Debug.Log($"{chosen.HeroID} �� {chosen.SkillID} �����");

        // �̹� ��û 1�� �Ҹ�
        if (levelUpQueue.Count > 0)
            levelUpQueue.Dequeue();

        // UI ���� �ִϸ��̼��� �ִٸ� unscaled�� �����ų�, ���� �ϷḦ ��ٸ� �� ���� Show
        StartCoroutine(ShowNextAfterUIClose());
    }

    private IEnumerator ShowNextAfterUIClose()
    {
        // SkillChoiceUI�� IsClosed�� �ִٸ� �װ� ��ٸ��� �� �ְ�
        // ������ �ּ� �� �������� ����� ��ħ/���̾ƿ� ���� ����
        // yield return new WaitUntil(() => skillChoiceUI.IsClosed);
        yield return null; // �� ������ ��� (unscaled)

        ShowNextSelection();
    }

    private void EndLevelUpFlow()
    {
        isLevelUpFlowActive = false;
        Time.timeScale = 1f;

    }


    // ���� ��ȭ/���� ��ȯ ����
    private void ApplyUpgrade(SkillUpgradeValue selected)
    {
        if (selected.Tier == ESkillUpgradeTier.Summon)
            InGameHeroManager.Instance.SummonHero(selected.HeroID);
        else
            /* ��ȭ ���� ���� */

            Debug.Log($"Applied upgrade: {selected.SkillID} on Hero: {selected.HeroID}");
            ;
    }

    // ������ 3�� ����
    private List<SkillUpgradeValue> GenerateSkillChoices()
    {
        var result = new List<SkillUpgradeValue>();
        var summonCandidates = InGameHeroManager.Instance.allSkillUpgrades
            .FindAll(x => x.Tier == ESkillUpgradeTier.Summon && !InGameHeroManager.Instance.HeroSummonedIDs.Contains(x.HeroID));

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
                .FindAll(x => InGameHeroManager.Instance.HeroSummonedIDs.Contains(x.HeroID) /* or ��Ÿ ���� */);

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
