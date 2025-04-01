using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupOverlay : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel; 
    [SerializeField] private RectTransform popupRectTransform;
    [SerializeField] private Image pictureArea;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button closeButton;

    void Start()
    {
        // Initially hide the popup
        popupPanel.SetActive(false);
        closeButton.onClick.AddListener(ClosePopup);

        if (popupRectTransform != null)
        {
            popupRectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                Screen.width * 0.5f
            );
            popupRectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                Screen.height * 0.5f
            );
        }
    }

    public void ShowPopup(Sprite image, string description)
    {
        // Set the image
        pictureArea.sprite = image;

        // Set the description text
        descriptionText.text = description;

        // Show the popup
        popupPanel.SetActive(true);
        Time.timeScale = 0f; // pausing the game
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
