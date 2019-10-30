using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUISubmarine : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private PhysicsSim physicsSim;

   
    void Update(){
        for(int i = 0; i < 6; ++i){
            int j = (physicsSim.motors[i].motorOn) ? (1) : (0);
            images[i].sprite = sprites[2*i+j];
        }
    }
}
