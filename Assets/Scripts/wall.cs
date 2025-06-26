using UnityEngine;

public class Wall : Obstacle
{
    public float length = 10.0f; 

   
    void Start()
    {
        InitializeObstacle();
        InitializeWall();
    }

  

    void InitializeWall()
    {
        transform.localScale = new Vector3(length, 1, 1); 
        Debug.Log("Wall initialized with length: " + length);
    }
}
