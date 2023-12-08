﻿using HarmonyLib;
using UnityEngine;

namespace AShortHike.Randomizer
{
    /// <summary>
    /// Display the key and value whenever something is being saved
    /// </summary>
    [HarmonyPatch(typeof(Tags), nameof(Tags.SetInt))]
    class Tags_SaveInt_Patch
    {
        public static void Postfix(string tag, int num)
        {
            Main.Log($"Saving int: {tag} ({num})");
        }
    }
    [HarmonyPatch(typeof(Tags), nameof(Tags.SetString))]
    class Tags_SaveString_Patch
    {
        public static void Postfix(string tag, string value)
        {
            Main.Log($"Saving string: {tag} ({value})");
        }
    }
    [HarmonyPatch(typeof(Tags), nameof(Tags.SetFloat))]
    class Tags_SaveFloat_Patch
    {
        public static void Postfix(string tag, float number)
        {
            Main.Log($"Saving float: {tag} ({number})");
        }
    }

    /// <summary>
    /// Sometimes the camera on outlook point throws an error and stops updating, so check for null renderers
    /// </summary>
    [HarmonyPatch(typeof(CustomCullZone), "UpdateRenderers")]
    class Camera_Update_Patch
    {
        public static bool Prefix(CustomCullZone __instance, bool show, ref bool ___shown)
        {
            if (show == ___shown)
                return false;

            foreach (Renderer r in __instance.renderers)
            {
                if (r != null)
                    r.enabled = show;
            }

            ___shown = show;
            return false;
        }
    }

    /// <summary>
    /// Main object is always destroyed so we will update the game through the player in the GameScene
    /// </summary>
    [HarmonyPatch(typeof(Player), "Update")]
    class Player_Update_Patch
    {
        public static void Postfix() => Main.Randomizer.UpdateGame();
    }

    //[HarmonyPatch(typeof(ItemMenuScroller), "Update")]
    //class MenuScroll_Update_Patch
    //{
    //    public static bool Prefix(LinearMenu ___menu) => ___menu.GetMenuObjects().Count > 0;
    //}
}
