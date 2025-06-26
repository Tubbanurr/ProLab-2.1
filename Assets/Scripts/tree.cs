using UnityEngine;

public class Tree : Obstacle
{
   
    protected override void InitializeObstacle()
    {
        base.InitializeObstacle(); 
        // A�ac�n boyutunu rastgele belirle
        int[] possibleSizes = new int[] { 2, 3, 4, 5 }; // Boyut se�enekleri
        int sizeIndex = Random.Range(0, possibleSizes.Length); // Rastgele bir indeks se�
        size = new Vector2(possibleSizes[sizeIndex], possibleSizes[sizeIndex]); // Boyutu ayarla

    }

    void Start()
    {
        InitializeObstacle(); // A�ac� ba�lat
    }

    
}
