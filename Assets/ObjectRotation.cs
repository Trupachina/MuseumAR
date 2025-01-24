using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectRotation : MonoBehaviour
{
    public RawImage rawImage; // RawImage для отображения объекта
    public float fadeDuration = 1.0f; // Длительность анимации
    public float rotationSpeed = 0.2f; // Скорость вращения
    public float scaleSpeed = 0.01f; // Скорость масштабирования
    public float minScale = 0.5f; // Минимальный масштаб объекта
    public float maxScale = 3.0f; // Максимальный масштаб объекта
    public float minRotationY = -45.0f; // Минимальный угол вращения
    public float maxRotationY = 45.0f; // Максимальный угол вращения

    private bool isFadingIn = false; // Флаг анимации появления

    void Start()
    {
        if (rawImage != null)
        {
            Color color = rawImage.color;
            color.a = 0; // Устанавливаем начальную прозрачность
            rawImage.color = color;

            StartCoroutine(FadeIn()); // Запускаем анимацию появления
        }
    }

    void Update()
    {
        if (Input.touchCount == 1) // Вращение
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float deltaRotation = -touch.deltaPosition.x * rotationSpeed;
                RotateObject(deltaRotation);
            }
        }
        else if (Input.touchCount == 2) // Масштабирование
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

    // Вращение объекта с ограничением угла
    private void RotateObject(float deltaRotation)
    {
        Vector3 currentRotation = transform.eulerAngles;
        float newYRotation = currentRotation.y + deltaRotation;

        // Ограничение вращения
        if (newYRotation > 180) newYRotation -= 360;
        newYRotation = Mathf.Clamp(newYRotation, minRotationY, maxRotationY);

        transform.eulerAngles = new Vector3(currentRotation.x, newYRotation, currentRotation.z);
    }

    // Масштабирование объекта с ограничением
    private void ScaleObject(float deltaScale)
    {
        float currentScale = transform.localScale.x; // Предполагаем, что объект равномерно масштабируется
        float newScale = Mathf.Clamp(currentScale + deltaScale, minScale, maxScale);

        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    // Анимация проявления RawImage
    private IEnumerator FadeIn()
    {
        isFadingIn = true;

        float elapsedTime = 0.0f;
        Color color = rawImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration); // Линейная интерполяция прозрачности
            rawImage.color = color;
            yield return null;
        }

        color.a = 1; // Устанавливаем конечное значение
        rawImage.color = color;
        isFadingIn = false;
    }
}
