#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

using ConfigCat.Client;
using System.Threading.Tasks;

public static class Gmtk2024_config
{
    static IConfigCatClient client = ConfigCatClient.Get("configcat-sdk-1/BsvcCGai60GEs15OwW_Lcg/R4N-l7XxHEe4LxDUFO5KDg");

    public static async Task<bool> GetBoolValue(string key)
    {
        return await client.GetValueAsync(key, false);
    }

    public static async Task<string?> GetStringValue(string key)
    {
        string defaultString = "Default";
        string result = await client.GetValueAsync(key, defaultString);

        if (result == defaultString)
        {
            return null;
        }

        return result;
    }

    //    struct OrbitConfig
    //    {

    //        static var shared = OrbitConfig()


    //    private let client = ConfigCatClient.get(sdkKey: "LOL") { options in  // <-- This is the actual SDK Key for your 'Test Environment' environment.
    //        options.logLevel = .info // Set the log level to INFO to track how your feature flags were evaluated. When moving to production, you can remove this line to avoid too detailed logging.
    //    }

    //    func getBoolValue(forKey key: Keys) async -> Bool {
    //        return await client.getValue(for: key.rawValue, defaultValue: false)
    //    }
    //func getStringValue(forKey key: Keys) async->String ? {

    //    let defaultString = "Default"
    //        let result = await client.getValue(for: key.rawValue, defaultValue: defaultString)

    //        if (result == defaultString)
    //        {
    //            return nil
    //        }

    //    return result
    //    }

    //enum Keys : String
    //{
    //        case showLaunchAlert = "showLaunchAlert"
    //        case launchAlertTitle = "launchAlertTitle"
    //        case launchAlertMessage = "launchAlertMessage"
    //}
    //}

}
