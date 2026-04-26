using System;
using UnityEngine;

namespace MultiSide.shared
{
    [Serializable]
    public class PlayerPositionData
    {
        public int actorNumber;
        public Vector3 position;
        public Quaternion rotation;

        public PlayerPositionData(int id, Vector3 pos, Quaternion rot)
        {
            actorNumber = id;
            position = pos;
            rotation = rot;
        }
    }
}
