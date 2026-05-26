using Assets.Scripts;
using UnityEngine;

namespace Assets
{
    internal interface IDepthModifier
    {
        public void Process(Vector3[] landmarks, IImageProvider imageProvider);
    }
}
