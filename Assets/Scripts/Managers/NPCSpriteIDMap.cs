using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class NpcSpriteIDMap : MonoBehaviour
    {
        private static NpcSpriteIDMap _i;
        [SerializeField] private List<Sprite> sprites;

        public static Sprite GetSprite(byte id)
        {
            return _i.sprites[id];
        }

        public static byte GetID(Sprite sprite)
        {
            byte id = (byte)_i.sprites.IndexOf(sprite);
            if (id >= 0)
                return id;

            throw new System.Exception("Sprite was not found!");
        }

        private void Start()
        {
            _i = this;
        }
    }
}