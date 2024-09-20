using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    [Serializable]
    public struct BlockList
    {
        [SerializeField]
        private TileBase[] useBlockList;

        /// <summary>
        /// ブロックのリストからIDを取得する
        /// </summary>
        /// <param name="tile">取得したいタイル</param>
        /// <returns></returns>
        public int GetBlockID(TileBase tile)
        {
            return Array.IndexOf(useBlockList, tile) + 1;
        }

        public TileBase GetBlock(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return useBlockList[id - 1];
        }
    }
}