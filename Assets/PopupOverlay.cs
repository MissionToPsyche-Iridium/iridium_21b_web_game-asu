using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupOverlay : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Image pictureArea;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button closeButton;

    void Start()
    {
        // Initially hide the popup
        popupPanel.SetActive(false);
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup(Sprite image, string description)
    {
        // Set the image
        pictureArea.sprite = image;

        // Set the description text
        descriptionText.text = description;

        // Show the popup
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
