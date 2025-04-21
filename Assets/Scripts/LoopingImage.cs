using UnityEngine;
using UnityEngine.UI;

public class LoopingUIImage : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private RawImage image;
    private Rect uvRect;

    void Start()
    {
        image = GetComponent<RawImage>();
        uvRect = image.uvRect;
    }

    void Update()
    {
        uvRect.x = (uvRect.x + Time.deltaTime * scrollSpeed) % 1f;
        image.uvRect = uvRect;
    }
}
