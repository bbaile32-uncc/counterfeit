using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MoneyPrinter : MonoBehaviour
{

    public enum PrinterTier { Rusty, Bronze, Silver, Gold, Platinum }

    public PrinterTier tier = PrinterTier.Rusty;
    public float battery = 100f;
    public float temperature = 25f;
    public float printRate = 1f;
    public UnityEngine.UI.Slider riskSlider;
    public TextMeshProUGUI riskText;
    public Slider riskBar;

    public List<string> installedParts = new List<string>();

    public TextMeshProUGUI statusText;

    private bool hasBattery = false;
    private bool hasCooling = false;
    private bool hasTierUpgrade = false;

    public bool HasBatteryInstalled() => hasBattery;
    public bool HasCoolingInstalled() => hasCooling;
    public bool HasUpgradeInstalled() => hasTierUpgrade;

    private void OnCollisionEnter(Collision collision)
    {
        PrinterPart part = collision.gameObject.GetComponent<PrinterPart>();
        if (part != null && !installedParts.Contains(part.type.ToString()))
        {
            Debug.Log($"Installed {part.type} part on printer");
            installedParts.Add(part.type.ToString());

            switch (part.type)
            {
                case PartType.Battery:
                    battery += part.value;
                    hasBattery = true;
                    FindFirstObjectByType<PartOrderingComputer>()?.CompleteTask("InstallBattery");
                    break;
                case PartType.Cooling:
                    temperature -= part.value;
                    hasCooling = true;
                    FindFirstObjectByType<PartOrderingComputer>()?.CompleteTask("InstallCooling");
                    break;
                case PartType.TierUpgrade:
                    tier = part.tierToUpgradeTo;
                    FindFirstObjectByType<PartOrderingComputer>()?.CompleteTask("InstallUpgrade");
                    hasTierUpgrade = true;
                    break;
            }

            UpdateStatusUI();
            Destroy(part.gameObject);
        }
    }


    void UpdateStatusUI()
    {
        string status = "";

        status += hasBattery ? "Battery Installed\n" : "Battery Missing\n";
        status += hasCooling ? "Cooling Installed\n" : "Cooling Missing\n";
        status += hasTierUpgrade ? "Tier Upgrade Installed\n" : "Tier Upgrade Missing\n";

        status += "\n------------------\n";

        
        if (hasBattery && hasCooling && hasTierUpgrade)
        {
            status += "[ PRINTING ]";

            if (isPrinting)
            {
                status += $"\n Printing: ${printRatePerSecond}/s";
            }
        }
        else
        {
            status += "[ WAITING FOR PARTS ]";
        }

        status += $"\nStored: ${moneyStored:F0}";

        if (statusText != null)
        {
            statusText.text = status;
        }
        
    }

    [Header("Printer Stats")]
    public float printRatePerSecond = 5f;
    private float moneyStored = 0f;
    private bool isPrinting = false;

    [Header("Money Prefab")]
    public GameObject MoneyPrefab;

    [Header("Withdraw Settings")]
    public Transform moneySpawnPoint;
    public float withdrawAmount = 50f;

    [Header("Risk Settings")]
    public float minRate = 1f;
    public float maxRate = 20f;

    [Header("Game Risk")]
    public float overallRisk = 0f;
    public float riskIncreaseRate = 0.005f;

    public void Withdraw()
    {
        if (moneyStored >= withdrawAmount)
        {
            GameObject cash = Instantiate(MoneyPrefab, moneySpawnPoint.position, Quaternion.identity);
            PlayerMoney m = cash.GetComponent<PlayerMoney>();
            if (m != null)
            {
                m.money = withdrawAmount;
            }

            moneyStored -= withdrawAmount;

        }
    }

   
    public void IncreaseRisk(float amount = 0.1f)
    {
        if (riskSlider != null)
        {
            riskSlider.value = Mathf.Clamp01(riskSlider.value + amount);
        }
    }

    public void DecreaseRisk(float amount = 0.1f)
    {
        if (riskSlider != null)
        {
            riskSlider.value = Mathf.Clamp01(riskSlider.value - amount);
        }
    }

    void UpdateRiskFromPlayerMoney()
    {
        PlayerMoney player = FindFirstObjectByType<PlayerMoney>();
        if (player == null) return;
        {
            float maxSafeMoney = 1000f;
            float risk = Mathf.Clamp01(player.money / maxSafeMoney);

            if (riskBar != null)
            {
                riskBar.value = risk;
            }

            if (riskSlider != null)
            {
                riskSlider.value = Mathf.Max(riskSlider.value, risk);
            }

            if (risk > riskSlider.value)
            {
                overallRisk += riskIncreaseRate * Time.deltaTime;
                overallRisk = Mathf.Clamp01(overallRisk);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (hasBattery && hasCooling && hasTierUpgrade) 
        {
            isPrinting = true;
        }
        else
        {
            isPrinting = false;
        }

        if (isPrinting)
        {
            moneyStored += printRatePerSecond * Time.deltaTime;
        }

        if (riskSlider != null)
        {
            float risk = riskSlider.value;
            printRatePerSecond = Mathf.Lerp(minRate, maxRate, risk);

            if (riskText != null)
            {
                riskText.text = $"Risk: {(risk * 100f):F0}%";
            }
        }

        UpdateRiskFromPlayerMoney();
        
        UpdateStatusUI();

    }
}
