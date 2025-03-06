using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class ObjectUI : MonoBehaviour
{
    [SerializeField] public PositionConstraint positionConstraint;
    [SerializeField] public RotationConstraint rotationConstraint;
    [SerializeField] public GridLayoutGroup gridLayout;
    [SerializeField] public GameObject wrapper;
}
