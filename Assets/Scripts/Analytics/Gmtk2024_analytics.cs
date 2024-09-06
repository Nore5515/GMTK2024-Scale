using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;
using System;
using System.Xml.Linq;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.LightingExplorerTableColumn;

public static class Gmtk2024_analytics
{
    public static void TrackPlayerDeath(string form, int maxHealth, bool isPoisoined, int currentLevel, string deathType = "unknown")
    {
        var props = new Value();
        props["$deathType"] = deathType;
        props["$form"] = form;
        props["$maxHealth"] = maxHealth;
        props["$isPoisoned"] = isPoisoined;
        props["$currentLevel"] = currentLevel;
        Mixpanel.Track("Player Death", props);
    }

    public static void TrackTest()
    {
        var props = new Value();
        props["$example"] = "EXAMPLE";
        Mixpanel.Track("Player Example", props);
    }
}


// Copyright (c) Nickolans Griffith

//import Mixpanel

//struct OrbitAnalytics
//{
//    static var shared = OrbitAnalytics()

//    func trackScreenPresented(_ screen: ScreenTypeKeys)
//    {
//        Mixpanel.mainInstance().track(event: Keys.screenPresented.rawValue, properties: [
//            "$screen": screen.rawValue
//        ])
//    }

//    func trackButtonClicked(_ button: ButtonTypeKeys)
//    {
//        Mixpanel.mainInstance().track(event: Keys.buttonClicked.rawValue, properties: [
//            "$button": button.rawValue
//        ])
//    }

//    func trackThemeSelected(_ theme: Int)
//    {
//        Mixpanel.mainInstance().track(event: Keys.theme.rawValue, properties: [
//            "$theme": theme
//        ])
//    }

//    private enum Keys : String
//    {
//        case screenPresented = "Screen Presented"
//        case buttonClicked = "Button Clicked"
//        case theme = "Theme"
//    }

//    enum ScreenTypeKeys : String
//    {
//        case home = "Home"...
//    }

//    enum ButtonTypeKeys : String
//    {
//        case profile = "Profile"
//        case deleteAccount = "Delete Account"
//        case manageSubscription = "Manage Subscription"
//        case termsOfService = "Terms of Service"
//        case privacyPolicy = "Privacy Policy"
//        case signOut = "Sign Out"
//        case signInWithGoogle = "Sign In With Google"
//        case signInWithApple = "Sign In With Apple"
//        case theme = "Theme"
//        case generateGoal = "Generate Goal"
//        case deleteGoal = "Delete Goal"
//        case saveGoal = "Save Goal"
//        case editGoal = "Edit Goal"
//    }
//}