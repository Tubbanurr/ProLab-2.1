using UnityEngine;

public class Bee : MonoBehaviour
{
    public float moveSpeed = 2.5f; // Arýnýn hareket hýzý
    private float moveRange = 3f; // Arýnýn saða ve sola hareket edebileceði maksimum mesafe

    private Vector3 startPosition; // Baþlangýç pozisyonu
    private float direction = 1f; // Hareket yönü, baþlangýçta saða doðru (+1)

    void Start()
    {
        startPosition = transform.position; // Baþlangýç pozisyonunu kaydet
    }

    void Update()
    {
        // Mevcut pozisyondan hareket mesafesi hesapla
        float move = Mathf.Sin(Time.time * moveSpeed) * moveRange;

        // Arýnýn yeni pozisyonunu hesapla ve güncelle
        transform.position = new Vector3(startPosition.x + move, startPosition.y, startPosition.z);
    }
}
