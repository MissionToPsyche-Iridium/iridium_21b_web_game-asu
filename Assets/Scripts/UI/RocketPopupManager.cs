using UnityEngine;
using UnityEngine.UI;

public class RocketPopupManager : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] public Image leftImage;
    [SerializeField] private Text textArea;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
        popupPanel.SetActive(false);
    }

    public void ShowPopup(string message, Sprite image = null)
    {
        Time.timeScale = 0f;
        popupPanel.SetActive(true);
        textArea.text = message;
        if (image != null)
        {
            leftImage.sprite = image;
        }
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(ClosePopup);
    }
}
