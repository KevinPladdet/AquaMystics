using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CleaningTank : MonoBehaviour
{

    [SerializeField] private GameObject shopManager;
    [SerializeField] private GameObject foodHolder;
    [SerializeField] private GameObject fishHolder;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject canvas;

    [Header("Cleaning References")]
    [SerializeField] private TextMeshProUGUI cleaningCostsText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button cleaningButton;

    [Header("Timer References")]
    [SerializeField] private float gameTime;
    [SerializeField] private float remainingTime; // Tracks remaining time for the timer
    [SerializeField] private bool stopTimer = false;

    [Header("Game Over Menu References")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private TextMeshProUGUI totalMoneyEarnedText;
    [SerializeField] private TextMeshProUGUI totalMoneySpentText;
    [SerializeField] private TextMeshProUGUI permanentCurrencyEarnedText;

    [Header("Button References")]
    [SerializeField] private GameObject[] allButtons;
    [SerializeField] private Button fishShopButton;
    [SerializeField] private Button foodShopButton;
    [SerializeField] private GameObject fishShop;
    [SerializeField] private GameObject foodShop;
    [SerializeField] private Material selectedMat;
    [SerializeField] private Material unselectedMat;

    private float cleaningCostAmount;

    private float totalTritonTokensEarned;

    private bool alreadyPaid = false;

    private bool onlyOnce = false;

    void Start()
    {
        remainingTime = gameTime; // Sets timer to gameTime (120s)
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;

        cleaningCostAmount = 200;
        cleaningCostsText.text = "Cleaning Costs: " + cleaningCostAmount + "$";
    }

    void Update()
    {
        if (!stopTimer)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                if (!alreadyPaid)
                {
                    remainingTime = 0;

                    if (!onlyOnce)
                    {
                        stopTimer = true;
                        GameOver();
                        onlyOnce = true;
                    }
                }
                else
                {
                    ContinueTimer();
                }
            }

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);
            timerText.text = textTime;
            timerSlider.value = remainingTime;
        }

        if (!shopManager.GetComponent<Shop>().gameOverActivated)
        {
            if (cleaningCostAmount >= shopManager.GetComponent<Shop>().money)
            {
                cleaningButton.interactable = false;
            }
            else if (!alreadyPaid)
            {
                cleaningButton.interactable = true;
            }
        }
    }

    public void CleanTank()
    {
        alreadyPaid = true;
        cleaningButton.interactable = false;

        // Reducts cleaningCostAmount from money and increases cleaningCostAmount * 8
        shopManager.GetComponent<Shop>().money -= cleaningCostAmount;
        cleaningCostAmount = cleaningCostAmount * 8;
        cleaningCostsText.text = "Next cleaning is due soon";
    }

    void ContinueTimer()
    {
        cleaningCostsText.text = "Cleaning Costs: " + cleaningCostAmount + "$";

        // Reset remaining time to 120s
        remainingTime = gameTime;
        timerSlider.value = remainingTime;

        alreadyPaid = false;
    }

    public void GameOver()
    {
        shopManager.GetComponent<Shop>().gameOverActivated = true;

        SetButtons();

        totalTritonTokensEarned = shopManager.GetComponent<Shop>().money / 100;

        Time.timeScale = 0f;
        gameOverMenu.SetActive(true);

        if (foodHolder.GetComponent<SpawnFood>().activateGameOver)
        {
            reasonText.text = "You do not have enough money to buy another fish";
        }
        else
        {
            reasonText.text = "You did not clean the fish tank";
        }

        totalMoneyEarnedText.text = "" + shopManager.GetComponent<Shop>().totalMoneyEarned.ToString("F2") + "$";
        totalMoneySpentText.text = "" + shopManager.GetComponent<Shop>().totalMoneySpent.ToString("F2") + "$";

        shopManager.GetComponent<PermanentShop>().tritonTokensAmount += totalTritonTokensEarned;
        permanentCurrencyEarnedText.text = "" + totalTritonTokensEarned.ToString("F2") + "⎊";

        // Save tritonTokensAmount to PlayerPrefs
        PlayerPrefs.SetFloat("tritonTokens", shopManager.GetComponent<PermanentShop>().tritonTokensAmount);
    }

    public void ReturnToMainMenu()
    {
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(true);

        ResetGame();
    }

    public void ResetGame()
    {
        foreach (Transform child in foodHolder.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in fishHolder.transform)
        {
            Destroy(child.gameObject);
        }

        remainingTime = gameTime;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;

        stopTimer = false;
        onlyOnce = false;

        shopManager.GetComponent<Shop>().moneyPerSecond = 0;
        shopManager.GetComponent<Shop>().totalMoneySpent = 0;
        shopManager.GetComponent<Shop>().totalMoneyEarned = 0;
        shopManager.GetComponent<Shop>().money = 10;

        cleaningCostAmount = 200;
        cleaningCostsText.text = "Cleaning Costs: " + cleaningCostAmount + "$";

        shopManager.GetComponent<Shop>().totalMoneyEarned = shopManager.GetComponent<Shop>().totalMoneyEarned + shopManager.GetComponent<Shop>().money;
    }

    public void PlayGame()
    {
        canvas.GetComponent<PauseMenu>().canActivate = true;
        shopManager.GetComponent<Shop>().gameOverActivated = false;
        SetButtons();
        mainMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    void SetButtons()
    {
        if (shopManager.GetComponent<Shop>().gameOverActivated)
        {
            foreach (GameObject button in allButtons)
            {
                button.GetComponent<Button>().interactable = false;
            }

            fishShopButton.interactable = false;
            foodShopButton.interactable = false;
        }
        else
        {
            fishShopButton.interactable = true;
            foodShopButton.interactable = true;

            fishShopButton.GetComponent<Image>().material = selectedMat;
            foodShopButton.GetComponent<Image>().material = unselectedMat;

            fishShop.SetActive(true);
            foodShop.SetActive(false);
        }
    }
}
