
public class BlockHelper
{
    #region Members

    private static IMyGridTerminalSystem _gts;
    private static string _debugBlockName;
    private static IMyTerminalBlock _debugBlock;
    private static readonly System.Text.RegularExpressions.Regex _infoPistionPosition = new System.Text.RegularExpressions.Regex(@"(\d+.\d+)m");
    private static readonly System.Text.RegularExpressions.Regex _infoMotorPosition = new System.Text.RegularExpressions.Regex(@"(-?\d+)°");

    #endregion

    #region Init

    public static void Init(IMyGridTerminalSystem gridTerminalSystem, string debugBlockName = null)
    {
        _gts = gridTerminalSystem;

        if (!string.IsNullOrEmpty(debugBlockName))
        {
            _debugBlockName = debugBlockName;
            _debugBlock = _gts.GetBlockWithName(debugBlockName);
        }
    }

    #endregion

    #region LINQ Functions

    public static void ForEach(IList<IMyTerminalBlock> blocks, Action<IMyTerminalBlock> action)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            action.Invoke(blocks[i]);
        }
    }

    public static bool All(IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (!predicate.Invoke(blocks[i]))
                return false;
        }

        return true;
    }

    public static bool Any(IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (predicate.Invoke(blocks[i]))
                return true;
        }

        return false;
    }

    public static IList<IMyTerminalBlock> Where(IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
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

    #region Debug

    public static void Debug(string text)
    {
        if (_debugBlock == null)
            throw new Exception("Deubg block was not found! You have to specifiy the debug block name when you initialize the block helper");

        _debugBlock.SetCustomName(_debugBlock.CustomName + "\n\r" + text);
    }

    public static void DebugClear()
    {
        if (_debugBlock != null)
        {
            _debugBlock.SetCustomName(_debugBlockName);
        }
    }

    public static void Debug(string text, params object[] args)
    {
        if (_debugBlock == null)
            throw new Exception("Deubg block was not found! You have to specifiy the debug block name when you initialize the block helper");

        _debugBlock.SetCustomName(_debugBlock.CustomName + "\n\r" + string.Format(text, args));
    }

    #endregion

    #region Block Finding (Find..)

    public static IList<IMyTerminalBlock> FindBlocksOfName(string name, Func<IMyTerminalBlock, bool> predicate = null)
    {
        var list = new List<IMyTerminalBlock>();
        _gts.SearchBlocksOfName(name, list, predicate);
        return list;
    }

    public static IList<IMyTerminalBlock> FindBlocksOfType<T>(Func<IMyTerminalBlock, bool> predicate = null)
    {
        var list = new List<IMyTerminalBlock>();
        _gts.GetBlocksOfType<T>(list, predicate);
        return list;
    }

    #endregion

    #region Block Actions (Do..)

    public static void DoAction(IMyTerminalBlock block, string action)
    {
        var terminalAction = block.GetActionWithName(action);

        if (terminalAction == null)
            throw new Exception(string.Format("Block '{0}' has no action '{1}", block.CustomName, action));

        terminalAction.Apply(block);
    }

    public static void DoTurnOn(IMyTerminalBlock block)
    {
        DoAction(block, "OnOff_On");
    }

    public static void DoTurnOff(IMyTerminalBlock block)
    {
        DoAction(block, "OnOff_Off");
    }

    public static void DoToggleOnOff(IMyTerminalBlock block)
    {
        DoAction(block, "OnOff");
    }

    public static void DoReverse(IMyTerminalBlock block)
    {
        DoAction(block, "Reverse");
    }

    #endregion

    #region Block Conditions (Is..)

    public static bool IsWorking(IMyTerminalBlock block)
    {
        return block.IsWorking;
    }

    public static bool IsNotWorking(IMyTerminalBlock block)
    {
        return !block.IsWorking;
    }

    public static bool IsFunctional(IMyTerminalBlock block)
    {
        return block.IsFunctional;
    }

    public static bool IsNotFunctional(IMyTerminalBlock block)
    {
        return !block.IsFunctional;
    }

    public static bool IsBeingHacked(IMyTerminalBlock block)
    {
        return block.IsBeingHacked;
    }

    #region Piston

    public static bool IsPiston(IMyTerminalBlock block)
    {
        return (block is IMyPistonBase);
    }

    public static bool IsPistonExpanded(IMyTerminalBlock block)
    {
        var piston = AsPiston(block);

        return (Math.Round(piston.MaxLimit, 1).ToString() == GetPistonPosition(block).ToString());
    }

    public static bool IsPistonContracted(IMyTerminalBlock block)
    {
        var piston = AsPiston(block);

        return (Math.Round(piston.MinLimit, 1).ToString() == GetPistonPosition(block).ToString());
    }

    #endregion

    #region Sensor

    public static bool IsSensor(IMyTerminalBlock block)
    {
        return (block is IMySensorBlock);
    }

    public static bool IsSensorActive(IMyTerminalBlock block)
    {
        var sensor = AsSensor(block);

        return sensor.IsActive;
    }

    #endregion

    #region Landing Gear

    public static bool IsLandingGear(IMyTerminalBlock block)
    {
        return (block is IMyLandingGear);
    }

    public static bool IsLandingGearLocked(IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Locked");
    }

    public static bool IsLandingGearUnlocked(IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Unlocked");
    }

    public static bool IsLandingGearReadyToLock(IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Ready To Lock");
    }

    #endregion

    #region Motor

    public static bool IsMotorAtUpperLimit(IMyTerminalBlock block)
    {
        var rotor = AsMotorStator(block);

        return (GetMotorPosition(block) >= rotor.UpperLimit * 180 / Math.PI);
    }

    public static bool IsMotorAtLowerLimit(IMyTerminalBlock block)
    {
        var rotor = AsMotorStator(block);

        return (GetMotorPosition(block) <= rotor.LowerLimit * 180 / Math.PI);
    }

    #endregion

    #endregion

    #region Block Properties (Get..)

    public static float? GetPistonPosition(IMyTerminalBlock block)
    {
        return TryExtractFloat(block.DetailedInfo, _infoPistionPosition);
    }

    public static float? GetMotorPosition(IMyTerminalBlock block)
    {
        return TryExtractFloat(block.DetailedInfo, _infoMotorPosition);
    }

    #endregion

    #region Block Cast (As..)

    public static IMySensorBlock AsSensor(IMyTerminalBlock block)
    {
        var target = block as IMySensorBlock;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Sensor'", block.CustomName));

        return target;
    }

    public static IMyPistonBase AsPiston(IMyTerminalBlock block)
    {
        var target = block as IMyPistonBase;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Piston'", block.CustomName));

        return target;
    }

    public static IMyLandingGear AsLandingGear(IMyTerminalBlock block)
    {
        var target = block as IMyLandingGear;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Landing Gear'", block.CustomName));

        return target;
    }

    public static IMyMotorStator AsMotorStator(IMyTerminalBlock block)
    {
        var target = block as IMyMotorStator;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Motor Stator'", block.CustomName));

        return target;
    }

    #endregion

    #region Internal

    private static float? TryExtractFloat(string value, System.Text.RegularExpressions.Regex expression)
    {
        var matches = expression.Match(value);

        if (!matches.Groups[1].Success)
            return null;

        return float.Parse(matches.Groups[1].Value);
    }

    #endregion
}