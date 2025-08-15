using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class CircleTransition : MonoBehaviour
{
    [Header("Overlay Image (full-screen panel)")]
    [SerializeField] private Image overlayImage;        // ��ü ȭ�� ���� Image (Material= UI/CircleWipePixel)
    [SerializeField] private string radiusProp = "_Radius";
    [SerializeField] private string centerProp = "_Center";
    [SerializeField] private string featherProp = "_Feather";
    [SerializeField] private string rectProp = "_RectSize";
    [SerializeField] private string colorProp = "_Color";

    [Header("Timing")]
    [SerializeField] private float closeDuration = 0.35f;
    [SerializeField] private float darkHold = 0.15f;
    [SerializeField] private float openDuration = 0.45f;

    [Header("Circle Params (normalized by rect height)")]
    [SerializeField] private float startRadius = 1.5f;     // ȭ�� �ۿ��� ����
    [SerializeField] private float endRadius = 0.00f;    // ���� ���� ����
    [SerializeField] private float feather = 0.20f;
    [SerializeField] private Color overlayColor = new(0, 0, 0, 0.85f);
    [SerializeField, Tooltip("Rect ���� 0~1")] private Vector2 center01 = new(0.5f, 0.5f);

    [Header("Toggle at Dark (optional)")]
    [SerializeField] private List<GameObject> enableOnDark = new();   // ��ο� �� �� �гε�
    [SerializeField] private List<GameObject> disableOnDark = new();  // ��ο� �� �� �гε�

    [Header("Input Blocking")]
    [SerializeField] private bool blockPointerDuringTransition = true;         // ���콺/��ġ
    [SerializeField] private bool blockNavigationDuringTransition = true;      // Ű����/�е� Submit/Cancel ��
    [SerializeField] private EventSystem eventSystem;                           // ����: ���� ������ EventSystem.current ���

    Material mat;
    int _radiusId, _centerId, _featherId, _rectId, _colorId;
    Tweener tweenClose, tweenOpen;

    // ������ ����
    bool _prevRaycastTarget;
    bool _prevSendNavigationEvents;

    void Awake()
    {
        if (!overlayImage)
        {
            Debug.LogError("[CircleTransition] overlayImage�� �ʿ��մϴ�.", this);
            enabled = false;
            return;
        }

        // ��Ƽ���� �ν��Ͻ�ȭ (���� ����)
        mat = Instantiate(overlayImage.material);
        overlayImage.material = mat;

        _radiusId = Shader.PropertyToID(radiusProp);
        _centerId = Shader.PropertyToID(centerProp);
        _featherId = Shader.PropertyToID(featherProp);
        _rectId = Shader.PropertyToID(rectProp);
        _colorId = Shader.PropertyToID(colorProp);

        ApplyStaticParams();
        overlayImage.enabled = false;

        // �Է� ���� �⺻�� ��: Overlay Image���� ��������Ʈ ����(�ܻ�) ��õ
        // (��������Ʈ ���� ��Ʈ �׽�Ʈ�� ����ϸ� ���� �κ��� ����� �� �־��)
    }

    void OnDestroy()
    {
        tweenClose?.Kill();
        tweenOpen?.Kill();
        if (mat) Destroy(mat);
    }

    void OnRectTransformDimensionsChange()
    {
        if (overlayImage && mat) UpdateRectSize();
    }

    void ApplyStaticParams()
    {
        UpdateRectSize();
        mat.SetVector(_centerId, new Vector4(center01.x, center01.y, 0, 0));
        mat.SetFloat(_featherId, feather);
        mat.SetColor(_colorId, overlayColor);
        mat.SetFloat(_radiusId, startRadius);
    }

    void UpdateRectSize()
    {
        var rt = overlayImage.rectTransform;
        var size = rt.rect.size; // px
        mat.SetVector(_rectId, new Vector4(size.x, size.y, 0, 0));
    }

    void ToggleAtDark()
    {
        foreach (var go in disableOnDark)
            if (go) go.SetActive(false);

        foreach (var go in enableOnDark)
            if (go) go.SetActive(true);
    }

    // ---------- �Է� ���� ----------
    void BeginBlockInput()
    {
        // ������(Ŭ��/��ġ) ����: �������̰� ����ĳ��Ʈ�� ����ϰ�
        if (blockPointerDuringTransition)
        {
            _prevRaycastTarget = overlayImage.raycastTarget;
            overlayImage.raycastTarget = true; // ��ü Rect�� Ŭ�� ���
        }

        // Ű����/�е� �׺���̼� ����
        if (blockNavigationDuringTransition)
        {
            var es = eventSystem != null ? eventSystem : EventSystem.current;
            if (es != null)
            {
                _prevSendNavigationEvents = es.sendNavigationEvents;
                es.sendNavigationEvents = false; // Submit/Cancel/Move �� ����
            }
        }
    }

    void EndBlockInput()
    {
        if (blockPointerDuringTransition)
        {
            overlayImage.raycastTarget = _prevRaycastTarget;
        }
        if (blockNavigationDuringTransition)
        {
            var es = eventSystem != null ? eventSystem : EventSystem.current;
            if (es != null) es.sendNavigationEvents = _prevSendNavigationEvents;
        }
    }
    // ------------------------------

    /// <summary>
    /// ���� Ʈ������ ���. ���� �� onFinished(����)�� ȣ��.
    /// </summary>
    public void Play(Action onFinished = null)
    {
        if (!overlayImage || !mat) return;

        tweenClose?.Kill();
        tweenOpen?.Kill();

        UpdateRectSize();
        overlayImage.enabled = true;

        // �� �Է� ���� ����
        BeginBlockInput();

        float r = startRadius;
        mat.SetFloat(_radiusId, r);

        // 1) ���� (������ ���� �� ������ ��)
        tweenClose = DOTween.To(() => r, v =>
        {
            r = v;
            mat.SetFloat(_radiusId, r);
        }, endRadius, closeDuration)
        .SetEase(Ease.OutCubic)
        .SetUpdate(true) // unscaled
        .OnComplete(() =>
        {
            // ��ο� Ÿ�ֿ̹� �ʿ� UI ���
            ToggleAtDark();

            // 2) ��� ���� �� ����
            DOVirtual.DelayedCall(darkHold, () =>
            {
                tweenOpen = DOTween.To(() => r, v =>
                {
                    r = v;
                    mat.SetFloat(_radiusId, r);
                }, startRadius, openDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    overlayImage.enabled = false;

                    // �� �Է� ���� ����
                    EndBlockInput();

                    onFinished?.Invoke(); // Ʈ������ ���� ��
                });
            }, true); // true = unscaled
        });
    }

    // --- ���� API ---
    public void SetCenter01(Vector2 rect01)
    {
        center01 = rect01;
        if (mat) mat.SetVector(_centerId, new Vector4(center01.x, center01.y, 0, 0));
    }

    public void SetDarkToggleTargets(IEnumerable<GameObject> toEnable, IEnumerable<GameObject> toDisable)
    {
        enableOnDark = toEnable != null ? new List<GameObject>(toEnable) : new List<GameObject>();
        disableOnDark = toDisable != null ? new List<GameObject>(toDisable) : new List<GameObject>();
    }
}
