using FrankenToilet.Core;
using FrankenToilet.triggeredidiot;
using HarmonyLib;
using System;
using UnityEngine;

namespace FrankenToilet.duviz;

public class HealthRemover : MonoBehaviour
{
    public static float percentage;

    public void Update()
    {
        if (NewMovement.instance == null) { percentage = 0; return; }
        if (!NewMovement.instance.activated) { percentage = 0; return; }
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(HealthBar), "Update")]
public class HealthPatch
{
    [HarmonyPostfix]
    public static void Postfix(HealthBar __instance)
    {
        if (__instance.hpText != null)
        {
            __instance.hpText.text = $"{__instance.hp.ToString("F0")} // {HealthRemover.percentage.ToString("F2")}%";
        }
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "GetHurt")]
public class NewMovementHurtPatch
{
    [HarmonyPrefix]
    public static void Prefix(NewMovement __instance, ref int damage)
    {
        HealthRemover.percentage = Mathf.Min(damage + UnityEngine.Random.Range(0, 1f) + HealthRemover.percentage, 5000) * 1.5f;
        damage *= (int)MathF.Min(HealthRemover.percentage / 75 + 1, 100000);
        if (HealthRemover.percentage > 1000 && damage < 10000)
            damage = 0;
    }
    [HarmonyPostfix]
    public static void Post(NewMovement __instance)
    {
        NewMovement.instance.rb.velocity += new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * HealthRemover.percentage; ;

        if (HealthRemover.percentage > 1000 && !__instance.dead)
        {
            TimeController.instance.ParryFlash();
            DeltaruneExplosion.ExplodePlayer();
        }
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "Respawn")]
public class NewMovementRespawnPatch
{
    [HarmonyPostfix]
    public static void Post(NewMovement __instance)
    {
        __instance.hp *= 5;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "Start")]
public class NewMovementStartPatch
{
    [HarmonyPostfix]
    public static void Post(NewMovement __instance)
    {
        __instance.hp *= 5;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "GetHealth")]
public class NewMovementGetHealthPatch
{
    [HarmonyPrefix]
    public static bool Prefix(NewMovement __instance, int health, bool silent, bool fromExplosion = false, bool bloodsplatter = true)
    {
        if (!__instance.dead && (!__instance.exploded || !fromExplosion))
        {
            float num = (float)health;
            float num2 = 500f;
            if (num < 1f)
                num = 1f;
            if ((float)__instance.hp <= num2)
            {
                if ((float)__instance.hp + num < num2 - (float)Mathf.RoundToInt(__instance.antiHp))
                    __instance.hp += Mathf.RoundToInt(num);
                else if ((float)__instance.hp != num2 - (float)Mathf.RoundToInt(__instance.antiHp))
                    __instance.hp = Mathf.RoundToInt(num2) - Mathf.RoundToInt(__instance.antiHp);
                __instance.hpFlash.Flash(1f);
                if (!silent && health > 5)
                {
                    if (__instance.greenHpAud == null)
                        __instance.greenHpAud = __instance.hpFlash.GetComponent<AudioSource>();
                    __instance.greenHpAud.Play();
                }
            }
            if (!silent && health > 5 && MonoSingleton<PrefsManager>.Instance.GetBoolLocal("bloodEnabled", false))
                GameObject.Instantiate<GameObject>(__instance.scrnBlood, __instance.fullHud.transform);
        }
        return false;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "FullHeal")]
public class NewMovementFullHealPatch
{
    [HarmonyPrefix]
    public static bool Prefix(NewMovement __instance, bool silent)
    {
        __instance.GetHealth(1000, silent, false, false);
        return false;
    }
}