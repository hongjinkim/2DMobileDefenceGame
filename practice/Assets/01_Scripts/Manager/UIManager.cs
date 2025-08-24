using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BasicSingleton<UIManager>
{
    [Header("UI Panel")]
    [SerializeField] private GameObject lobbyPanel; 
    [SerializeField] private GameObject stagePanel;

    [Header("FX")]
    [SerializeField] private CircleTransition transition;

    public void StartStageTransition()
    {
        transition.SetDarkToggleTargets(
            toEnable: new[] { stagePanel },
            toDisable: new[] { lobbyPanel }
        );

        // ��ο��� �� �غ� + UI ������(�� �����ӿ� �ٷ� ���̵���) �� 1������ ���� �� ���
        transition.PlayWaitCoroutine(
        atDarkCoroutine: () => StageManager.Instance.StagePrepareRoutine(),
        onFinished: () => StageManager.Instance.StageStart()
        );
    }

    // ����: Ʈ�����Ǹ� ����ϰ� �ƹ� �͵� �� �ϰ� ���� ��
    public void PlayOnlyTransition()
    {
        transition.SetDarkToggleTargets(null, null); // ��� ����
        transition.Play(); // �ݹ� ����
    }
}
