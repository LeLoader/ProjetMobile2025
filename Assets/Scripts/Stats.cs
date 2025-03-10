using System;

public static class Stats
{
    public enum STATS
    {
        NONE,
        APPLICATION_STARTED_COUNT,
        IN_GAME_TIME,
        SERINGUE_COUNT,
        JUMP_COUNT,
        BOUNCE_COUNT,
        STICK_COUNT,
        RESTART_COUNT,
    }

    public static void IncrementStat(STATS statType, int value = 1)
    {
        if (!SaveSystem._instance) return;
        ref PlayerStats stats = ref SaveSystem._instance._playerStats;

        switch (statType)
        {
            case STATS.APPLICATION_STARTED_COUNT:
                stats.application_started_count += value;
                break;
            case STATS.IN_GAME_TIME:
                stats.in_game_time += value;
                break;
            case STATS.SERINGUE_COUNT:
                stats.seringue_count += value;
                if (stats.seringue_count >= 1000)
                {
                    AchivementManager.UnlockAchievement(AchivementManager.Dramaturge);
                }
                else if (stats.seringue_count >= 100)
                {
                    AchivementManager.UnlockAchievement(AchivementManager.Ecrivain);
                }
                else if (stats.seringue_count >= 50)
                {
                    AchivementManager.UnlockAchievement(AchivementManager.LePoidsDesMots);
                }
                break;
            case STATS.JUMP_COUNT:
                stats.jump_count += value;
                break;
            case STATS.BOUNCE_COUNT:
                stats.bounce_count += value;
                break;
            case STATS.STICK_COUNT:
                stats.stick_count += value;
                if (stats.stick_count >= 10)
                {
                    AchivementManager.UnlockAchievement(AchivementManager.StickyStick);
                }
                break;
            case STATS.RESTART_COUNT:
                stats.restart_count += value;
                AchivementManager.UnlockAchievement(AchivementManager.BetterLuckNextTime);
                break;
            default:
                break;
        }
    }
}

[Serializable]
public struct PlayerStats
{
    // General
    public int application_started_count;
    public int in_game_time;
    public int restart_count;

    // Player
    public int seringue_count;
    public int jump_count;

    // Blocks
    public int bounce_count;
    public int stick_count;
}

