using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Outbox : MonoBehaviour
{
    public PlayerMoney playerMoney;
    public float valuePerBill = 100f;

    void Awake() { GetComponent<Collider>().isTrigger = false; }

    void OnCollisionEnter(Collision collision)
    {
        var dm = collision.collider.GetComponent<DirtyMoney>();
        if (dm == null || !dm.isClean) return;   // only cash clean bills

        if (playerMoney != null) playerMoney.money += valuePerBill;
        Destroy(collision.collider.attachedRigidbody
            ? collision.collider.attachedRigidbody.gameObject
            : collision.collider.gameObject);
    }
}
