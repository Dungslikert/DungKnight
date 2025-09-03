using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveUpSpeed = 1f;
    public float fadeSpeed = 2f;
    private TextMeshPro textMesh;
    private Color textColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        textColor = textMesh.color;
    }

    public void Setup(int damage, Color? color = null)
    {
        textMesh.text = damage.ToString();
        if (color != null)
        {
            textMesh.color = color.Value;
            textColor = color.Value;
        }
    }

    void Update()
    {
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;
        if (textColor.a <= 0)
            Destroy(gameObject);
    }
}
