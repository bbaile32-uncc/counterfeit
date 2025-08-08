using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class PartOrderingComputer : MonoBehaviour
{
    public MoneyPrinter moneyPrinter;

    [Header("UI")]
    public GameObject uiPanel;
    public GameObject defaultSelectedButton;
    public TextMeshProUGUI TaskText;

    [Header("Spawn/Prefabs")]
    public Transform spawnPoint;
    public GameObject batteryPrefab;
    public GameObject coolingPrefab;
    public GameObject tierPrefab;

    [Header("Economy")]
    public PlayerMoney playerMoney;
    public float batteryCost = 50f;
    public float coolingCost = 75f;
    public float tierCost = 100f;

    // --Task System
    [System.Serializable] public struct SimpleTask { public string id; public string label; }
    public SimpleTask[] tasks = new SimpleTask[]
    {
        new SimpleTask{ id = "OrderBattery", label = "Order a Battery"},
        new SimpleTask{ id = "InstallBattery", label = "Install Battery"},
        new SimpleTask{ id = "OrderCooling", label = "Order a Cooling Unit" },
        new SimpleTask{ id = "InstallCooling", label = "Install Cooling"},
        new SimpleTask{ id = "OrderTier", label = "Order a Tier Upgrade"},
        new SimpleTask{ id = "InstallUpgrade", label = "Install Upgrade"},
    };
    int taskIndex = 0;
    void Start()
    {
        if (playerMoney == null)
        {
            Debug.LogWarning("PlayerMoney is not assigned");
        }
        UpdateTaskUI();
        uiPanel.SetActive(false);
    }

    public void ToggleUI()
    {
        uiPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        if (defaultSelectedButton) EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        
              
    }

    public void OrderBattery()
    {
               
        if (playerMoney.TrySpend(batteryCost))
        {
            Debug.Log("Spawning battery part");
            Instantiate(batteryPrefab, spawnPoint.position, Quaternion.identity);
            CompleteTask("OrderBattery");
        }
    }

    public void OrderCooling()
    {
        if (!moneyPrinter.HasBatteryInstalled())
        {
            Debug.Log("You must install the cooling before buying upgrade.");
            return;
        }

        if (playerMoney.TrySpend(coolingCost))
        {
            Debug.Log("Spawning cooler part");
            Instantiate(coolingPrefab, spawnPoint.position, Quaternion.identity);
            CompleteTask("OrderCooling");
        }

    }

    public void OrderTier()
    {
        if (!moneyPrinter.HasCoolingInstalled())
        {
            Debug.Log("You must install the cooling before buying tier upgrade.");
            return;
        }

        if (playerMoney.TrySpend(tierCost)) 
        { 
            Debug.Log("Spawning tier upgrade part");
            Instantiate(tierPrefab, spawnPoint.position, Quaternion.identity);
            CompleteTask("OrderTier");
        }
    }

    void UpdateTaskUI()
    {
        if (!TaskText) return;
        TaskText.text = (taskIndex < tasks.Length)
            ? $"TASK: {tasks[taskIndex].label}"
            : "All tasks done!";
    }

    public void CompleteTask(string id)
    {
        if (taskIndex < tasks.Length && tasks[taskIndex].id == id)
        {
            taskIndex++;
            UpdateTaskUI();
        }
        
    }
}
