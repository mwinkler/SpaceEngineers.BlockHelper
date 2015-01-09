using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;


class Examples
{
    IMyGridTerminalSystem GridTerminalSystem;

    void Main()
    {
        var debug = GridTerminalSystem.GetBlockWithName("Debug:");

        // get all lights
        var lights = GridTerminalSystem.FindBlocksOfType<IMyLightingBlock>();

        // is any light on
        if (lights.Any(BlockHelper.IsWorking))
        {
            // turn all lights off
            lights.ForEach(BlockHelper.DoTurnOff);
        }



        // get all lights witch are turned off
        var offLights = GridTerminalSystem.FindBlocksOfType<IMyLightingBlock>(BlockHelper.IsNotWorking);



        // reverse if piston is fully expanded
        var piston = GridTerminalSystem.GetBlockWithName("Piston");

        if (piston.IsPistonExpanded() || piston.IsPistonContracted())
            piston.DoReverse();



        // stop rotor if exceed specific angle
        var rotor = GridTerminalSystem.GetBlockWithName("Rotor");

        if (rotor.GetMotorPosition() > 90 || rotor.GetMotorPosition() < 10)
            rotor.DoReverse();



        // get all being hacked blocks
        var hackingBlocks = GridTerminalSystem.Blocks.Where(BlockHelper.IsBeingHacked);



        // get piston position
    }
}

