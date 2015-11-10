using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

public static class BlockHelper
{
    #region LINQ Functions

    public static void ForEach(this IList<IMyTerminalBlock> blocks, Action<IMyTerminalBlock> action)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            action.Invoke(blocks[i]);
        }
    }

    public static bool All(this IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (!predicate.Invoke(blocks[i]))
                return false;
        }

        return true;
    }

    public static bool Any(this IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (predicate.Invoke(blocks[i]))
                return true;
        }

        return false;
    }

    public static IList<IMyTerminalBlock> Where(this IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
    {
        var result = new List<IMyTerminalBlock>();

        for (int i = 0; i < blocks.Count; i++)
        {
            if (predicate.Invoke(blocks[i]))
                result.Add(blocks[i]);
        }

        return result;
    }

    #endregion

    #region Block Finding (Find..)

    public static IList<IMyTerminalBlock> FindBlocksOfName(this IMyGridTerminalSystem gts, string name, Func<IMyTerminalBlock, bool> predicate = null)
    {
        var list = new List<IMyTerminalBlock>();
        gts.SearchBlocksOfName(name, list, predicate);
        return list;
    }

    public static IList<IMyTerminalBlock> FindBlocksOfType<T>(this IMyGridTerminalSystem gts, Func<IMyTerminalBlock, bool> predicate = null)
    {
        var list = new List<IMyTerminalBlock>();
        gts.GetBlocksOfType<T>(list, predicate);
        return list;
    }

    #endregion

    #region Block Actions (Do..)

    public static void DoAction(this IMyTerminalBlock block, string action)
    {
        var terminalAction = block.GetActionWithName(action);

        if (terminalAction == null)
            throw new Exception(string.Format("Block '{0}' has no action '{1}", block.CustomName, action));

        terminalAction.Apply(block);
    }

    public static void DoTurnOn(this IMyTerminalBlock block)
    {
        DoAction(block, "OnOff_On");
    }

    public static void DoTurnOff(this IMyTerminalBlock block)
    {
        DoAction(block, "OnOff_Off");
    }

    public static void DoToggleOnOff(this IMyTerminalBlock block)
    {
        DoAction(block, "OnOff");
    }

    public static void DoReverse(this IMyTerminalBlock block)
    {
        DoAction(block, "Reverse");
    }

    #endregion

    #region Block Conditions (Is..)

    public static bool IsWorking(this IMyTerminalBlock block)
    {
        return block.IsWorking;
    }

    public static bool IsNotWorking(this IMyTerminalBlock block)
    {
        return !block.IsWorking;
    }

    public static bool IsFunctional(this IMyTerminalBlock block)
    {
        return block.IsFunctional;
    }

    public static bool IsNotFunctional(this IMyTerminalBlock block)
    {
        return !block.IsFunctional;
    }

    public static bool IsBeingHacked(this IMyTerminalBlock block)
    {
        return block.IsBeingHacked;
    }

    #region Piston

    public static bool IsPiston(this IMyTerminalBlock block)
    {
        return (block is IMyPistonBase);
    }

    public static bool IsPistonExpanded(this IMyTerminalBlock block)
    {
        var piston = AsPiston(block);

        // have to use '.ToString()' because the rounded value don't exactly match to the ui value
        return (Math.Round(piston.MaxLimit, 1).ToString() == GetPistonPosition(block).ToString());
    }

    public static bool IsPistonContracted(this IMyTerminalBlock block)
    {
        var piston = AsPiston(block);

        // have to use '.ToString()' because the rounded value don't exactly match to the ui value
        return (Math.Round(piston.MinLimit, 1).ToString() == GetPistonPosition(block).ToString());
    }

    #endregion

    #region Sensor

    public static bool IsSensor(this IMyTerminalBlock block)
    {
        return (block is IMySensorBlock);
    }

    public static bool IsSensorActive(this IMyTerminalBlock block)
    {
        var sensor = AsSensor(block);

        return sensor.IsActive;
    }

    #endregion

    #region Landing Gear

    public static bool IsLandingGear(this IMyTerminalBlock block)
    {
        return (block is IMyLandingGear);
    }

    public static bool IsLandingGearLocked(this IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Locked");
    }

    public static bool IsLandingGearUnlocked(this IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Unlocked");
    }

    public static bool IsLandingGearReadyToLock(this IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Ready To Lock");
    }

    #endregion

    #region Motor

    public static bool IsMotorReachedUpperLimit(this IMyTerminalBlock block)
    {
        var rotor = AsMotorStator(block);

        return (GetMotorPosition(block) >= rotor.UpperLimit * 180 / Math.PI);
    }

    public static bool IsMotorReachedLowerLimit(this IMyTerminalBlock block)
    {
        var rotor = AsMotorStator(block);

        return (GetMotorPosition(block) <= rotor.LowerLimit * 180 / Math.PI);
    }

    #endregion

    #endregion

    #region Block Properties (Get..)

    public static IList<ITerminalProperty> GetProperties(this IMyTerminalBlock block, Func<ITerminalProperty, bool> predicate = null)
    {
        var list = new List<ITerminalProperty>();
        block.GetProperties(list, predicate);
        return list;
    }

    public static float? GetPistonPosition(this IMyTerminalBlock block)
    {
        return TryExtractFloat(block.DetailedInfo, @"(\d+.\d+)m");
    }

    public static float? GetMotorPosition(this IMyTerminalBlock block)
    {
        return TryExtractFloat(block.DetailedInfo, @"(-?\d+)°");
    }

    #endregion

    #region Block Cast (As..)

    public static IMySensorBlock AsSensor(this IMyTerminalBlock block)
    {
        var target = block as IMySensorBlock;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Sensor'", block.CustomName));

        return target;
    }

    public static IMyPistonBase AsPiston(this IMyTerminalBlock block)
    {
        var target = block as IMyPistonBase;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Piston'", block.CustomName));

        return target;
    }

    public static IMyLandingGear AsLandingGear(this IMyTerminalBlock block)
    {
        var target = block as IMyLandingGear;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Landing Gear'", block.CustomName));

        return target;
    }

    public static IMyMotorStator AsMotorStator(this IMyTerminalBlock block)
    {
        var target = block as IMyMotorStator;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Motor Stator'", block.CustomName));

        return target;
    }

    public static IMyLightingBlock AsLightingBlock(this IMyTerminalBlock block)
    {
        var target = block as IMyLightingBlock;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Lighting Block'", block.CustomName));

        return target;
    }

    #endregion

    #region Internal

    private static float? TryExtractFloat(string value, string pattern)
    {
        var matches = System.Text.RegularExpressions.Regex.Match(value, pattern);

        if (!matches.Groups[1].Success)
            return null;

        return float.Parse(matches.Groups[1].Value);
    }

    #endregion
}

