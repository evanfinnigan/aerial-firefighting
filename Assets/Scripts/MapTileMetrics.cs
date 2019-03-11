using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTileMetrics {

    public const float outerWidth = 40f;
    public const float innerWidth = 20f;

    public const float oceanElevation = 0.3f;

    public const float elevationMin = 0f;
    public const float elevationMax = 20f;

    public static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerWidth),
        new Vector3(outerWidth, 0f, outerWidth),
        new Vector3(outerWidth, 0f, 0f),
        new Vector3(0f, 0f, 0f)
    };

    public enum Direction
    {
        N, E, S, W
    }

    public static Direction Opposite (this Direction direction)
    {
        return (int)direction < 2 ? direction + 2 : direction - 2;
    }

}
