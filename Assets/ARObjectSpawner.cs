using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;          // Объект, который нужно включить
    [SerializeField] private GameObject textPanel;                 // Панель с текстом
    [SerializeField] private TextMeshProUGUI statusText;           // Текст статуса

    private ARTrackedImageManager trackedImageManager;             // ARTrackedImageManager
    private bool isObjectActivated = false;                       // Проверка, был ли объект уже активирован
    private CanvasGroup textPanelCanvasGroup;                     // CanvasGroup для панели текста

    void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }

        if (textPanel != null)
        {
            textPanelCanvasGroup = textPanel.GetComponent<CanvasGroup>();
            if (textPanelCanvasGroup == null)
            {
                textPanelCanvasGroup = textPanel.AddComponent<CanvasGroup>();
            }
            textPanelCanvasGroup.alpha = 0;
            textPanel.SetActive(false);
        }

        UpdateStatusText("Ищем изображение...");
    }

    void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (!isObjectActivated && trackedImage.trackingState == TrackingState.Tracking)
            {
                ActivateObjectAndText();
                UpdateStatusText("Изображение найдено! Объект и текст активированы.");
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (!isObjectActivated && trackedImage.trackingState == TrackingState.Tracking)
            {
                ActivateObjectAndText();
                UpdateStatusText("Изображение найдено! Объект и текст активированы.");
            }
        }
    }

    private void ActivateObjectAndText()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        if (textPanel != null)
        {
            textPanel.SetActive(true);
            StartCoroutine(FadeInTextPanel());
        }

        isObjectActivated = true;
    }

    private System.Collections.IEnumerator FadeInTextPanel()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textPanelCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        textPanelCanvasGroup.alpha = 1f;
    }

    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    void Update()
    {
        if (Input.touchCount == 1 && !IsAnyTouchOverUI()) // Вращение только если пользователь не взаимодействует с UI
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved && objectToActivate.activeSelf)
            {
                float rotationSpeed = 0.2f;
                objectToActivate.transform.Rotate(0, -touch.deltaPosition.x * rotationSpeed, 0, Space.World);
            }
        }
    }

    private bool IsAnyTouchOverUI()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.GetTouch(i).position
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            if (results.Count > 0)
            {
                return true; // Если хотя бы один палец взаимодействует с UI, возвращаем true
            }
        }
        return false; // Ни одно касание не взаимодействует с UI
    }
}
