using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUISubmarine : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private PhysicsSim physicsSim;

    private void Start() {
        for (int i = 0; i < buttons.Length; ++i) {
            buttons[i].onClick.AddListener(physicsSim.soloMotors[i].SwitchOnOff);
        }
    }

    void Update(){
        for(int i = 0; i < images.Length; ++i){
            int j = (physicsSim.motors[i].motorOn) ? (1) : (0);
            images[i].sprite = sprites[2*i+j];
        }
    }
}
