using UnityEngine;

public class Tree : Obstacle
{
   
    protected override void InitializeObstacle()
    {
        base.InitializeObstacle(); 
        // Aðacýn boyutunu rastgele belirle
        int[] possibleSizes = new int[] { 2, 3, 4, 5 }; // Boyut seçenekleri
        int sizeIndex = Random.Range(0, possibleSizes.Length); // Rastgele bir indeks seç
        size = new Vector2(possibleSizes[sizeIndex], possibleSizes[sizeIndex]); // Boyutu ayarla

    }

    void Start()
    {
        InitializeObstacle(); // Aðacý baþlat
    }

    
}
