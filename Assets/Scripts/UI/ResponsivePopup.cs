using UnityEngine;

public class ResponsivePopup : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private float maxWidth = 1000f;
    [SerializeField] private float maxHeight = 500f;
    [SerializeField] private float minWidth = 600f;
    [SerializeField] private float minHeight = 300f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float width = Mathf.Clamp(screenWidth * 0.8f, minWidth, maxWidth);
        float height = Mathf.Clamp(screenHeight * 0.8f, minHeight, maxHeight);

        rectTransform.sizeDelta = new Vector2(width, height);
    }
}