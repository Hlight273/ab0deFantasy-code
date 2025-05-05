using HFantasy.Script.Core;
using HFantasy.Script.Entity.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HFantasy.Script.UI
{
    public class LifeBarManager : MonoBehaviour
    {
        [SerializeField] private GameObject lifeBarPrefab;
        [SerializeField] private Transform lifeBarContainer;
        //[SerializeField] private float updateInterval = 0.1f; //更新间隔
        [SerializeField] private float smoothSpeed = 5f; //平滑过渡速度

        private Dictionary<int, Slider> lifeBarDict = new Dictionary<int, Slider>();
        private Dictionary<int, float> targetValues = new Dictionary<int, float>(); //存储目标血量值
        private Camera mainCamera;
        private Canvas canvas;
        //private float lastUpdateTime;

        private void Start()
        {
            if (lifeBarContainer == null)
                lifeBarContainer = transform;

            mainCamera = Camera.main;
            canvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            // 限制更新频率
            //if (Time.time - lastUpdateTime < updateInterval) return;
            //lastUpdateTime = Time.time;

            //获取所有玩家并按照与摄像机的距离排序
            var sortedPlayers = EntityManager.Instance.PlayerDict.Values
                .OrderByDescending(p => Vector3.Distance(mainCamera.transform.position, p.PlayerObject.transform.position))
                .ToList();

            foreach (var player in sortedPlayers)
            {
                UpdatePlayerLifeBar(player);
            }
        }

        private void UpdatePlayerLifeBar(PlayerEntity player)
        {
            if (!lifeBarDict.TryGetValue(player.Id, out Slider lifeBarSlider))
            {
                GameObject lifeBarGO = Instantiate(lifeBarPrefab, lifeBarContainer);
                lifeBarSlider = lifeBarGO.GetComponent<Slider>();
                lifeBarDict.Add(player.Id, lifeBarSlider);
                targetValues.Add(player.Id, player.Info.Life);
            }
            //更新血条的值
            if (lifeBarSlider.maxValue != player.Info.MaxLife)
                lifeBarSlider.maxValue = player.Info.MaxLife;
            float currentTarget = player.Info.Life;
            targetValues[player.Id] = currentTarget;
            float smoothedValue = Mathf.Lerp(lifeBarSlider.value, targetValues[player.Id], Time.deltaTime * smoothSpeed);//平滑过渡
            lifeBarSlider.value = smoothedValue;

            //更新位置（考虑Canvas缩放）
            if (canvas && mainCamera)
            {
                Vector3 worldPos = player.PlayerObject.transform.position + Vector3.up * player.Info.PlayerViewHeight;
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    lifeBarSlider.transform.position = screenPos;
                }
                else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    Vector2 viewportPos = mainCamera.WorldToViewportPoint(worldPos);
                    lifeBarSlider.transform.position = new Vector3(
                        viewportPos.x * canvas.pixelRect.width,
                        viewportPos.y * canvas.pixelRect.height,
                        0
                    );
                }

                //设置血条的显示顺序
                float distanceToCamera = Vector3.Distance(mainCamera.transform.position, player.PlayerObject.transform.position);
                lifeBarSlider.transform.SetSiblingIndex(Mathf.RoundToInt(distanceToCamera * 100f));
            }

            lifeBarSlider.gameObject.SetActive(!player.Info.IsDead);
        }
    }
}