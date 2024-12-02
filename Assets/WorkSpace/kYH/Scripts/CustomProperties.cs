using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperties
{
    private static PhotonHashtable customProperty = new PhotonHashtable();

    public const string READY = "Ready";

    public static void SetReady(this Player player, bool ready)
    {
        customProperty.Clear();
        customProperty[READY] = ready;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetReady(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(READY))
        {
            Debug.Log("제발 되라 좀!!!!!!!!!!!!!!!!!");
            return (bool)customProperty[READY];
        }
        else//
        {
            Debug.Log("왜 안되냐고!!!!!!!!!!!!!!!!!");
            return false;
        }
    }

    public static bool ReadyCheck(Player player)
    {
        if (player.CustomProperties.TryGetValue(READY, out object isReady))
        {
            return (bool)isReady;
        }
        return false;
    }

    public const string LOAD = "Load";

    public static void SetLoad(this Player player, bool load)
    {
        customProperty.Clear();
        customProperty[LOAD] = load;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetLoad(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(LOAD))
        {
            return (bool)customProperty[LOAD];
        }
        else
        {
            return false;
        }
    }

    public const string ALIVE = "alive";

    public static void SetAlive(this Player player, bool alive)
    {
        customProperty.Clear();
        customProperty[ALIVE] = alive;
        player.SetCustomProperties(customProperty);
    }

    public static bool GetAlive(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(ALIVE))
        {
            return (bool)customProperty[ALIVE];
        }
        else
        {
            return false;
        }
    }

    //룸 프로퍼티

    public const string STAGE = "Stage";

    public static void SetStage(this Room room, StageData.StageType stage)
    {
        customProperty.Clear();
        customProperty[STAGE] = (int)stage;
        room.SetCustomProperties(customProperty);
    }

    public static StageData.StageType GetStage(this Room room)
    {
        PhotonHashtable customProperty = room.CustomProperties;
        if (customProperty.ContainsKey(STAGE))
        {
            return (StageData.StageType)(int)customProperty[STAGE];
        }
        else
        {
            return (StageData.StageType)(-1);
        }
    }

}
