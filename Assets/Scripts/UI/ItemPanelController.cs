using System.Globalization;
using TMPro;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace UI {
	public class ItemPanelController : MonoBehaviour {

		[SerializeField] private GameController controller;

		[SerializeField] private TextMeshProUGUI title;
	
		[SerializeField] private TMP_InputField posX;
		[SerializeField] private TMP_InputField posY;
		[SerializeField] private TMP_InputField posZ;
	
		[SerializeField] private TMP_InputField rotX;
		[SerializeField] private TMP_InputField rotY;
		[SerializeField] private TMP_InputField rotZ;
	
		[SerializeField] private TMP_InputField scaleX;
		[SerializeField] private TMP_InputField scaleY;
		[SerializeField] private TMP_InputField scaleZ;

		[SerializeField] private TextMeshProUGUI tip;

		public void Awake() {
			ToggleMouse();
		}

		public void IncreaseButton(TMP_InputField value) {
			value.text = "" + (float.Parse(value.text, CultureInfo.InvariantCulture)+1);
			UpdateObject();
		}
	
		public void DecreaseButton(TMP_InputField value) {
			value.text = "" + (float.Parse(value.text, CultureInfo.InvariantCulture)-1);
			UpdateObject();
		}

		public void UpdateObject() {
			controller.UpdateObjectFromUI(
				new Vector3(p(posX), p(posY), p(posZ)),
				new Vector3(p(rotX), p(rotY), p(rotZ)),
				new Vector3(p(scaleX), p(scaleY), p(scaleZ)));
		}

		//Shorthand parsing function
		private float p(TMP_InputField text) {
			return float.Parse(text.text, CultureInfo.InvariantCulture);
		}

		public void ToggleMouse() {
			if (Cursor.lockState == CursorLockMode.Locked) {
				tip.text = "Press LEFT ALT to access the mouse";
			} else {
				tip.text = "Press ENTER to complete changes";
			}
		}

		public void UpdatePanel(GameObject go) {
			title.text = go.name;
			Vector3 pos = go.transform.position;
			Vector3 rot = go.transform.eulerAngles;
			Vector3 scale = go.transform.localScale;

			posX.text = "" + pos.x;
			posY.text = "" + pos.y;
			posZ.text = "" + pos.z;
		
			rotX.text = "" + rot.x;
			rotY.text = "" + rot.y;
			rotZ.text = "" + rot.z;
		
			scaleX.text = "" + scale.x;
			scaleY.text = "" + scale.y;
			scaleZ.text = "" + scale.z;
		}

	}
}
