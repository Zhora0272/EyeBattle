using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;
using UniRx;

public class MergeController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Camera _camera;

    private SelectableGrid _selectableGrid;

    public ReactiveProperty<int> MaxWeaponLevel = new();

    public void OnPointerDown(PointerEventData eventData)
    {
        _canMoveObject = true;
        var pos = _camera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(_camera.transform.position, pos.direction, out RaycastHit hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Selectable"))
            {
                _selectableGrid = hit.transform.GetComponent<SelectableGrid>();
            }
            else
            {
                _selectableGrid = null;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var pos = _camera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(_camera.transform.position, pos.direction, out RaycastHit hit))
        {
            if (_selectableGrid && hit.transform.gameObject.layer == LayerMask.NameToLayer("Selectable"))
            {
                if (_selectableGrid.GetObject(out var result))
                {
                    var state = hit.transform.GetComponent<SelectableGrid>();

                    if (_selectableGrid.GetUpgradeIndex() == state.GetUpgradeIndex() || state.GetUpgradeIndex() == -1)
                    {
                        state.MoveToObject(result, _selectableGrid.GetUpgradeIndex());

                        if (state != _selectableGrid)
                        {
                            _selectableGrid.RemoveObject();
                        }
                        else
                        {
                            _canMoveObject = false;
                            _selectableGrid.CallToBack();
                        }

                        /*if (state.GetUpgradeIndex() > MaxWeaponLevel.Value)
                        {
                            /*PlayerPrefs.SetInt(PlayerPrefsEnum.WeaponUpdateLevel.ToString(),
                                state.GetUpgradeIndex());
                            MaxWeaponLevel.Value = state.GetUpgradeIndex();#1#
                        }*/
                    }
                    else
                    {
                        _canMoveObject = false;
                        _selectableGrid.CallToBack();
                    }
                }
            }
            else if (_selectableGrid)
            {
                _canMoveObject = false;
                _selectableGrid.CallToBack();
            }
        }
    }

    bool _canMoveObject;

    public void OnDrag(PointerEventData eventData)
    {
        var pos = _camera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(_camera.transform.position, pos.direction, out RaycastHit hit))
        {
            if (_selectableGrid && _selectableGrid.GetObject(out var objects))
            {
                if (_canMoveObject)
                {
                    var hitPoint = hit.point;
                    var clickVector = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z)
                                      + Vector3.up * 0.3f
                                      - Vector3.forward * 0.2f;

                    objects.transform.DOMove(clickVector, 0.2f);
                }
            }
        }
    }
}