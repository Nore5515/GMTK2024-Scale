using System;
using System.Collections.Generic;

public static class GameState
{
    public static int hp { get; set; } = 3;
    public static int maxHP { get; set; } = 3;

    public static bool IsPoisoned { get; set; } = false;

    public static bool EyesOpen { get; set; } = false;

    public static Dictionary<string, bool> formState = new();

    public static void SetFormState(string form, bool newState)
    {
        formState[form] = newState;
    }

    public static bool CheckForm(string form)
    {
        if (formState.ContainsKey(form))
        {
            return formState[form];
        }
        else
        {
            formState.Add(form, false);
            return formState[form];
        }
    }
}
