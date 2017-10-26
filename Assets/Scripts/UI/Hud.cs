using Level;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] GameObject player;
        [SerializeField] World world;
        [SerializeField] Text debugInfo;
        [SerializeField] RectTransform interactionBar;
        
        Inventory inventory;
        Vector2 interactionBarSize;
        BlockDestroyer destroyer;

        void Awake()
        {
            interactionBarSize = interactionBar.sizeDelta;
            interactionBar.gameObject.SetActive(false);
            destroyer = player.GetComponent<BlockDestroyer>();
            
            inventory = GetComponentInChildren<Inventory>();
            inventory.SetBlockBuilder(player.GetComponent<BlockBuilder>());
        }

        void DrawDebugInfo()
        {
            var fps = 1.0f / Time.deltaTime;
            debugInfo.text = $"FPS: {fps}\nChunks: {world.chunksCount}\n" + $"Blocks: {world.chunksCount * Mathf.Pow(Chunk.size, 3)}";
        }
        

        void Update()
        {
            if (debugInfo.gameObject.activeSelf) DrawDebugInfo();
            if (destroyer)
            {
                if (destroyer.isDestroying)
                {
                    interactionBar.gameObject.SetActive(true);
                    var remain = (destroyer.healthRemains/destroyer.blockHeath);
                    var newSize = new Vector2(remain * interactionBarSize.x, interactionBarSize.y);
                    interactionBar.sizeDelta = newSize;
                }
                else
                {
                    interactionBar.gameObject.SetActive(false);
                }
            }
        }
    }
}