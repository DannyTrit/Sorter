using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sorter.GameplayLogic
{
    public class GameComponents : MonoBehaviour
    {
        [SerializeField] private Transform[] _startPoints;

        [field: SerializeField] public Transform FiguresContainer { get; private set; }
        [field: SerializeField] public Transform VfxContainer { get; private set; }
        public IEnumerable<Vector3> StartPoints => _startPoints.Select(x => x.position);
    }
}