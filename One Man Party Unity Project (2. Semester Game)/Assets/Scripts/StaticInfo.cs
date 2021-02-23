using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticInfo
{
    //player
    public static bool loadPlayerInfo = false;
    public static Vector3 playerPos;
    public static float playerHelath = 1;

    //scene
    public static int currentOverworldSceneIdx;
    public static AudioClip previousMusic;

    //enemy
    public static OverworldEnemy enemyCurrentlyFighting;

    public static Race[] enemyCurrentlyFightingRace;
    //public static int enemyCurrentlyFightingHealth;
    //public static int enemyCurrentlyFightingArmor;
    //public static int enemyCurrentlyFightingIdx;
    //public static int enemyCurrentlyFightingLevel;
    //public static Job enemyCurrentlyFightingClass;

    //list of all placed enemies
    public static List<bool> enemyDefeatStatus = new List<bool>();
    public static bool gotEnemiesInScenes = false;
}
