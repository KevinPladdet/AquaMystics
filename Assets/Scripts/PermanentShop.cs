using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PermanentShop : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI tritonText;
    [SerializeField] private TextMeshProUGUI cantAffordText;
    public float tritonTokensAmount;

    // Background Changer
    [SerializeField] private TMP_Dropdown bgChangerDropdown;  // Reference to the Dropdown UI element
    [SerializeField] private SpriteRenderer backgroundImage;  // Reference to the Image component (or SpriteRenderer for 2D games)

    [SerializeField] private GameObject bgChangerObject;
    [SerializeField] private GameObject bgChangerButton;

    // Background Sprites
    [SerializeField] private Sprite[] backgroundSprites;

    void Start()
    {
        // Ensure the dropdown's value change event is connected to the method
        bgChangerDropdown.onValueChanged.AddListener(delegate { ChangeBackground(bgChangerDropdown.value); });

        // Optionally, set the initial background image based on the current dropdown value
        ChangeBackground(bgChangerDropdown.value);
    }

    public void ResetTritonTokens()
    {
        PlayerPrefs.SetFloat("tritonTokens", 0f);
        tritonTokensAmount = PlayerPrefs.GetFloat("tritonTokens");
        tritonText.text = "Triton Tokens: " + tritonTokensAmount.ToString("F2") + "⎊";
    }

    public void UpdateTritonTokens()
    {
        tritonTokensAmount = PlayerPrefs.GetFloat("tritonTokens");
        tritonText.text = "Triton Tokens: " + tritonTokensAmount.ToString("F2") + "⎊";
    }

    public void BuyBackgroundChanger()
    {
        if (tritonTokensAmount >= 100)
        {
            tritonTokensAmount -= 100;
            PlayerPrefs.SetFloat("tritonTokens", tritonTokensAmount);
            tritonText.text = "Triton Tokens: " + tritonTokensAmount.ToString("F2") + "⎊";
            bgChangerButton.SetActive(false);
            bgChangerObject.SetActive(true); // MAKE IT SO THAT BACKGROUND IN PERMANENT SHOP UPDATES AS WELL ON FRIDAY
        }
        else
        {
            // Set alpha to 255
            Color color = cantAffordText.color;
            color.a = 1f;
            cantAffordText.color = color;

            // Start the fade-out coroutine
            StartCoroutine(FadeWarningText());
        }
    }

    public void ChangeBackground(int selectedOption)
    {
        // Check if the selected option index is valid and within range of the sprites array
        if (selectedOption >= 0 && selectedOption < backgroundSprites.Length)
        {
            // Change the background image's sprite
            backgroundImage.sprite = backgroundSprites[selectedOption];
        }
        else
        {
            Debug.LogError("Invalid dropdown option or sprite not assigned!");
        }
    }

    private IEnumerator FadeWarningText()
    {
        float duration = 3f;
        float elapsedTime = 0f;

        Color color = cantAffordText.color;

        // Fade alpha from 255 to 0 over 3 seconds
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            cantAffordText.color = color;
            yield return null;
        }
    }
}
