using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCSpriteIDMap : MonoBehaviour
{
    public static NPCSpriteIDMap i;
    [SerializeField] List<Sprite> sprites;

    public static Sprite GetSprite(byte id)
    {
        return i.sprites[id];
    }

    public static byte GetID(Sprite sprite)
    {
        byte id = (byte)i.sprites.IndexOf(sprite);
        if (id >= 0)
            return id;

        throw new System.Exception("Sprite was not found!");
    }

    private void Start()
    {
        i = this;
    }
}