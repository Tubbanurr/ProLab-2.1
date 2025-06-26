using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
public class Karakter : MonoBehaviour
{
    public int moveSpeed = 1;
    public int ID { get; private set; }
    public string Ad { get; private set; }
    public Lokasyon CurrentLocation { get; private set; }

    // Constructor
    public Karakter(int id, string ad, Lokasyon baslangicLokasyonu)
    {
        ID = id;
        Ad = ad;
        CurrentLocation = baslangicLokasyonu;
    }

    // ID ve Ad için Set metotlarý
    public void SetID(int id)
    {
        ID = id;
    }

    public void SetAd(string ad)
    {
        Ad = ad;
    }

    // Karakterin lokasyonunu güncelleyen metot
    public void Move(int x, int y)
    {
        // Sadece sað, sol, yukarý veya aþaðý hareket
        if (x == CurrentLocation.x || y == CurrentLocation.y)
        {
            CurrentLocation.x += x;
            CurrentLocation.y += y;
        }
        else
        {
            Console.WriteLine("Çapraz hareket edilemez!");
        }
    }

    public void Initialize(int id, string ad, Lokasyon baslangicLokasyonu)
    {
        ID = id;
        Ad = ad;
        CurrentLocation = baslangicLokasyonu;
        // Karakterin fiziksel konumunu güncelle
        transform.position = new Vector3(baslangicLokasyonu.x, baslangicLokasyonu.y, 0);
    }
    // En Kýsa Yol hesaplamasý için bir metod; bu örnek, detaylý bir algoritma içermez
    // Gerçek dünya uygulamasýnda, A* gibi bir algoritma kullanabilirsiniz
    public void EnKisaYol(Lokasyon hedefLokasyon)
    {
        // En kýsa yol hesaplama (burada basit bir placeholder)
        Console.WriteLine($"En kýsa yol hesaplanýyor: ({CurrentLocation.x}, {CurrentLocation.y}) -> ({hedefLokasyon.x}, {hedefLokasyon.y})");
    }

    private List<Vector2Int> path; // Karakterin takip etmesi gereken yol
    private int currentPathIndex = 0; // Karakterin þu anki hedef konumu index'i
    private int stepCount = 0; // Karakterin attýðý adým sayýsý
    private int treasureCount = 0; // Karakterin topladýðý hazine sayýsý

    public void Initialize(List<Vector2Int> path)
    {
        this.path = path;
        transform.position = new Vector3(path[0].x, path[0].y, 0);
    }

    void Update()
    {
        // Karakter hedef konuma ulaþtý mý kontrol et
        if (transform.position == new Vector3(path[currentPathIndex].x, path[currentPathIndex].y, 0))
        {
            // Hedef konuma ulaþýldýysa
            if (currentPathIndex < path.Count - 1)
            {
                // Bir sonraki hedef konumu belirle
                currentPathIndex++;
                stepCount++;
                // Hazine sandýðý var mý kontrol et
                GameObject treasure = GetTreasureAtCurrentPosition();
                if (treasure != null)
                {
                    // Hazine sandýðý varsa topla
                    CollectTreasure(treasure);
                }
            }
        }
        else
        {
            // Hedefe ulaþmadýysa, hedefe doðru hareket et
            MoveToNextPosition();
        }
    }

    void MoveToNextPosition()
    {
        // Hedef konuma doðru hareket et
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[currentPathIndex].x, path[currentPathIndex].y, 0), Time.deltaTime * moveSpeed);
    }

    GameObject GetTreasureAtCurrentPosition()
    {
        // Eðer karakterin olduðu konumda bir hazine varsa, onu döndür
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Treasure"))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void CollectTreasure(GameObject treasure)
    {
        // Hazineyi topla
        Destroy(treasure);
        treasureCount++;
        // Toplanan hazineyi ekrana yazdýr
        Debug.Log("Hazine Toplandý! Toplam Hazine Sayýsý: " + treasureCount);
    }
}

