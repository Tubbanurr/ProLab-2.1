using UnityEngine;

public class Bee : MonoBehaviour
{
    public float moveSpeed = 2.5f; // Ar�n�n hareket h�z�
    private float moveRange = 3f; // Ar�n�n sa�a ve sola hareket edebilece�i maksimum mesafe

    private Vector3 startPosition; // Ba�lang�� pozisyonu
    private float direction = 1f; // Hareket y�n�, ba�lang��ta sa�a do�ru (+1)

    void Start()
    {
        startPosition = transform.position; // Ba�lang�� pozisyonunu kaydet
    }

    void Update()
    {
        // Mevcut pozisyondan hareket mesafesi hesapla
        float move = Mathf.Sin(Time.time * moveSpeed) * moveRange;

        // Ar�n�n yeni pozisyonunu hesapla ve g�ncelle
        transform.position = new Vector3(startPosition.x + move, startPosition.y, startPosition.z);
    }
}
