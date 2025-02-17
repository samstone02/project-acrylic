using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ui.MainMenu
{
    public class PlayButton : MonoBehaviour
    {
        [field: SerializeField] public GameObject TutorialModal { get; set; }
        
        void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Gameplay"));
        }
    }
}