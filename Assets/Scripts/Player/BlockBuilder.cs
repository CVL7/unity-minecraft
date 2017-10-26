using System;
using Level;
using UnityEngine;
using Utils;

namespace Player
{
    [RequireComponent(typeof(BlockInteractor))]
    public class BlockBuilder : MonoBehaviour
    {
        public event Action blockTypeSelected = delegate { };

        public BlockType blockTypeToBuild;
        public readonly BlockType[] buildableBlocks = {BlockType.Snow, BlockType.Ore, BlockType.Ice, BlockType.Stone};

        readonly KeyCode[] keyCodes =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };

        BlockInteractor interactor;
        Transform playerTransform;

        void Awake()
        {
            blockTypeToBuild = buildableBlocks[0];
            playerTransform = gameObject.transform;
            interactor = GetComponent<BlockInteractor>();
        }

        void Update()
        {
            for (var i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]) && i < buildableBlocks.Length)
                {
                    blockTypeToBuild = buildableBlocks[i];
                    blockTypeSelected();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (!interactor.isBlockSelected) return;
                // Prevent building on blocks where player stays
                var playerPosition = Vector3i.FloorToInt(playerTransform.localPosition - new Vector3(0, 0.5f, 0));
                var upperPosition = playerPosition + new Vector3i(0, 1, 0);
                var placeblePosition = interactor.placeblePosition + interactor.selectedChunk.chunkPosition;
    
                if (playerPosition != placeblePosition && upperPosition != placeblePosition)
                {
                    interactor.selectedChunk.CreateBlock(blockTypeToBuild, interactor.placeblePosition);
                }
            }
        }
    }
}