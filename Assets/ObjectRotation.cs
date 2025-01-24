using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectRotation : MonoBehaviour
{
    public RawImage rawImage; // RawImage ��� ����������� �������
    public float fadeDuration = 1.0f; // ������������ ��������
    public float rotationSpeed = 0.2f; // �������� ��������
    public float scaleSpeed = 0.01f; // �������� ���������������
    public float minScale = 0.5f; // ����������� ������� �������
    public float maxScale = 3.0f; // ������������ ������� �������
    public float minRotationY = -45.0f; // ����������� ���� ��������
    public float maxRotationY = 45.0f; // ������������ ���� ��������

    private bool isFadingIn = false; // ���� �������� ���������

    void Start()
    {
        if (rawImage != null)
        {
            Color color = rawImage.color;
            color.a = 0; // ������������� ��������� ������������
            rawImage.color = color;

            StartCoroutine(FadeIn()); // ��������� �������� ���������
        }
    }

    void Update()
    {
        if (Input.touchCount == 1) // ��������
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float deltaRotation = -touch.deltaPosition.x * rotationSpeed;
                RotateObject(deltaRotation);
            }
        }
        else if (Input.touchCount == 2) // ���������������
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDelta = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentTouchDelta = (touch0.position - touch1.position).magnitude;

            float deltaMagnitude = currentTouchDelta - prevTouchDelta;
            ScaleObject(deltaMagnitude * scaleSpeed);
        }
    }

    // �������� ������� � ������������ ����
    private void RotateObject(float deltaRotation)
    {
        Vector3 currentRotation = transform.eulerAngles;
        float newYRotation = currentRotation.y + deltaRotation;

        // ����������� ��������
        if (newYRotation > 180) newYRotation -= 360;
        newYRotation = Mathf.Clamp(newYRotation, minRotationY, maxRotationY);

        transform.eulerAngles = new Vector3(currentRotation.x, newYRotation, currentRotation.z);
    }

    // ��������������� ������� � ������������
    private void ScaleObject(float deltaScale)
    {
        float currentScale = transform.localScale.x; // ������������, ��� ������ ���������� ��������������
        float newScale = Mathf.Clamp(currentScale + deltaScale, minScale, maxScale);

        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    // �������� ���������� RawImage
    private IEnumerator FadeIn()
    {
        isFadingIn = true;

        float elapsedTime = 0.0f;
        Color color = rawImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration); // �������� ������������ ������������
            rawImage.color = color;
            yield return null;
        }

        color.a = 1; // ������������� �������� ��������
        rawImage.color = color;
        isFadingIn = false;
    }
}
