using UnityEngine;

public static class Vector_ExtentionMethods
{
    public static Vector3 Floor(this Vector3 value)
    {
        value.x = Mathf.Floor(value.x);
        value.y = Mathf.Floor(value.y);
        value.z = Mathf.Floor(value.z);

        return value;
    }

    public static Vector3 Ceil(this Vector3 value)
    {
        value.x = Mathf.Ceil(value.x);
        value.y = Mathf.Ceil(value.y);
        value.z = Mathf.Ceil(value.z);

        return value;
    }

    public static Vector3 Round(this Vector3 value)
    {
        value.x = Mathf.Round(value.x);
        value.y = Mathf.Round(value.y);
        value.z = Mathf.Round(value.z);

        return value;
    }

    public static Vector3 Round(this Vector3 value, float roundValue)
    {
        value /= roundValue;
        value = value.Round();
        value *= roundValue;

        return value;
    }

    public static string ToFancyString(this Vector3 value)
    {
        string ret = string.Empty;
        ret = $"X: {value.x}, Y: {value.y}, Z: {value.z}";
        return ret;
    }
}
