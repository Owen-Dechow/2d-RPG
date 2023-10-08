using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{
    void Start()
    {
        Destroy(GetComponent<SpriteRenderer>());
    }
}
