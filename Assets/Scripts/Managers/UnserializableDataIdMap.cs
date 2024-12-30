using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Managers
{
    public class UnserializableDataIdMap : MonoBehaviour
    {
        private static UnserializableDataIdMap _i;
        [SerializeField] private List<Sprite> sprites;
        [SerializeField] private List<ItemScriptable> items;
        [SerializeField] private List<MagicScriptable> magic;

        public static Sprite GetSprite(short id) => _i.sprites[id];

        public static short GetSpriteID(Sprite sprite)
        {
            short id = (short)_i.sprites.IndexOf(sprite);
            
            if (id >= 0) return id;
            else throw new System.Exception("Sprite was not found!");
        }

        public static MagicScriptable GetMagic(short id) => _i.magic[id];
        public static short GetMagicID(MagicScriptable magic)
        {
            short id = (short)_i.magic.IndexOf(magic);
            
            if (id >= 0) return id;
            else throw new System.Exception("Magic was not found!");
        }

        public static ItemScriptable GetItem(short id) => _i.items[id];

        public static short GetItemID(ItemScriptable item)
        {
            short id  = (short)_i.items.IndexOf(item);
            
            if (id >= 0) return id;
            else throw new System.Exception("Item was not found!");
        }

        private void Awake() => _i = this;
    }
}