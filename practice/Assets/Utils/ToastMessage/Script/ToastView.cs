using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ToastView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rect;
    //[SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    [Header("Style")]
    [SerializeField] private float fadeIn = 0.22f;
    [SerializeField] private float fadeOut = 0.22f;
    [SerializeField] private float yInOffset = 22f;   // ���� �� ��/�Ʒ� ��¦ �̵�
    [SerializeField] private float yOutOffset = 16f;  // ���� �� ���� ��¦
    [SerializeField] private float cornerRadius = 16f; // (9-slice ��������Ʈ ��� ����)

    Sequence playingSeq;

    void Reset()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        //background = GetComponent<Image>();
        label = GetComponentInChildren<TMP_Text>(true);
    }

    void Awake()
    {
        if (!rect) rect = GetComponent<RectTransform>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        //if (background) background.raycastTarget = false;
        if (label) label.raycastTarget = false;
    }

    public void SetText(string message) => label.text = message;

    public void SetColors(Color bg, Color text)
    {
        //if (background) background.color = bg;
        if (label) label.color = text;
    }

    public float Play(float holdSeconds, Action onFinished)
    {
        if (playingSeq != null) playingSeq.Kill();
        canvasGroup.alpha = 0f;

        // ���� ��ġ: ��¦ �Ʒ���(�Ǵ� ����)���� ����
        var startPos = rect.anchoredPosition;
        rect.anchoredPosition = startPos + new Vector2(0, -Mathf.Sign(yInOffset) * Mathf.Abs(yInOffset));

        playingSeq = DOTween.Sequence()
            // ����
            .Append(rect.DOAnchorPos(startPos, fadeIn).SetEase(Ease.OutCubic))
            .Join(canvasGroup.DOFade(1f, fadeIn).SetEase(Ease.OutCubic))
            // ����
            .AppendInterval(holdSeconds)
            // ����
            .Append(rect.DOAnchorPos(startPos + new Vector2(0, yOutOffset), fadeOut).SetEase(Ease.InCubic))
            .Join(canvasGroup.DOFade(0f, fadeOut).SetEase(Ease.InCubic))
            .SetUpdate(true)
            .OnComplete(() =>
            {
                onFinished?.Invoke();
            });

        return fadeIn + holdSeconds + fadeOut;
    }

    public void HideImmediate()
    {
        playingSeq?.Kill();
        canvasGroup.alpha = 0f;
    }
}
