using UnityEditor;
using UnityEngine;

namespace HB.Match3.Editor
{
    public class ResetProfile : MonoBehaviour
    {
    
    
    
        static void DoResetProfile()
        {
            // if (File.Exists(App.ProfilePath))
            // {
            //     File.Delete(App.ProfilePath);
            // }
            // if (File.Exists(App.ProfilePathBackup))
            // {
            //     File.Delete(App.ProfilePathBackup);
            // }
            // if (File.Exists(App.ProfilePathBackup2))
            // {
            //     File.Delete(App.ProfilePathBackup2);
            // }
            Debug.Log("Deleting profile...");
        }
    
    
        static void DoResetProfileBackup()
        {
            // if (File.Exists(App.ProfilePathBackup))
            // {
            //     File.Delete(App.ProfilePathBackup);
            // }
            Debug.Log("Deleting profile...");
        }
    
        static void DoResetProfileBackup2()
        {
            // if (File.Exists(App.ProfilePathBackup2))
            // {
            //     File.Delete(App.ProfilePathBackup2);
            // }
            Debug.Log("Deleting profile...");
        }
    
    
        [MenuItem("Garaj/ResetProfileAll")]
        static void DoResetProfileAll()
        {
            DoResetProfile();
            DoResetProfileBackup();
            DoResetProfileBackup2();
            DoResetProfilePlayerPrefs();
        }


    
        static void DoResetProfilePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    
    
    
    
    
    
    }
}