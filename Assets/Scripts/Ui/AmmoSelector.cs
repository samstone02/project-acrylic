using System.Collections.Generic;
using TankAgents;
using TankGuns;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class AmmoSelector : MonoBehaviour
    {
        [field: SerializeField] public List<GameObject> SelectorList { get; set; }

        public void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SelectorList.Add(transform.GetChild(i).gameObject);
            }

            var playerTank = GameObject.Find("PlayerTank");
            var playerAgent = playerTank.GetComponentInChildren<PlayerTankAgent>();
            playerAgent.SelectStandardAmmoEvent += () => SelectShell("StandardShellSelector");
            playerAgent.SelectExplosiveAmmoEvent += () => SelectShell("ExplosiveShellSelector");
            playerAgent.SelectRicochetAmmoEvent += () => SelectShell("RicochetShellSelector");
            
            var playerCannon = playerTank.GetComponentInChildren<AutoLoadingCannon>();
            
            foreach (var selector in SelectorList)
            {
                int idx = selector.name.IndexOf("Selector");
                string nameWithoutSelector = selector.name.Substring(0, idx);
                
                var img = selector.GetComponent<Image>();
                var rectTransform = selector.GetComponent<RectTransform>();
                
                if (nameWithoutSelector == playerCannon.ProjectilePrefab.name)
                {
                    img.color = Color.cyan;
                    rectTransform.sizeDelta = new Vector2(65, 65);
                }
                else
                {
                    img.color = Color.gray;
                    rectTransform.sizeDelta = new Vector2(60, 60);
                }
            }
        }

        private void SelectShell(string selectorName)
        {
            foreach (var selector in SelectorList)
            {
                var img = selector.GetComponent<Image>();
                var rectTransform = selector.GetComponent<RectTransform>();
                
                if (selector.name == selectorName)
                {
                    img.color = Color.cyan;
                    rectTransform.sizeDelta = new Vector2(65, 65);
                }
                else
                {
                    img.color = Color.gray;
                    rectTransform.sizeDelta = new Vector2(60, 60);
                }
            }
        }
    }
}