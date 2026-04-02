using UnityEngine;

namespace Tools
{ 
    public static class FieldMeasurer
    { 
        private static Vector3 _minPoint = Vector3.positiveInfinity;
        private static Vector3 _maxPoint = Vector3.negativeInfinity;
        private static bool _initialized = false;

        
        public static void AddPoint(Vector3 point)
        {
            if (!_initialized)
            {
                _minPoint = point;
                _maxPoint = point;
                _initialized = true;
                return;
            }
 
            _minPoint.x = Mathf.Min(_minPoint.x, point.x);
            _minPoint.y = Mathf.Min(_minPoint.y, point.y);
            _minPoint.z = Mathf.Min(_minPoint.z, point.z);
 
            _maxPoint.x = Mathf.Max(_maxPoint.x, point.x);
            _maxPoint.y = Mathf.Max(_maxPoint.y, point.y);
            _maxPoint.z = Mathf.Max(_maxPoint.z, point.z);
        }
 
        public static Vector3 GetMinPoint() => _minPoint;

      
        public static Vector3 GetMaxPoint() => _maxPoint;

       
        public static Vector3 GetFieldSize() => _maxPoint - _minPoint;
 
        public static Vector3 GetFieldCenter() => (_minPoint + _maxPoint) * 0.5f;
 
        public static void Reset()
        {
            _minPoint = Vector3.positiveInfinity;
            _maxPoint = Vector3.negativeInfinity;
            _initialized = false;
        }
    }
}