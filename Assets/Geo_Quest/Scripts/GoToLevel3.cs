using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLevel3 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level 3");
        }
    }
}