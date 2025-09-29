using UnityEngine;

namespace LayerLab.CasualGame
{
    public class PanelView : MonoBehaviour
    {
        [SerializeField] private GameObject[] otherPanels;

        public void OnEnable()
        {
            if (otherPanels == null) return;   // ✅ 추가
            foreach (var p in otherPanels)
            {
                if (p != null) p.SetActive(false);
            }
        }

        public void OnDisable()
        {
            for (int i = 0; i < otherPanels.Length; i++) otherPanels[i].SetActive(false);
        }
    }
}
