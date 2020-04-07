using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ChatColors
{
    public static string End = "</color>";

    public static string Dev(string m)
    {
        m = "<color=#C70039>" + m + End;
        return m;
    }
    public static string Mod(string m)
    {
        m = "<color=#e68a00>" + m + End;
        return m;
    }
    public static string Admin(string m)
    {
        m = "<color=#f45942>" + m + End;
        return m;
    }
    public static string Player(string m)
    {
        m = "<color=#d6d6c2>" + m + End;
        return m;
    }
    public static string Premium(string m)
    {
        m = "<color=#007899>" + m + End;
        return m;
    }
    public static string Beta(string m)
    {
        m = "<color=#3b9900>" + m + End;
        return m;
    }

    public static string System(string m)
    {
        m = "<color=#455a5f>" + m + End;
        return m;
    }

    public static string Enemy(string m)
    {
        m = "<color=#FF0000>" + m + End; 
        return m;
    }

    public static string Friendly(string m)
    {
        m = "<color=#16d60f>" + m + End;
        return m;
    }
}