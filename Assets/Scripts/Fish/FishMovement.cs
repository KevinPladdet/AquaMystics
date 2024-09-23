using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public FishStats fish;

    [Header("Fish Variables")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private bool isMoving = true;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private Vector2 targetPositionFood;

    // Food detection and moving towards it
    private GameObject nearestFood;
    private float originalSpeed;
    private bool wasTargetingFood = false;

    // Fish dies
    private bool fishAlive = true;
    private bool isUpsideDown = false;
    private bool touchedGround = false;
    [SerializeField] private bool huntingCooldown = false;

    // Hunting Fish
    [SerializeField] private bool currentlyHunting = false;
    [SerializeField] private FishMovement targetFish = null;

    [SerializeField] private float actualFishSize;

    // Fish Tank Positions (So the target position will always be inside the fish tank)
    private float minX = -7;
    private float minY = -4;
    private float maxX = 5;
    private float maxY = 4;

    private SpriteRenderer fishSpriteRenderer;
    private GameObject shopManager;

    void Start()
    {
        fishSpriteRenderer = this.GetComponent<SpriteRenderer>();
        shopManager = GameObject.Find("ShopManager");

        shopManager.GetComponent<Shop>().moneyPerSecond += fish.givesMoney;

        actualFishSize = fish.fishSize;

        // Save original speed at the start to add 0.5 when food spawns
        originalSpeed = Random.Range(fish.minSpeed, fish.maxSpeed);
        movementSpeed = originalSpeed;

        GenerateNewTarget();
        StartCoroutine(PassiveIncome());
        StartCoroutine(IncreaseFishSize());
    }

    void Update()
    {
        nearestFood = FindNearestFood();
        if (fishAlive)
        {
            // If food is found, move to nearest food
            if (nearestFood != null)
            {
                wasTargetingFood = true; // Fish is targeting food
                MoveToFood();
            }
            else if (currentlyHunting)
            {
                HuntFish();
            }
            else
            {
                // If the fish was previously targeting food, generate new position
                if (wasTargetingFood)
                {
                    wasTargetingFood = false; // Fish is no longer targeting food
                    GenerateNewTarget();
                    isMoving = true;
                }

                MoveFish();
            }
        }
        else
        {
            if (isUpsideDown)
            {
                if (!touchedGround)
                {
                    transform.position += transform.up * Time.deltaTime * 1;
                }
            } 
        }
    }

    void MoveFish()
    {
        // Removes 0.5 from the speed because the fish is no longer chasing the food
        movementSpeed = originalSpeed;

        if (isMoving)
        {
            // Move fish towards target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

            // Flip the fish sprite based on direction
            FlipFishSprite();

            // If the fish is closer than 0.1f away from the target, stop moving and wait for the next move
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                StartCoroutine(WaitForNextMove());
            }
        }
    }

    void MoveToFood()
    {
        if (nearestFood != null)
        {
            // This is for detecting if the fish is close enough to the food to eat it
            targetPositionFood = nearestFood.transform.position;

            // Makes it so that the fish will only move towards the food if fishSize is 80 or below
            if (actualFishSize <= 80)
            {
                targetPosition = nearestFood.transform.position;

                // Increase movement speed by 0.5f when moving towards food
                movementSpeed = originalSpeed + 0.5f;

                // Move fish towards nearest food
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

                FlipFishSprite();
            }
            else
            {
                MoveFish();
            }
            
            // Eat food if food is <0.7 away from fish
            if (Vector2.Distance(transform.position, targetPositionFood) < 0.7f)
            {
                Destroy(nearestFood); // Destroy the food
                nearestFood = null; // Reset food detection

                AteFood();
            }
        }
    }

    GameObject FindNearestFood()
    {
        Food[] allFood = FindObjectsOfType<Food>(); // Find all food instances
        float shortestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (Food food in allFood)
        {
            float distance = Vector2.Distance(transform.position, food.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = food.gameObject;
            }
        }

        return nearest;
    }

    void FlipFishSprite()
    {
        if (targetPosition.x > transform.position.x)
        {
            // Moving right
            fishSpriteRenderer.flipX = true;
        }
        else if (targetPosition.x < transform.position.x)
        {
            // Moving left
            fishSpriteRenderer.flipX = false;
        }
    }

    // Wait between 2 to 7 seconds for the fish to move again
    IEnumerator WaitForNextMove()
    {
        float waitTime = Random.Range(2f, 7f);
        yield return new WaitForSeconds(waitTime);
        GenerateNewTarget();
        isMoving = true;
    }

    // Generate a new target position for the fish to go to
    void GenerateNewTarget()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        targetPosition = new Vector2(randomX, randomY);

        originalSpeed = Random.Range(fish.minSpeed, fish.maxSpeed);
        movementSpeed = originalSpeed;
    }

    IEnumerator IncreaseFishSize()
    {
        yield return new WaitForSeconds(10f);
        while (true)
        {
            if (fishAlive)
            {
                actualFishSize -= 10f;

                scaleFish();

                if (actualFishSize >= 100 || actualFishSize <= 0)
                {
                    fishAlive = false;
                    shopManager.GetComponent<Shop>().moneyPerSecond -= fish.givesMoney;
                    FishDies();
                }

                if (this.name == "Damsel(Clone)" && !huntingCooldown)
                {
                    if (actualFishSize >= 70 || actualFishSize <= 30)
                    {
                        currentlyHunting = true;
                    }
                }
                
                if (this.name == "Piranha(Clone)" && !huntingCooldown)
                {
                    if (actualFishSize >= 80 || actualFishSize <= 40)
                    {
                        currentlyHunting = true;
                    }
                }
            }
            yield return new WaitForSeconds(10f);
        }
    }

    // Adds money every second
    IEnumerator PassiveIncome()
    {
        while (true)
        {
            if (fishAlive)
            {
                shopManager.GetComponent<Shop>().money += fish.givesMoney;
                shopManager.GetComponent<Shop>().totalMoneyEarned += fish.givesMoney;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void scaleFish()
    {
        switch (actualFishSize)
        {
            case 0:
                this.transform.localScale = new Vector3(0.15f, 0.15f, gameObject.transform.localScale.z);
                break;
            case 10:
                this.transform.localScale = new Vector3(0.16f, 0.16f, gameObject.transform.localScale.z);
                break;
            case 20:
                this.transform.localScale = new Vector3(0.17f, 0.17f, gameObject.transform.localScale.z);
                break;
            case 30:
                this.transform.localScale = new Vector3(0.18f, 0.18f, gameObject.transform.localScale.z);
                break;
            case 40:
                this.transform.localScale = new Vector3(0.19f, 0.19f, gameObject.transform.localScale.z);
                break;
            case 50:
                this.transform.localScale = new Vector3(0.20f, 0.20f, gameObject.transform.localScale.z);
                break;
            case 60:
                this.transform.localScale = new Vector3(0.21f, 0.21f, gameObject.transform.localScale.z);
                break;
            case 70:
                this.transform.localScale = new Vector3(0.22f, 0.22f, gameObject.transform.localScale.z);
                break;
            case 80:
                this.transform.localScale = new Vector3(0.23f, 0.23f, gameObject.transform.localScale.z);
                break;
            case 90:
                this.transform.localScale = new Vector3(0.24f, 0.24f, gameObject.transform.localScale.z);
                break;
            case 100:
                this.transform.localScale = new Vector3(0.25f, 0.25f, gameObject.transform.localScale.z);
                break;
        }

    }

    void FishDies()
    {
        StartCoroutine(RotateFishUpsideDown());
    }

    private IEnumerator RotateFishUpsideDown()
    {
        float rotationTime = 0.5f;
        float elapsedTime = 0f;
        float startRotation = transform.rotation.eulerAngles.x;
        float endRotation = startRotation - 180f;

        while (elapsedTime < rotationTime)
        {
            float t = elapsedTime / rotationTime;
            float newRotation = Mathf.Lerp(startRotation, endRotation, t);
            transform.rotation = Quaternion.Euler(0, 0, newRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Makes sure its exactly 180
        transform.rotation = Quaternion.Euler(0, 0, endRotation);

        isUpsideDown = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            touchedGround = true;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 1f;

        Color color = fishSpriteRenderer.color;
        float startAlpha = color.a;
        float fadeSpeed = startAlpha / fadeDuration;

        // Gradually fade the alpha to 0
        while (color.a > 0)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            fishSpriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }

    void AteFood()
    {
        actualFishSize += 10f;

        scaleFish();

        if (actualFishSize >= 100 || actualFishSize <= 0)
        {
            fishAlive = false;
            shopManager.GetComponent<Shop>().moneyPerSecond -= fish.givesMoney;
            FishDies();
        }
        else
        {
            if (this.name == "Damsel(Clone)" && !huntingCooldown)
            {
                if (actualFishSize >= 70 || actualFishSize <= 30)
                {
                    currentlyHunting = true;
                }
            }
            else if (this.name == "Piranha(Clone)" && !huntingCooldown)
            {
                if (actualFishSize >= 80 || actualFishSize <= 40)
                {
                    currentlyHunting = true;
                }
            }
            else
            {
                // Once food is eaten, the fish will switch to MoveFish()
                wasTargetingFood = false;
                GenerateNewTarget();
                isMoving = true;
            }
        }
    }

    void HuntFish()
    {
        // Makes sure FindTargetFish() only runs once
        if (targetFish == null)
        {
            FindTargetFish();
        }

        // No fish available to hunt, so stop hunting
        if (targetFish == null)
        {
            currentlyHunting = false;
            GenerateNewTarget();
            isMoving = true;
            return;
        }

        // Move towards the target fish
        targetPosition = targetFish.transform.position;
        movementSpeed = originalSpeed + 1f; // Adds 1f speed when hunting
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        FlipFishSprite();

        // If hunter is close enough, eat fish
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            shopManager.GetComponent<Shop>().moneyPerSecond -= targetFish.fish.givesMoney;
            
            Destroy(targetFish.gameObject); // Destroy fish that got eaten

            // This entire part is for feeding because the fish got eaten
            actualFishSize += 10f;
            scaleFish();

            if (actualFishSize >= 100 || actualFishSize <= 0)
            {
                fishAlive = false;
                shopManager.GetComponent<Shop>().moneyPerSecond -= fish.givesMoney;
                FishDies();
            }

            // Stop hunting and set targetFish to empty
            huntingCooldown = true;
            StartCoroutine(HuntingCooldown());
            targetFish = null;
            currentlyHunting = false;
            GenerateNewTarget();
            isMoving = true;
        }
    }

    void FindTargetFish()
    {
        GameObject fishHolder = GameObject.Find("FishHolder");

        // Get all fish in FishHolder except for the current fish and other enemies
        List<FishMovement> allFish = new List<FishMovement>();

        foreach (Transform fish in fishHolder.transform)
        {
            FishMovement fishScript = fish.GetComponent<FishMovement>();

            // Ignore itself and other enemies
            if (fishScript != null && fishScript != this && fish.name != "Damsel(Clone)" && fish.name != "Piranha(Clone)")
            {
                allFish.Add(fishScript);
            }
        }

        // No fish available to hunt, so stop hunting
        if (allFish.Count == 0)
        {
            currentlyHunting = false;
            GenerateNewTarget();
            isMoving = true;
            return;
        }

        // Picks a random fish to hunt
        targetFish = allFish[Random.Range(0, allFish.Count)];
    }

    IEnumerator HuntingCooldown()
    {
        if (huntingCooldown)
        {
            yield return new WaitForSeconds(25f);
            huntingCooldown = false;
        }
    }
}