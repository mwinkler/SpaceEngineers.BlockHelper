SpaceEngineers.BlockHelper (WIP)
================================

The block helper class allow you to write more expressiv and compact code for Space Engineers programming block.
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
```
    void Main()
    {
        var block = new BlockHelper(GridTerminalSystem);



        // get all lights
        var lights = block.FindBlocksOfType<IMyLightingBlock>();

        // is any light on
        if (block.Any(lights, block.IsWorking))
        {
            // turn all lights off
            block.ForEach(lights, block.DoTurnOff);
        }




        // get all lights witch are turned off
        var offLights = block.FindBlocksOfType<IMyLightingBlock>(block.IsNotWorking);




        // reverse if piston is fully expanded
        var piston = GridTerminalSystem.GetBlockWithName("Piston");

        if (block.IsPistonExpanded(piston))
            block.DoReverse(piston);




        // stop rotor if exceed specific angle
        var rotor = GridTerminalSystem.GetBlockWithName("Rotor");

        if (block.GetRotorPosition(rotor) > 90)
            block.DoTurnOff(rotor);



            
        // get all being hacked blocks
        var hackingBlocks = block.Where(GridTerminalSystem.Blocks, block.IsBeingHacked);

    }
```

At the moment only a few properties, conditions and methods are implemented:

```
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
GetRotorPosition(IMyTerminalBlock): float? 

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
IsSensor(IMyTerminalBlock): bool 
IsSensorActive(IMyTerminalBlock): bool 
IsWorking(IMyTerminalBlock): bool
```

More to come...
