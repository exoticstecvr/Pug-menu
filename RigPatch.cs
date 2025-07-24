using HarmonyLib;

[HarmonyPatch(typeof(VRRig), "OnDisable")]
public class RigPatch
{
    public static bool Prefix(VRRig __instance)
    {
        return __instance != GorillaTagger.Instance.offlineVRRig;
    }
}
