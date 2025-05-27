using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public string tenManChoi;

    public void LoadManChoiMoi()
    {
        SceneManager.LoadScene(tenManChoi);
    }

}
