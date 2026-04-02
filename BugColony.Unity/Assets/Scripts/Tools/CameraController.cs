using System;
using Tools;
using UnityEngine;

public class CameraController : MonoBehaviour
{
   private Camera _camera;

   [SerializeField,Range(1,1000)] private float multiplier = 10f;
   private void Awake()
   {
      _camera = GetComponent<Camera>();
   }

   private void Update()
   {
       //_camera.orthographicSize = Mathf.Clamp(FieldMeasurer.GetFieldSize().magnitude/multiplier, 10f, 100f);
   }
}
