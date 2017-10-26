using Level;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] GameObject player;
        [SerializeField] World world;
        Image loadingBar;

        void Awake()
        {
            loadingBar = gameObject.GetComponentInChildren<Image>();
            player.SetActive(false);
        }

        void Update()
        {
            loadingBar.fillAmount = (float)world.currentlyRenderingLayer / world.viewDistance;
        }

        void OnEnable()
        {
            world.firstGenerationComplete += OnWorldGenerated;
        }
        
        void OnDisable()
        {
            world.firstGenerationComplete -= OnWorldGenerated;
        }

        void OnWorldGenerated()
        {
            gameObject.SetActive(false);
            player.SetActive(true);
        }

    }
}