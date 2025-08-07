using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public float money = 500f;
    public TextMeshProUGUI moneyText; // ✅ this goes inside the class

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

    void Update() // ✅ must include return type (void)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${money:F0}"; // ✅ missing semicolon
        }
    }
}
