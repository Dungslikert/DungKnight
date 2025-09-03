using UnityEngine;

public class DamagePopupCreator : MonoBehaviour
{
    public static DamagePopupCreator Instance;
    public GameObject damagePopupPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CreatePopup(Vector3 position, int damage, Color? color = null)
    {
        var popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        popup.GetComponent<DamagePopup>().Setup(damage, color);
    }
}
