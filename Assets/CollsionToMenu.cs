using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenuOnCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // This triggers whenever the spacecraft hits anything
        SceneNavigator.GoToMainMenu(2f);
    }
}
