using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

// Achievement*
public static class AchivementManager
{

    public const string FirstTry = "CgkI6M2W0bwfEAIQAA";
    public const string PaperKnight = "CgkI6M2W0bwfEAIQAQ";
    public const string PaperKing = "CgkI6M2W0bwfEAIQAg";
    public const string PaperGod = "CgkI6M2W0bwfEAIQAw";
    public const string BetterLuckNextTime = "CgkI6M2W0bwfEAIQBA";
    public const string perseverance = "CgkI6M2W0bwfEAIQBQ";
    public const string Ragequit = "CgkI6M2W0bwfEAIQBg";
    public const string RollingStone = "CgkI6M2W0bwfEAIQBw";
    public const string StickyStick = "CgkI6M2W0bwfEAIQCA";
    public const string SpiderMan = "CgkI6M2W0bwfEAIQCQ";
    public const string Maniac = "CgkI6M2W0bwfEAIQCg";
    public const string Really = "CgkI6M2W0bwfEAIQCw";
    public const string KeyOfSuccess = "CgkI6M2W0bwfEAIQDA";
    public const string ForAnAchivement = "CgkI6M2W0bwfEAIQDQ";
    public const string FloorIsLava = "CgkI6M2W0bwfEAIQDg";
    public const string AreYouStillHere = "CgkI6M2W0bwfEAIQDw";
    public const string LePoidsDesMots = "CgkI6M2W0bwfEAIQEA";
    public const string Ecrivain = "CgkI6M2W0bwfEAIQEQ";
    public const string Dramaturge = "CgkI6M2W0bwfEAIQEg";
    public const string IDidIt = "CgkI6M2W0bwfEAIQEw";

    public static void AutomaticConnect()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(
            (SignInStatus status) =>
            {
                switch (status)
                {
                    case SignInStatus.Success:
                        Debug.Log("Authenticate: Success");
                        break;
                    case SignInStatus.InternalError:
                        Debug.Log("Authenticate: InternalError");
                        break;
                    case SignInStatus.Canceled:
                        Debug.Log("Authenticate: Canceled");
                        break;
                }
            });
    }

    public static void ManualConnect()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.ManuallyAuthenticate(
            (SignInStatus status) =>
            {
                switch (status)
                {
                    case SignInStatus.Success:
                        Debug.Log("ManuallyAuthenticate: Success");
                        break;
                    case SignInStatus.InternalError:
                        Debug.Log("ManuallyAuthenticate: InternalError");
                        break;
                    case SignInStatus.Canceled:
                        Debug.Log("ManuallyAuthenticate: Canceled");
                        break;
                }
            });
    }

    public static void UnlockAchievement(string code)
    {
        PlayGamesPlatform.Instance.UnlockAchievement(
                        code, (bool result) =>
                        {
                            Debug.Log($"Tried to unlock achievement {code}: {result}");
                        }
                        );
    }

    public static void IncrementAchievement(string code, int progress = 100)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(
               code, progress, (bool result) =>
               {
                   Debug.Log($"Tried to increment achievement {code} with progress {progress}: {result}");
               }
                       );
    }
public static void ShowAchievement()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

}

