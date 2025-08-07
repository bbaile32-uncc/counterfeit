using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartOrderingComputer : MonoBehaviour
{
    public GameObject uiPanel;
    public Transform spawnPoint;
    public GameObject batteryPrefab;
    public GameObject coolingPrefab;
    public GameObject tierPrefab;
    public GameObject defaultSelectedButton;


    public PlayerMoney playerMoney;
    public float batteryCost = 50f;
    public float coolingCost = 75f;
    public float tierCost = 100f;


    void Start()
    {
        if (playerMoney == null)
        {
            Debug.LogWarning("PlayerMoney is not assigned");
        }
        uiPanel.SetActive(false);
    }

    public void ToggleUI()
    {
        uiPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        
              
    }

    public void OrderBattery()
    {
        if (playerMoney.TrySpend(batteryCost))
        {
            Debug.Log("Spawning battery part");
            Instantiate(batteryPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    public void OrderCooling()
    {
        if (playerMoney.TrySpend(coolingCost))
        {
            Debug.Log("Spawning cooler part");
            Instantiate(coolingPrefab, spawnPoint.position, Quaternion.identity);
        }

    }

    public void OrderTier()
    {
        if (playerMoney.TrySpend(tierCost)) 
        { 
            Debug.Log("Spawning tier upgrade part");
            Instantiate(tierPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
