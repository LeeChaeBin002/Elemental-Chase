using UnityEngine;
using System.Collections;

    public class Swapper1 : MonoBehaviour
    {

        public GameObject[] character;
        public int index;
        public Texture btn_tex;
        void Awake()
        {
            foreach (GameObject c in character)
            {
                c.SetActive(false);
            }
            character[0].SetActive(true);
        }
    // 버튼 클릭 시 호출될 메서드
    public void SwapCharacter()
    {
        character[index].SetActive(false);
        index++;
        index %= character.Length;
        character[index].SetActive(true);
    }
}

