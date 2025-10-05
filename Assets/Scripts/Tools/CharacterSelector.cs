using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private CameraLookAt _lookAt;

    private GameObject _selected;
    private Animator _targetAnimator;

    public void SelectVisual(GameObject targetVisual)
    {
        if(targetVisual != null)
        {
            if(_selected != null && _selected != targetVisual)
                _selected.SetActive(false);

            _selected = targetVisual;
        }
        else
            Debug.LogWarning("Boss not implemented yet!");
    }

    public void SelectTarget(Transform targetHips)
    {
        if(targetHips == null)
            return;

        _selected.SetActive(true);
        _targetAnimator = targetHips.GetComponentInParent<Animator>();
        _lookAt.Target = targetHips;
    }

    public void SetTrigger(string trigger)
    {
        if (_targetAnimator != null)
            _targetAnimator.SetTrigger(trigger);
        else
            Debug.LogWarning($"Couldn't set trigger \"{trigger}\" because no target is selected", this);
    }
}
