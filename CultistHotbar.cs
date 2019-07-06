using System.Collections.Generic;
using System.Linq;
using Assets.CS.TabletopUI;
using TabletopUi.Scripts.Interfaces;
using UnityEngine;

namespace CultistHotbar
{
    [BepInEx.BepInPlugin("net.robophreddev.CultistSimulator.CultistHotbar", "CultistHotbar", "0.0.1")]
    public class CultistHotbarMod : BepInEx.BaseUnityPlugin
    {
        private bool _keyPressLatch = false;

        readonly KeyCode[] HotbarKeys = new KeyCode[] {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Alpha0
        };

        private TabletopTokenContainer TabletopTokenContainer
        {
            get
            {
                {
                    var tabletopManager = (TabletopManager)Registry.Retrieve<ITabletopManager>();
                    if (tabletopManager == null)
                    {
                        this.Logger.LogError("Could not fetch TabletopManager");
                    }

                    return tabletopManager._tabletop;
                }
            }
        }

        void Start()
        {
            this.Logger.LogInfo("CultistHotbar initialized.");
        }

        void Update()
        {
            for (var i = 0; i < HotbarKeys.Length; i++)
            {
                var key = HotbarKeys[i];
                if (Input.GetKeyDown(key))
                {
                    if (!this._keyPressLatch)
                    {
                        this._keyPressLatch = true;
                        this.SelectHotbarSituation(i);
                    }

                    // A key was pressed, so bail without resetting key latch.
                    return;
                }
            }
            this._keyPressLatch = false;
        }

        void SelectHotbarSituation(int index)
        {
            if (TabletopManager.IsInMansus())
            {
                return;
            }

            var hotbarSituations =
                from situation in this.GetAllSituations()
                orderby situation.RectTransform.position.x
                select situation;

            var targetSituation = hotbarSituations.Skip(index).FirstOrDefault();
            if (!targetSituation)
            {
                return;
            }

            if (targetSituation.SituationController.IsOpen)
            {
                targetSituation.SituationController.CloseWindow();
            }
            else
            {
                targetSituation.SituationController.OpenWindow();
            }
        }

        IEnumerable<SituationToken> GetAllSituations()
        {
            foreach (var token in TabletopTokenContainer.GetTokens())
            {
                var situationToken = token as SituationToken;
                if (situationToken != null)
                {
                    yield return situationToken;
                }
            }
        }
    }
}