using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableButtonController : MonoBehaviour {

    public int id;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;
    
    void Start() {}

    public void SetText(string textIn) {
        text.SetText(textIn);
    }

    public Image GetImage() {
        return image;
    }

    public void SetImage(Image imageIn) {
        image.sprite = imageIn.sprite;
    }
}
