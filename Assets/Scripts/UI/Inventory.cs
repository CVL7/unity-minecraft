using System.Collections.Generic;
using Level;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] RectTransform blockIconPrefab;
        readonly List<RawImage> iconFrames = new List<RawImage>();
        BlockBuilder builder;


        void OnDisable()
        {
            if (builder) builder.blockTypeSelected -= OnBlockTypeSelected;
        }

        void OnBlockTypeSelected()
        {
            for (var i = 0; i < iconFrames.Count; i++)
            {
                iconFrames[i].enabled = builder.buildableBlocks[i] == builder.blockTypeToBuild;
            }
        }

        public void SetBlockBuilder(BlockBuilder blockBuilder)
        {
            builder = blockBuilder;
            if (blockBuilder)
            {
                builder.blockTypeSelected += OnBlockTypeSelected;
                foreach (var blockType in blockBuilder.buildableBlocks)
                {
                    var blockIcon = Instantiate(blockIconPrefab);
                    blockIcon.SetParent(gameObject.transform, false);

                    var icon = blockIcon.GetComponentsInChildren<RawImage>();
                    icon[0].enabled = false;
                    icon[1].uvRect = MeshBuilder.GetTileUVsRect(Block.tiles[blockType]);
                    iconFrames.Add(icon[0]);
                }
                OnBlockTypeSelected();
            }
            else
            {
                enabled = false;
            }

            // For some reason letting ContentSizeFitter enabled rebuilds canvas every frame, even if content does not changes
            Canvas.ForceUpdateCanvases();
            var size = GetComponent<RectTransform>().sizeDelta;
            GetComponent<ContentSizeFitter>().enabled = false;
            GetComponent<RectTransform>().sizeDelta = size;
        }
    }
}