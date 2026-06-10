using System.Collections.Generic;
using UnityEngine;
using HandsForMobileAR.DefaultComponents;

namespace HandsForMobileAR
{
    namespace DrawerSample
    {
        [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
        public class TubeDrawer : MonoBehaviour
        {
            [SerializeField] private float _radius = 0.02f;
            [SerializeField] private int _radialSegments = 8;
            [SerializeField] private Color _brushColor = Color.white;

            [SerializeField] private LandmarkInterpreter _landmarkInterpreter;
            [SerializeField] private PoseDetectBus _poseDetectBus;

            [SerializeField] private MeshFilter PaintingPrefab;

            [SerializeField]
            private bool _isDrawMode;

            private Mesh _mesh;
            private List<Vector3> _path = new List<Vector3>();

            private List<Vector3> _vertices = new List<Vector3>();
            private List<int> _triangles = new List<int>();
            private List<Color> _colors = new List<Color>();

            private void Start()
            {
                _mesh = new Mesh();
                GetComponent<MeshFilter>().mesh = _mesh;

                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Pointing_Up.ToString(), OnPointUpDetected);
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Closed_Fist.ToString(), OnFistDetected);
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Thumb_Down.ToString(), OnDislikeDetected);
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Thumb_Up.ToString(), OnLikeDetected);
            }

            private void Update()
            {
                if (_isDrawMode)
                {
                    Vector3 currentPos = _landmarkInterpreter.LastProcessedLandmarks[8];
                    if (_path.Count == 0 || Vector3.Distance(currentPos, _path[_path.Count - 1]) > 0.02f)
                    {
                        _path.Add(currentPos);
                        UpdateTubeMesh();
                    }
                }
            }

            private void UpdateTubeMesh()
            {
                if (_path.Count < 2) return;

                _vertices.Clear();
                _triangles.Clear();
                _colors.Clear();

                Vector3 lastNormal = Vector3.up;

                for (int i = 0; i < _path.Count; i++)
                {
                    Vector3 tangent = GetTangent(i);

                    Vector3 normal = Vector3.Cross(tangent, lastNormal).normalized;
                    if (normal.sqrMagnitude < 0.001f)
                    {
                        normal = Vector3.Cross(tangent, Vector3.right).normalized;
                    }
                    Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                    lastNormal = normal;

                    for (int j = 0; j < _radialSegments; j++)
                    {
                        float angle = j * 2f * Mathf.PI / _radialSegments;
                        float x = Mathf.Cos(angle) * _radius;
                        float y = Mathf.Sin(angle) * _radius;

                        Vector3 vertexOffset = (normal * x) + (binormal * y);
                        _vertices.Add(_path[i] + vertexOffset);
                        _colors.Add(_brushColor);

                        if (i < _path.Count - 1)
                        {
                            int current = i * _radialSegments + j;
                            int next = (i + 1) * _radialSegments + j;
                            int nextRadial = i * _radialSegments + (j + 1) % _radialSegments;
                            int nextNextRadial = (i + 1) * _radialSegments + (j + 1) % _radialSegments;

                            _triangles.Add(current);
                            _triangles.Add(next);
                            _triangles.Add(nextRadial);

                            _triangles.Add(nextRadial);
                            _triangles.Add(next);
                            _triangles.Add(nextNextRadial);
                        }
                    }
                }

                _mesh.Clear();
                _mesh.SetVertices(_vertices);
                _mesh.SetTriangles(_triangles, 0);
                _mesh.SetColors(_colors);
                _mesh.RecalculateNormals();
            }

            private Vector3 GetTangent(int index)
            {
                if (index == 0) return (_path[1] - _path[0]).normalized;
                if (index == _path.Count - 1) return (_path[index] - _path[index - 1]).normalized;
                return (_path[index + 1] - _path[index - 1]).normalized;
            }

            private void OnPointUpDetected() => _isDrawMode = true;
            private void OnFistDetected() => _isDrawMode = false;
            private void OnDislikeDetected()
            {
                _isDrawMode = false;
                _path.Clear();
                _mesh.Clear();
            }
            private void OnLikeDetected()
            {
                var painting = Instantiate(PaintingPrefab);
                painting.mesh = _mesh;

                _isDrawMode = false;
                _path.Clear();

                _mesh = new Mesh();
                GetComponent<MeshFilter>().mesh = _mesh;
            }

            public void SetBrushColor(Color color)
            {
                _brushColor = color;
                UpdateTubeMesh();
            }
        }
    }
}