using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnFood : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject foodHolder;
    [SerializeField] private Rect insideFishTank; // Area of the fish tank

    [Header("References")]
    [SerializeField] private GameObject shopManager;
    [SerializeField] private TextMeshProUGUI foodWarningText;

    //public bool canSpawnFood;
    public bool activateGameOver;

    private Coroutine warningFadeCoroutine;

    private void Start()
    {
        StartCoroutine(CheckIfBroke());
    }

    private void Update()
    {
        if (Time.timeScale == 1f)
        {
            if (Input.GetMouseButtonDown(0)) // This code part could be a lot more efficient, but I want to just get it to work
            {
                if (shopManager.GetComponent<Shop>().money >= 0.50 && shopManager.GetComponent<Shop>().moneyPerSecond >= 0.50)
                {
                    Vector3 mousePos = Input.mousePosition;

                    if (IsWithinFishTank(mousePos))
                    {
                        mousePos.z = 2.0f;

                        Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos);

                        shopManager.GetComponent<Shop>().money -= 0.50f;
                        shopManager.GetComponent<Shop>().totalMoneySpent += 0.50f;

                        Instantiate(foodPrefab, objectPos, Quaternion.identity, foodHolder.transform);
                    }
                }
                else if (shopManager.GetComponent<Shop>().money >= 0.50 && shopManager.GetComponent<Shop>().moneyPerSecond == 0.00)
                {
                    Vector3 mousePos = Input.mousePosition;

                    if (IsWithinFishTank(mousePos))
                    {
                        // Stops coroutine if it is already running
                        if (warningFadeCoroutine != null)
                        {
                            StopCoroutine(warningFadeCoroutine);
                        }

                        foodWarningText.text = "You must buy a fish before buying food";

                        // Set alpha to 255
                        Color color = foodWarningText.color;
                        color.a = 1f;
                        foodWarningText.color = color;

                        // Start the fade-out coroutine
                        warningFadeCoroutine = StartCoroutine(FadeWarningText());
                    }
                }
                else
                {
                    Vector3 mousePos = Input.mousePosition;

                    if (IsWithinFishTank(mousePos))
                    {
                        // Stops coroutine if it is already running
                        if (warningFadeCoroutine != null)
                        {
                            StopCoroutine(warningFadeCoroutine);
                        }

                        foodWarningText.text = "You do not have enough money for food";

                        // Set alpha to 255
                        Color color = foodWarningText.color;
                        color.a = 1f;
                        foodWarningText.color = color;

                        // Start the fade-out coroutine
                        warningFadeCoroutine = StartCoroutine(FadeWarningText());
                    }
                }
            }
        }
    }

    IEnumerator CheckIfBroke()
    {
        while (true)
        {
            if (shopManager.GetComponent<Shop>().money <= 9.50 && shopManager.GetComponent<Shop>().moneyPerSecond == 0.00)
            {
                activateGameOver = true;
                shopManager.GetComponent<CleaningTank>().GameOver();
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator FadeWarningText()
    {
        float duration = 3f;
        float elapsedTime = 0f;

        Color color = foodWarningText.color;

        // Fade alpha from 255 to 0 over 3 seconds
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            foodWarningText.color = color;
            yield return null;
        }
    }

    // Checks if mousePos is inside of the fish tank
    private bool IsWithinFishTank(Vector3 mousePos)
    {
        return insideFishTank.Contains(mousePos);
    }
}