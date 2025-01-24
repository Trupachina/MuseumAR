using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;          // ������, ������� ����� ��������
    [SerializeField] private GameObject textPanel;                 // ������ � �������
    [SerializeField] private TextMeshProUGUI statusText;           // ����� �������

    private ARTrackedImageManager trackedImageManager;             // ARTrackedImageManager
    private bool isObjectActivated = false;                       // ��������, ��� �� ������ ��� �����������
    private CanvasGroup textPanelCanvasGroup;                     // CanvasGroup ��� ������ ������

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

        UpdateStatusText("���� �����������...");
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
                UpdateStatusText("����������� �������! ������ � ����� ������������.");
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (!isObjectActivated && trackedImage.trackingState == TrackingState.Tracking)
            {
                ActivateObjectAndText();
                UpdateStatusText("����������� �������! ������ � ����� ������������.");
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
        if (Input.touchCount == 1 && !IsAnyTouchOverUI()) // �������� ������ ���� ������������ �� ��������������� � UI
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
                return true; // ���� ���� �� ���� ����� ��������������� � UI, ���������� true
            }
        }
        return false; // �� ���� ������� �� ��������������� � UI
    }
}
