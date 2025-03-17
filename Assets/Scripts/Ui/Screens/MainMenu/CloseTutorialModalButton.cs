using UnityEngine;
using UnityEngine.UI;

namespace Ui.MainMenu
{
    public class CloseTutorialModalButton : MonoBehaviour
    {
        [field: SerializeField] public GameObject TutorialModal { get; set; }
        
        protected void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => TutorialModal.SetActive(false));
        }
    }
}