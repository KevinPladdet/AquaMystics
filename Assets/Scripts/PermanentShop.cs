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
    [SerializeField] private TMP_Dropdown bgChangerDropdown;
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private Image permaShopBGImage;

    [SerializeField] private GameObject bgChangerObject;
    [SerializeField] private GameObject bgChangerButton;

    // Background Sprites
    [SerializeField] private Sprite[] backgroundSprites;

    void Start()
    {
        bgChangerDropdown.onValueChanged.AddListener(delegate { ChangeBackground(bgChangerDropdown.value); });

        ChangeBackground(bgChangerDropdown.value);
    }

    private void Update()
    {
        if (tritonTokensAmount >= 100)
        {
            bgChangerButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            bgChangerButton.GetComponent<Button>().interactable = false;
        }
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
            bgChangerObject.SetActive(true);
        }
    }

    public void ChangeBackground(int selectedOption)
    {
        if (selectedOption >= 0 && selectedOption < backgroundSprites.Length)
        {
            backgroundImage.sprite = backgroundSprites[selectedOption];
            permaShopBGImage.sprite = backgroundSprites[selectedOption];
        }
        else
        {
            Debug.LogError("Dropdown did not work!");
        }
    }
}
