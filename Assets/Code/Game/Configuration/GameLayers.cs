using System;

namespace Chroma.Game.Configuration
{
    [Flags]
    public enum GameLayers
    {
        PostProcessing = 256,   //  8
        Terrain = 512,          //  9
        Characters = 1024,      // 10
        Objects = 2048,         // 11
        Buildings = 4096        // 12
    }
}
