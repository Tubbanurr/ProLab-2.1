using UnityEngine;

public class Bird : MonoBehaviour
{
    public float moveSpeed = 2.5f; // Kuþun hareket hýzý
    private float moveRange = 5f; // Kuþun yukarý ve aþaðý hareket edebileceði maksimum mesafe (5 birim)

    private Vector3 startPosition; // Baþlangýç pozisyonu
    private bool movingUp = true; // Kuþun yukarý mý aþaðý mý hareket ettiðini kontrol eder

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; // Baþlangýç pozisyonunu kaydet
    }

    // Update is called once per frame
    void Update()
    {
        MoveVertically();
    }

    void MoveVertically()
    {
        // Mevcut y pozisyonu ve hedef y pozisyonunu hesapla
        float newY = Mathf.Sin(Time.time * moveSpeed) * moveRange + startPosition.y;

        // Kuþun pozisyonunu güncelle
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
