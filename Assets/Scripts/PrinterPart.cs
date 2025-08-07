using UnityEngine;

public enum PartType { TierUpgrade, Battery, Cooling }
public class PrinterPart : MonoBehaviour
{

    [Header("Part Type")]
    public PartType type;

    [Header("Only used for Tier Upgrade")]
    public MoneyPrinter.PrinterTier tierToUpgradeTo;

    [Header("Only used for Battery / Cooling")]
    public float value = 0f;



    // Update is called once per frame
    void Update()
    {
        
    }
}
