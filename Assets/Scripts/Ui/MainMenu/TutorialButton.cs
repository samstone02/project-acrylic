using UnityEngine;
using UnityEngine.UI;

namespace Ui.MainMenu
{
    public class TutorialButton : MonoBehaviour
    {
        [field: SerializeField] public GameObject TutorialModal { get; set; }
        
        void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => TutorialModal.SetActive(true));
        }
    }
}