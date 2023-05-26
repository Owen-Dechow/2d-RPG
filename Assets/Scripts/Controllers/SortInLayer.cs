using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortInLayer : MonoBehaviour
{
    Renderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer.sortingLayerName == "Objects")
        {
            spriteRenderer.sortingOrder = (int)(-transform.position.y*100);
        }
    }
}
