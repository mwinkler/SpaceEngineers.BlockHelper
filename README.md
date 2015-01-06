SpaceEngineers.BlockHelper (WIP)
================================

The block helper class allow you to write more expressiv and compact code for Space Engineers programming BlockHelper.
It contains methods for finding blocks, executing actions, get properties, check constrains and more.

The methods are group by:
- Find..() : Find block
- Do..() : Do action
- Is..() : Check condition
- As..() : Cast (with check)
- Get..() : Get property from block
- Debug..() : Output for debugging

And some LINQ like functions:
- ForEach() : Do something on all blocks
- Any() : Match any block a condition
- All() : Match all blocks the condition
- Where() : Filter blocks by condition

Some usage examples:
```csharp
    void Main()
    {
        BlockHelper.Init(GridTerminalSystem);



        // get all lights
        var lights = BlockHelper.FindBlocksOfType<IMyLightingBlock>();

        // is any light on
        if (BlockHelper.Any(lights, BlockHelper.IsWorking))
        {
            // turn all lights off
            BlockHelper.ForEach(lights, BlockHelper.DoTurnOff);
        }




        // get all lights witch are turned off
        var offLights = BlockHelper.FindBlocksOfType<IMyLightingBlock>(BlockHelper.IsNotWorking);




        // reverse if piston is fully expanded
        var piston = GridTerminalSystem.GetBlockWithName("Piston");

        if (BlockHelper.IsPistonExpanded(piston))
            BlockHelper.DoReverse(piston);




        // stop rotor if exceed specific angle
        var rotor = GridTerminalSystem.GetBlockWithName("Rotor");

        if (BlockHelper.GetRotorPosition(rotor) > 90)
            BlockHelper.DoTurnOff(rotor);



            
        // get all being hacked blocks
        var hackingBlocks = BlockHelper.Where(GridTerminalSystem.Blocks, BlockHelper.IsBeingHacked);

    }
```

At the moment only a few properties, conditions and methods are implemented:

```csharp
All(IList<IMyTerminalBlock>, Func<IMyTerminalBlock,bool>): bool 
Any(IList<IMyTerminalBlock>, Func<IMyTerminalBlock,bool>): bool 
ForEach(IList<IMyTerminalBlock>, Action<IMyTerminalBlock>) 
Where(IList<IMyTerminalBlock>, Func<IMyTerminalBlock,bool>): IList<IMyTerminalBlock> 

FindBlocksOfName(string, [Func<IMyTerminalBlock,bool>]): IList<IMyTerminalBlock> 
FindBlocksOfType<T>([Func<IMyTerminalBlock,bool>]): IList<IMyTerminalBlock> 

AsLandingGear(IMyTerminalBlock): IMyLandingGear 
AsMotorStator(IMyTerminalBlock): IMyMotorStator
AsPiston(IMyTerminalBlock): IMyPistonBase 
AsSensor(IMyTerminalBlock): IMySensorBlock 

Debug(string, params object[]) 
DebugClear() 

DoAction(IMyTerminalBlock, string) 
DoReverse(IMyTerminalBlock) 
DoToggleOnOff(IMyTerminalBlock) 
DoTurnOff(IMyTerminalBlock) 
DoTurnOn(IMyTerminalBlock) 

GetPistonPosition(IMyTerminalBlock): float? 
GetMotorPosition(IMyTerminalBlock): float? 

IsBeingHacked(IMyTerminalBlock): bool 
IsFunctional(IMyTerminalBlock): bool 
IsLandingGear(IMyTerminalBlock): bool 
IsLandingGearLocked(IMyTerminalBlock): bool 
IsLandingGearReadyToLock(IMyTerminalBlock): bool 
IsLandingGearUnlocked(IMyTerminalBlock): bool 
IsNotFunctional(IMyTerminalBlock): bool 
IsNotWorking(IMyTerminalBlock): bool 
IsPiston(IMyTerminalBlock): bool 
IsPistonContracted(IMyTerminalBlock): bool 
IsPistonExpanded(IMyTerminalBlock): bool 
IsMotorReachedUpperLimit(IMyTerminalBlock): bool 
IsMotorReachedLowerLimit(IMyTerminalBlock): bool 
IsSensor(IMyTerminalBlock): bool 
IsSensorActive(IMyTerminalBlock): bool 
IsWorking(IMyTerminalBlock): bool
```

More to come...
