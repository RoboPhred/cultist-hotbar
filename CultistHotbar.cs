using System.Collections.Generic;
using System.Linq;
using Assets.CS.TabletopUI;
using TabletopUi.Scripts.Interfaces;
using UnityEngine;

namespace CultistHotbar
{
    [BepInEx.BepInPlugin("net.robophreddev.CultistSimulator.CultistHotbar", "CultistHotbar", "0.0.2")]
    public class CultistHotbarMod : BepInEx.BaseUnityPlugin
    {
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

        readonly HashSet<string> PrimarySituations = new HashSet<string>() {
            "dream",
            "work",
            "study",
            "time",
            "explore",
            "talk"
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
                    var secondarySituations = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    this.SelectHotbarSituation(i, secondarySituations);

                    // A key was pressed, so bail without resetting key latch.
                    return;
                }
            }
        }

        void SelectHotbarSituation(int index, bool targetSecondarySituations)
        {
            if (TabletopManager.IsInMansus())
            {
                return;
            }

            var hotbarSituations =
                from situation in this.GetAllSituations()
                where PrimarySituations.Contains(situation.EntityId) != targetSecondarySituations
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