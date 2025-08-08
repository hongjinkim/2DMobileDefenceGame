using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReferenceImageManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string imageFolderPath = "Assets/ReferenceImages";
    [SerializeField] private RawImage targetRawImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Image List")]
    [SerializeField] private List<Texture2D> loadedImages = new List<Texture2D>();
    [SerializeField] private List<string> imageNames = new List<string>();
    [SerializeField] private List<string> assetPaths = new List<string>();
    [SerializeField] private int currentImageIndex = 0;

    [Header("Controls")]
    [SerializeField] private bool showReference = true;
    [SerializeField, Range(0f, 1f)] private float referenceAlpha = 0.5f;

    private void Start()
    {
        InitializeComponents();
        LoadImagesFromFolder();
        UpdateDisplay();
    }

    private void OnValidate()
    {
        // �����Ϳ��� ���� ����� ������ ȣ��
        if (!Application.isPlaying)
        {
            InitializeComponents();
            UpdateDisplay();
        }
    }

    private void InitializeComponents()
    {
        if (targetRawImage == null)
            targetRawImage = GetComponent<RawImage>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void LoadImagesFromFolder()
    {
        loadedImages.Clear();
        imageNames.Clear();
        assetPaths.Clear();

#if UNITY_EDITOR
        // �����Ϳ����� AssetDatabase ���
        if (!AssetDatabase.IsValidFolder(imageFolderPath))
        {
            Debug.LogWarning($"Reference image folder not found: {imageFolderPath}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new string[] { imageFolderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture != null)
            {
                loadedImages.Add(texture);
                imageNames.Add(texture.name);
                assetPaths.Add(assetPath);
            }
        }

        Debug.Log($"Loaded {loadedImages.Count} reference images from {imageFolderPath}");

        // �����Ϳ��� ������� ����
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
#else
        // ����� ���ӿ����� Resources ���� ����� ����
        Debug.LogWarning("Image loading from Assets folder is only available in Editor. Consider using Resources folder for runtime loading.");
#endif
    }

    public void SetCurrentImage(int index)
    {
        if (index >= 0 && index < loadedImages.Count)
        {
            currentImageIndex = index;
            UpdateDisplay();

#if UNITY_EDITOR
            // �����Ϳ��� ��������� ����
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }

    public void NextImage()
    {
        if (loadedImages.Count > 0)
        {
            currentImageIndex = (currentImageIndex + 1) % loadedImages.Count;
            UpdateDisplay();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }

    public void PreviousImage()
    {
        if (loadedImages.Count > 0)
        {
            currentImageIndex = (currentImageIndex - 1 + loadedImages.Count) % loadedImages.Count;
            UpdateDisplay();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }

    public void ToggleVisibility()
    {
        showReference = !showReference;
        UpdateDisplay();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    public void SetAlpha(float alpha)
    {
        referenceAlpha = Mathf.Clamp01(alpha);
        UpdateDisplay();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    public float GetAlpha()
    {
        return referenceAlpha;
    }

    private void UpdateDisplay()
    {
        if (targetRawImage != null && loadedImages.Count > 0 && currentImageIndex < loadedImages.Count)
        {
            targetRawImage.texture = loadedImages[currentImageIndex];
            targetRawImage.gameObject.SetActive(showReference);
        }
        else if (targetRawImage != null)
        {
            targetRawImage.gameObject.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = showReference ? referenceAlpha : 0f;
            canvasGroup.interactable = showReference;
        }

#if UNITY_EDITOR
        // �����Ϳ��� Scene �� ������Ʈ
        if (!Application.isPlaying)
        {
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }

    public string GetCurrentImagePath()
    {
        if (currentImageIndex >= 0 && currentImageIndex < assetPaths.Count)
            return assetPaths[currentImageIndex];
        return "";
    }

    public string GetCurrentImageName()
    {
        if (currentImageIndex >= 0 && currentImageIndex < imageNames.Count)
            return imageNames[currentImageIndex];
        return "No Image";
    }

    public int GetImageCount()
    {
        return loadedImages.Count;
    }

    private void OnDestroy()
    {
        // �����Ϳ��� �ε��� ������ ������ ������ �ʿ� ����
        loadedImages.Clear();
        imageNames.Clear();
        assetPaths.Clear();
    }

    public void SetVisibility(bool visible)
    {
        showReference = visible;
        UpdateDisplay();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}