using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class menu : MonoBehaviour
{
    public GameObject fogPanel;

    public void StartButton()
    {
        // Sahne yüklendiðinde çaðrýlacak olayý belirleyin
        MapGenerator mapGenerator = new MapGenerator();
        // Sahneyi yeniden yükle
        SceneManager.LoadScene(0);
        mapGenerator.GenerateFog();

    }




    IEnumerator LoadSceneAfterDelay()
    {
        // Optionally wait for a delay or till the end of frame.
        // yield return new WaitForSeconds(1);
        yield return new WaitForEndOfFrame();

        // Now, load the scene.
        SceneManager.LoadScene(0);
    }

    public void NewMapButton()
    {
        SceneManager.LoadScene(0);
    }
}
