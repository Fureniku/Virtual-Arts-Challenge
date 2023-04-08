using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableListManager : MonoBehaviour {

    [SerializeField] private GameObject buttonBase;
    [SerializeField] private GameObject contentHolder;
    [SerializeField] private PlaceableManager placeableManager;
    
    void Start() {
        GameObject[] goList = placeableManager.GetRegistry();

        for (int i = 0; i < goList.Length; i++) {
            GameObject createdButton = Instantiate(buttonBase, contentHolder.transform);
            PlaceableButtonController button = createdButton.GetComponent<PlaceableButtonController>();

            button.id = i;
            button.SetText(goList[i].name);
            button.SetImage(GenerateImage(button.GetImage(), goList[i]));
        }
    }

    //Render a screenshot of the prefab in question
    private Image GenerateImage(Image buttonImg, GameObject go) {
        GameObject prefabToRender = Instantiate(go, new Vector3(0, 0.5f, 0), Quaternion.identity);
        int imageSize = 256;
        // Create a new camera and position it to frame the prefab
        Camera camera = new GameObject("Camera").AddComponent<Camera>();

        camera.transform.position = new Vector3(-1, 2, -1);
        camera.transform.localEulerAngles = new Vector3(45, 45, 0);

        // Render the prefab to a texture
        RenderTexture renderTexture = new RenderTexture(imageSize, imageSize, 24);
        camera.targetTexture = renderTexture;
        camera.Render();

        // Convert the texture to a sprite and assign it to the Image component
        Texture2D texture = new Texture2D(imageSize, imageSize, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, imageSize, imageSize), Vector2.zero);
        buttonImg.sprite = sprite;

        // Clean up
        Destroy(camera.gameObject);
        Destroy(prefabToRender);
        camera.targetTexture = null;
        renderTexture.Release();

        return buttonImg;
    }
}
