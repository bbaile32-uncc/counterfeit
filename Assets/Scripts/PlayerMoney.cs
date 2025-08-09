using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public float money = 500f;
    public TextMeshProUGUI moneyText; 

    public bool TrySpend(float cost)
    {
        if (money >= cost)
        {
            money -= cost;
            Debug.Log($"Spent ${cost}. Remaining: ${money}");
            return true;
        }

        Debug.Log("Not enough money!");
        return false;
    }

    void Update() 
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${money:F0}"; 
        }
    }
}
