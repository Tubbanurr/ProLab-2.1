using UnityEngine;

public class Bird : MonoBehaviour
{
    public float moveSpeed = 2.5f; // Ku�un hareket h�z�
    private float moveRange = 5f; // Ku�un yukar� ve a�a�� hareket edebilece�i maksimum mesafe (5 birim)

    private Vector3 startPosition; // Ba�lang�� pozisyonu
    private bool movingUp = true; // Ku�un yukar� m� a�a�� m� hareket etti�ini kontrol eder

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; // Ba�lang�� pozisyonunu kaydet
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

        // Ku�un pozisyonunu g�ncelle
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
