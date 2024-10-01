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
        /// �u���b�N�̃��X�g����ID���擾����
        /// </summary>
        /// <param name="tile">�擾�������^�C��</param>
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