using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
    public static T[] Append<T>(this T[] array, T item)
    {
        if (array == null)
        {
            return new T[] { item };
        }
        T[] result = new T[array.Length + 1];
        array.CopyTo(result, 0);
        result[array.Length] = item;
        return result;
    }
    public static float CalculateStat(float baseValue, float valueMultiplier, float flat, float level) 
    {
        return (baseValue + flat) * ((1 + 0.08f * level) * valueMultiplier / 100);
    }

    public static int GetLevelFromXP(int xp)
    {
        int currentExp = xp;

        int levelPerExp = 500;

        int currentLevel = (int)Math.Floor(currentExp / ((float)levelPerExp));

        return currentLevel;
    }

    public static int GetXPRemaining(int xp)
    {
        int currentExp = xp;

        int levelPerExp = 500;

        int expProgressToNextLevel = currentExp % levelPerExp;

        int expUntilNextLevel = levelPerExp - expProgressToNextLevel;

        return expUntilNextLevel;
    }
    public static float ToPercent(this float value) => (1 + value / 100);
    public static int ToPercent(this int value) => (1 + value / 100);

    public static float ToFloat(this string value) => float.Parse(value);

    public static int ToInt(this string value) => int.Parse(value);

    public static bool ToBool(this string value) => bool.Parse(value);
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    public static void SetTransparency(this UnityEngine.UI.Image p_image, float p_transparency)
    {
        if (p_image != null)
        {
            UnityEngine.Color __alpha = p_image.color;
            __alpha.a = p_transparency;
            p_image.color = __alpha;
        }
    }

    public static bool IsNanOrInfinity(this float value)
    {
        return float.IsNaN(value) || float.IsInfinity(value);
    }
}
