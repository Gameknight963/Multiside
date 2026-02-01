using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MSZmultiplayer
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
