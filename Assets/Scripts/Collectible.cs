using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    public enum ColorType { Red, Green, Blue }
    public ColorType colorType = ColorType.Red;

    void Reset()
    {
        Collider c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    void Start()
    {
        transform.position += Vector3.up * 0.2f;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // This calls GameManager.Collect() which handles:
            
            GameManager.Instance.Collect(this);

            Destroy(gameObject);
        }
    }

    public void ApplyMaterial(Material mat)
    {
        var r = GetComponent<Renderer>();
        if (r != null) r.material = mat;
    }
}
