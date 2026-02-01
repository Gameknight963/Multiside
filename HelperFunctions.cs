using Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MSZmultiplayer
{
    public static class HelperFunctions
    {
        public static GameObject CreateKiri()
        {
            if (SceneManager.GetActiveScene().name != "Version 1.9 POST") return null;

            GameObject kiri = GameObject.Find("Kiri");
            GameObject kiri2 = GameObject.Instantiate(kiri);

            GameObject cameraObject = kiri2.transform.Find(
                "Zero/PLAYER Armature/Rig Root/Hips/Spine/Chest/Neck2/Neck1/Head/CameraHoldHead/playerCamera"
            ).gameObject;
            kiri2.GetComponent<kiriMoveBasic>().enabled = false;

            cameraObject.SetActive(false);
            return kiri2;
        }
    }
}
