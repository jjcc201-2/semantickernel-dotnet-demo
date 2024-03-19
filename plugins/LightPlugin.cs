using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins;

public class LightPlugin
{
    public bool IsOn { get; set;} = false;

    [KernelFunction]
    [Description("Get the state of the light.")]
    public string GetState() => IsOn ? "on" : "off";

    [KernelFunction]
    [Description("Turn on the light.")]
    public string TurnOn()
    {
        IsOn = true;

        //Console.ForegroundColor = ConsoleColor.Yellow;
        Console.BackgroundColor = ConsoleColor.Yellow;

        return "on";
    }

    [KernelFunction]
    [Description("Turn off the light.")]
    public string TurnOff()
    {
        IsOn = false;

        Console.BackgroundColor = ConsoleColor.Black;

        return "off";
    }
}