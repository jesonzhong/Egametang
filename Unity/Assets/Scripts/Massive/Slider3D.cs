using UnityEngine;
using System.Collections;

public class Slider3D : MonoBehaviour
{
    private Transform _spriteBg;
    [SerializeField]
    private Transform _spriteFill;
    [SerializeField]
    private Transform _scaleTrans;

    private Vector3 _defaultScale;

    void Awake()
    {
        _defaultScale = _scaleTrans.localScale;
        _scaleTrans.localPosition = new Vector3(-0.5f, 0, 0);
        _spriteFill.localPosition = new Vector3(0.5f, 0, 0);
        sliderValue = 1;
        scaleValue = 1;
    }


    [SerializeField]
    [Range(0, 1)]
    private float _sliderValue = 1;

    public float sliderValue
    {
        get
        {
            return _sliderValue;
        }
        set
        {
            _sliderValue = value;
            if (_spriteFill != null)
            {
                _spriteFill.localScale = new Vector3(_sliderValue, 1, 1);
                _spriteFill.localPosition = new Vector3(0.44f * _sliderValue - 0.44f + 0.5f, 0, _spriteFill.localPosition.z);
            }
        }
    }


    [SerializeField]
    [Range(0, 1f)]
    private float _scaleValue = 1;

    public float scaleValue
    {
        get
        {
            return _scaleValue;
        }
        set
        {
            _scaleValue = value;
            _scaleTrans.localScale = new Vector3(_scaleValue * _defaultScale.x, _defaultScale.y, _defaultScale.z);
            _scaleTrans.localPosition = new Vector3(-0.055f * _scaleValue + 0.045f - 0.5f, 0, _spriteFill.localPosition.z);
        }
    }

    void Update()
    {
        scaleValue = _scaleValue;
        sliderValue = _sliderValue;
    }
}
