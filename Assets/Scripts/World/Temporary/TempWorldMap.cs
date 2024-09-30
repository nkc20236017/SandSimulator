using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation.Temp
{
    public class TempWorldMap : ScriptableObject
    {
        [SerializeField, TextArea]
        private string[] map;

        [SerializeField]    // 地層のブロックとして認識するID
        private int[] layerTileID;
        public IReadOnlyCollection<int> LayerTileID => layerTileID;

        public string GetMap(int index)
        {
            return map[index];
        }
        public int GetMapLength()
        {
            return map.Length;
        }
    }
}