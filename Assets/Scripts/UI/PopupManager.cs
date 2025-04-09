using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
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
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(ClosePopup);
    }
}