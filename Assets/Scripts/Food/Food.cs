using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    private bool touchedGround = false;
    private SpriteRenderer spriteRenderer;
    private float fadeDuration = 2f;

    private void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!touchedGround)
        {
            transform.position -= transform.up * Time.deltaTime * 1;
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            touchedGround = true;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float fadeSpeed = startAlpha / fadeDuration;

        // Gradually fade the alpha to 0
        while (color.a > 0)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}