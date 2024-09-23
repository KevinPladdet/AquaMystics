using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PermanentShop : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI tritonText;
    public float tritonTokensAmount;

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
}
