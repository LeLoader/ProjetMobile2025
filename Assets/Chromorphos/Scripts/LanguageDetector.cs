using UnityEngine;

public class LanguageDetector : MonoBehaviour
{
    void Start()
    {
        string language = Application.systemLanguage.ToString();

        if (language == "French")
            Debug.Log("Joueur en France !");
        else if (language.StartsWith("English"))
            Debug.Log("Joueur dans un pays anglophone !");
        else
            Debug.Log("Langue non reconnue : " + language);
    }
}
