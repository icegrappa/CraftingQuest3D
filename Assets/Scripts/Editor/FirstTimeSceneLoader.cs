using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public static class FirstTimeSceneLoader
{
    static FirstTimeSceneLoader()
    {
        // Sprawdzam czy to pierwsze uruchomienie projektu
        if (!PlayerPrefs.HasKey("ProjectFirstRun"))
        {
            // Otwieram okno z pytaniem czy załadować wymagane sceny
            FirstTimeSceneLoaderWindow.ShowWindow();
        }
    }
}

public class FirstTimeSceneLoaderWindow : EditorWindow
{
    public static void ShowWindow()
    {
        // Tworzę i wyświetlam okno
        FirstTimeSceneLoaderWindow window = (FirstTimeSceneLoaderWindow)EditorWindow.GetWindow(typeof(FirstTimeSceneLoaderWindow));
        window.titleContent = new GUIContent("Pierwsze uruchomienie projektu");
        window.Show();
    }

    private void OnGUI()
    {
        // Wyświetlam komunikat do użytkownika
        GUILayout.Label("To wygląda na pierwsze uruchomienie projektu.", EditorStyles.boldLabel);
        GUILayout.Label("Czy chcesz załadować sceny wymagane do poprawnego działania?", EditorStyles.wordWrappedLabel);

        GUILayout.Space(20);

        // Jeśli użytkownik wybierze "Tak"
        if (GUILayout.Button("Tak, załaduj sceny"))
        {
            // Ładuję i otwieram scenę głównego menu
            EditorSceneManager.OpenScene("Assets/Scenes/Scene_Main_Menu.unity", OpenSceneMode.Single);

            // Ładuję scenę świata w tle
            EditorSceneManager.OpenScene("Assets/Scenes/Scene_World_01.unity", OpenSceneMode.AdditiveWithoutLoading);

            // Zapisuję, że pierwsze uruchomienie zostało zakończone
            PlayerPrefs.SetInt("ProjectFirstRun", 1);
            PlayerPrefs.Save();

            // Zamykam okno
            this.Close();
        }

        // Jeśli użytkownik wybierze "Nie"
        if (GUILayout.Button("Nie, załaduję sceny ręcznie"))
        {
            // Zapisuję, że pierwsze uruchomienie zostało zakończone, nawet jeśli użytkownik nie załadował scen automatycznie
            PlayerPrefs.SetInt("ProjectFirstRun", 1);
            PlayerPrefs.Save();

            // Zamykam okno
            this.Close();
        }
    }
}
#endif
