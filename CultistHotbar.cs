using System;
using System.Collections.Generic;
using System.Linq;
using SecretHistories.Entities;
using SecretHistories.Enums;
using SecretHistories.Spheres;
using SecretHistories.Fucine;
using SecretHistories.UI;
using SecretHistories.Services;
using UnityEngine.InputSystem;
using UnityEngine;

public class CultistHotbar : MonoBehaviour
{
    readonly Key[] HotbarKeys = new Key[] {
            Key.Digit1,
            Key.Digit2,
            Key.Digit3,
            Key.Digit4,
            Key.Digit5,
            Key.Digit6,
            Key.Digit7,
            Key.Digit8,
            Key.Digit9,
            Key.Digit0
        };

    readonly HashSet<string> PrimarySituations = new HashSet<string>() {
            "dream",
            "work",
            "study",
            "time",
            "explore",
            "talk"
        };

    public static void Initialise()
    {
        try
        {
            NoonUtility.LogWarning($"HotBar intitializing");
            new GameObject().AddComponent<CultistHotbar>();
        }
        catch (Exception e)
        {
            NoonUtility.LogException(e);
        }
    }

    void Start()
    {
    }

    void Update()
    {
        try
        {
            for (var i = 0; i < HotbarKeys.Length; i++)
            {
                var key = HotbarKeys[i];
                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    NoonUtility.Log($"Hotbar key {key} down");
                    var secondarySituations = Keyboard.current[Key.LeftShift].IsPressed() || Keyboard.current[Key.RightShift].IsPressed();

                    NoonUtility.Log($"Hotbar key {key} pressed, selecting situation {i} (secondary: {secondarySituations})");
                    this.SelectHotbarSituation(i, secondarySituations);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            NoonUtility.LogException(ex);
        }
    }

    void SelectHotbarSituation(int index, bool targetSecondarySituations)
    {
        if (Watchman.Get<Numa>().IsOtherworldActive())
        {
            return;
        }

        var hotbarSituations =
            from situation in this.GetAllSituations()
            where PrimarySituations.Contains(situation.VerbId) != targetSecondarySituations
            orderby situation.GetRectTransform().position.x
            select situation;

        var targetSituation = hotbarSituations.Skip(index).FirstOrDefault();
        if (targetSituation == null)
        {
            return;
        }

        if (targetSituation.IsOpen)
        {
            targetSituation.Close();
        }
        else
        {
            targetSituation.OpenAt(targetSituation.Token.Location);
        }
    }

    IEnumerable<Situation> GetAllSituations()
    {
        return Watchman.Get<HornedAxe>().GetRegisteredSituations();
    }
}