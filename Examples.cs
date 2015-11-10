using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

class Examples
{
    IMyGridTerminalSystem GridTerminalSystem;
    string Storage;

    void Main()
    {
        var debug = GridTerminalSystem.FindBlocksOfName("Debug")[0];

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
        //var hackingBlocks = GridTerminalSystem.block.Blocks.Where(BlockHelper.IsBeingHacked);


        //
        var light = lights[0];
        var properties = piston.GetProperties();
        var output = "";

        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            output += string.Format("{0}:{1}; ", property.Id, property.TypeName);
        }

        var color = light.GetProperty("Color").AsColor().GetValue(light);

        debug.SetCustomName(string.Format("Debug: {0} {1}", output, color));
        
    }
}

