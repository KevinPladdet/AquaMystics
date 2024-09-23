using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject fishHolder;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI moneyPerSecondText;

    [Header("Shop Buttons & Prices")]
    [SerializeField] private List<Button> fishButtons;
    [SerializeField] private List<float> fishCosts;

    [Header("Currencies")]
    public float money;
    public float moneyPerSecond;
    // These two below are for the game over menu
    public float totalMoneyEarned;
    public float totalMoneySpent;

    public bool gameOverActivated;

    private void Update()
    {
        moneyText.text = "Money: " + money.ToString("F2") + "$";
        moneyPerSecondText.text = "Per Second: " + moneyPerSecond.ToString("F2") + "$";

        // Puts fish shop buttons on interactable if you have enough money to buy it
        UpdateButtonInteractable();
    }

    // Fish Tank Positions (So the fish will always spawn inside the fish tank)
    private float minX = -7;
    private float minY = -4;
    private float maxX = 5;
    private float maxY = 4;

    public void SpawnFish(GameObject fishPrefab)
    {
        float fishCost;
        
        if (fishPrefab.name == "Damsel")
        {
            fishCost = fishPrefab.GetComponent<FishMovement>().fish.givesMoney * 10;
            totalMoneySpent += fishPrefab.GetComponent<FishMovement>().fish.givesMoney * 10;
        }
        else if (fishPrefab.name == "Piranha")
        {
            fishCost = fishPrefab.GetComponent<FishMovement>().fish.givesMoney * 8;
            totalMoneySpent += fishPrefab.GetComponent<FishMovement>().fish.givesMoney * 8;
        }
        else
        {
            fishCost = fishPrefab.GetComponent<FishMovement>().fish.givesMoney * 20;
            totalMoneySpent += fishPrefab.GetComponent<FishMovement>().fish.givesMoney * 20;
        }
            
        money -= fishCost;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        Vector2 spawnPos = new Vector2(randomX, randomY);
        Instantiate(fishPrefab, spawnPos, Quaternion.identity, fishHolder.transform);
    }

    private void UpdateButtonInteractable()
    {
        if (!gameOverActivated)
        {
            for (int i = 0; i < fishButtons.Count; i++)
            {
                // Checks if you have enough money to buy the fish
                if (money >= fishCosts[i])
                {
                    fishButtons[i].interactable = true;
                }
                else
                {
                    fishButtons[i].interactable = false;
                }
            }
        }
    }
}
