using Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiSide
{
    public static class HelperFunctions
    {
        public static GameObject CreateKiri()
        {
            ModController.CoolLogger.Msg("adding new player");
            if (SceneManager.GetActiveScene().name != "Version 1.9 POST")
                throw new InvalidOperationException("Problem detected, dont call it in this scene");

            GameObject kiri = GameObject.Find("Kiri");
            GameObject kiri2 = GameObject.Instantiate(kiri);

            GameObject cameraObject = kiri2.transform.Find("Zero/PLAYER Armature/Rig Root/Hips/Spine/Chest/Neck2/Neck1/Head/CameraHoldHead/playerCamera").gameObject;
            try
            {
                cameraObject.SetActive(false);

                kiri2.GetComponent<kiriMoveBasic>()?.enabled = false;

                //disable bhopmovment
                MonoBehaviour? bhopMovement = kiri2.GetComponents<MonoBehaviour>()
                    .FirstOrDefault(mb => mb.GetType().Name == "BhopMovement");
                bhopMovement?.enabled = false;

                kiri2.transform.Find("PsuedoHead").gameObject.GetComponent<kiriLook>()?.enabled = false;

                kiri2.GetComponent<Rigidbody>()?.isKinematic = true;
            }
            catch (NullReferenceException)
            {
                ModController.CoolLogger.Error($"Null exception while creating new player. One of the components disabled may not exist");
            }
            return kiri2;
        }
    }
}
