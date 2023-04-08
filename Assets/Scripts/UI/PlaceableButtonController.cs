using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class PlaceableButtonController : MonoBehaviour {

        public int id;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
    
    
        public void SetText(string textIn) {
            text.SetText(textIn);
        }

        public Image GetImage() {
            return image;
        }

        public void SetImage(Image imageIn) {
            image.sprite = imageIn.sprite;
        }

        public void GenerateItem() {
            PlaceableRegistry.Instance.SetHeldObject(id);
        }
    }
}
