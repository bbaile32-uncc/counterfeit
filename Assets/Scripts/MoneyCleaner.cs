using UnityEngine;
using TMPro;

public class MoneyCleaner : MonoBehaviour
{
    [Header("Cleaner Settings")]
    public Transform spawnPoint;        
    public GameObject cleanMoneyPrefab; 
    public float cleanDelay = 3f;       

    [Header("Cleaner UI")]
    public TextMeshProUGUI cleanerText; 

    private int dirtyCount = 0;
    private float timer = 0f;
    private int totalCleaned = 0;

    public int DirtyCount => dirtyCount;
    public int TotalCleaned => totalCleaned;
    public float SecondsToNextClean => (dirtyCount > 0) ? Mathf.Max(0f, cleanDelay - timer) : 0f;

    void OnTriggerEnter(Collider other)
    {
        DirtyMoney dm = other.GetComponent<DirtyMoney>();
        if (dm != null && !dm.isClean)
        {
            dirtyCount++;
            Destroy(other.gameObject); // remove dirty bill from the scene
        }
    }

    void Update()
    {
        // cleaning cycle
        if (dirtyCount > 0)
        {
            timer += Time.deltaTime;
            if (timer >= cleanDelay)
            {
                timer = 0f;
                dirtyCount--;

                // spawn clean bill
                GameObject cleanBill = Instantiate(cleanMoneyPrefab, spawnPoint.position, Quaternion.identity);
                DirtyMoney dm = cleanBill.GetComponent<DirtyMoney>();
                if (dm != null) dm.isClean = true;

                totalCleaned++;
            }
        }

        // Update UI
        if (cleanerText != null)
        {
            cleanerText.text =
                $"Dirty in cleaner: {dirtyCount}\n" +
                $"Total cleaned: {totalCleaned}\n" +
                $"Next clean in: {SecondsToNextClean:F1}s";
        }
    }
}
