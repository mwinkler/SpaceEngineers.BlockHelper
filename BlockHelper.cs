
public class BlockHelper
{
    #region Members

    private readonly IMyGridTerminalSystem _gts;
    private readonly string _debugBlockName;
    private readonly IMyTerminalBlock _debugBlock;
    private readonly System.Text.RegularExpressions.Regex _infoPistionPosition = new System.Text.RegularExpressions.Regex(@"(\d+.\d+)m");
    private readonly System.Text.RegularExpressions.Regex _infoRotorPosition = new System.Text.RegularExpressions.Regex(@"(\d+)°");

    #endregion

    public BlockHelper(IMyGridTerminalSystem gts, string debugBlockName = null)
    {
        _gts = gts;

        if (!string.IsNullOrEmpty(debugBlockName))
        {
            _debugBlockName = debugBlockName;
            _debugBlock = gts.GetBlockWithName(debugBlockName);
        }
    }

    #region LINQ Functions

    public void ForEach(IList<IMyTerminalBlock> blocks, Action<IMyTerminalBlock> action)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            action.Invoke(blocks[i]);
        }
    }

    public bool All(IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (!predicate.Invoke(blocks[i]))
                return false;
        }

        return true;
    }

    public bool Any(IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (predicate.Invoke(blocks[i]))
                return true;
        }

        return false;
    }

    public IList<IMyTerminalBlock> Where(IList<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> predicate)
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

    public void Debug(string text)
    {
        Debug(text, null);
    }

    public void DebugClear()
    {
        if (_debugBlock != null)
        {
            _debugBlock.SetCustomName(_debugBlockName);
        }
    }

    public void Debug(string text, params object[] args)
    {
        if (_debugBlock == null)
            throw new Exception("Deubg block was not found! You have to specifiy the debug block name when you initialize the block helper");
            
        _debugBlock.SetCustomName(_debugBlock.CustomName + "\n\r" + string.Format(text, args));
    }

    #endregion

    #region Block Finding (Find..)

    public IList<IMyTerminalBlock> FindBlocksOfName(string name, Func<IMyTerminalBlock, bool> predicate = null)
    {
        var list = new List<IMyTerminalBlock>();
        _gts.SearchBlocksOfName(name, list, predicate);
        return list;
    }

    public IList<IMyTerminalBlock> FindBlocksOfType<T>(Func<IMyTerminalBlock, bool> predicate = null)
    {
        var list = new List<IMyTerminalBlock>();
        _gts.GetBlocksOfType<T>(list, predicate);
        return list;
    }

    #endregion

    #region Block Actions (Do..)

    public void DoAction(IMyTerminalBlock block, string action)
    {
        var terminalAction = block.GetActionWithName(action);

        if (terminalAction == null)
            throw new Exception(string.Format("Block '{0}' has no action '{1}", block.CustomName, action));

        terminalAction.Apply(block);
    }

    public void DoTurnOn(IMyTerminalBlock block)
    {
        DoAction(block, "OnOff_On");
    }

    public void DoTurnOff(IMyTerminalBlock block)
    {
        DoAction(block, "OnOff_Off");
    }

    public void DoToggleOnOff(IMyTerminalBlock block)
    {
        DoAction(block, "OnOff");
    }

    public void DoReverse(IMyTerminalBlock block)
    {
        DoAction(block, "Reverse");
    }

    #endregion

    #region Block Conditions (Is..)

    public bool IsWorking(IMyTerminalBlock block)
    {
        return block.IsWorking;
    }

    public bool IsNotWorking(IMyTerminalBlock block)
    {
        return !block.IsWorking;
    }

    public bool IsFunctional(IMyTerminalBlock block)
    {
        return block.IsFunctional;
    }

    public bool IsNotFunctional(IMyTerminalBlock block)
    {
        return !block.IsFunctional;
    }

    public bool IsBeingHacked(IMyTerminalBlock block)
    {
        return block.IsBeingHacked;
    }

    #region Piston

    public bool IsPiston(IMyTerminalBlock block)
    {
        return (block is IMyPistonBase);
    }

    public bool IsPistonExpanded(IMyTerminalBlock block)
    {
        var piston = AsPiston(block);

        return (Math.Round(piston.MaxLimit, 1).ToString() == GetPistonPosition(block).ToString());
    }

    public bool IsPistonContracted(IMyTerminalBlock block)
    {
        var piston = AsPiston(block);

        return (Math.Round(piston.MinLimit, 1).ToString() == GetPistonPosition(block).ToString());
    }

    #endregion

    #region Sensor

    public bool IsSensor(IMyTerminalBlock block)
    {
        return (block is IMySensorBlock);
    }

    public bool IsSensorActive(IMyTerminalBlock block)
    {
        var sensor = AsSensor(block);

        return sensor.IsActive;
    }

    #endregion

    #region Landing Gear

    public bool IsLandingGear(IMyTerminalBlock block)
    {
        return (block is IMyLandingGear);
    }

    public bool IsLandingGearLocked(IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Locked");
    }

    public bool IsLandingGearUnlocked(IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Unlocked");
    }

    public bool IsLandingGearReadyToLock(IMyTerminalBlock block)
    {
        var gear = AsLandingGear(block);

        return gear.DetailedInfo.Contains("Ready To Lock");
    }

    #endregion


    #endregion

    #region Block Properties (Get..)

    public float? GetPistonPosition(IMyTerminalBlock block)
    {
        return TryExtractFloat(block.DetailedInfo, _infoPistionPosition);
    }

    public float? GetRotorPosition(IMyTerminalBlock block)
    {
        return TryExtractFloat(block.DetailedInfo, _infoRotorPosition);
    }

    #endregion

    #region Block Cast (As..)

    public IMySensorBlock AsSensor(IMyTerminalBlock block)
    {
        var target = block as IMySensorBlock;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Sensor'", block.CustomName));

        return target;
    }

    public IMyPistonBase AsPiston(IMyTerminalBlock block)
    {
        var target = block as IMyPistonBase;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Piston'", block.CustomName));

        return target;
    }

    public IMyLandingGear AsLandingGear(IMyTerminalBlock block)
    {
        var target = block as IMyLandingGear;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Landing Gear'", block.CustomName));

        return target;
    }

    public IMyMotorStator AsMotorStator(IMyTerminalBlock block)
    {
        var target = block as IMyMotorStator;

        if (target == null)
            throw new Exception(string.Format("Block '{0}' is no 'Motor Stator'", block.CustomName));

        return target;
    }

    #endregion

    #region Internal

    private float? TryExtractFloat(string value, System.Text.RegularExpressions.Regex expression)
    {
        var matches = expression.Match(value);

        if (!matches.Groups[1].Success)
            return null;

        return float.Parse(matches.Groups[1].Value);
    }

    #endregion
} 

