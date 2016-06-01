using System;
using UnityEngine;
using System.Collections;

namespace AStar
{
    public enum GridType
    {
        TwoDimensional = 0,
        ThreeDimensional
    }

    public class Grid : MonoBehaviour
    {
        [SerializeField]
        protected GridType _gridType;
        public GridType GridType
        {
            get { return _gridType; }
            set { _gridType = value; }
        }
        [SerializeField]
        protected IntVector3 _gridSize;
        public virtual IntVector3 GridSize
        {
            get { return _gridSize; }
            set { _gridSize = value; }
        }
        [SerializeField]
        protected Vector3 _gridElementSize;
        [SerializeField]
        protected Vector3 _startPosition = Vector3.zero;
        [SerializeField]
        protected bool _useMyPositionAsStart = true;
        [SerializeField]
        protected IntVector3 _generateDirection = new IntVector3(1,-1,0);
        public virtual IntVector3 GenerateDirection
        {
            get { return _generateDirection; }
            set { _generateDirection = value; }
        }
        [SerializeField]
        protected GameObject _gridElementObject;

        [SerializeField]
        protected GridElement[, ,] _elements = new GridElement[1,1,1];
        public virtual GridElement[, ,] Elements
        {
            get { return _elements; }
        }

        [SerializeField]
        protected Vector3 _elementMoveCostDirection;

        [SerializeField]
        protected bool _checkWalkability;
        [SerializeField]
        protected LayerMask _collisionLayers;

        public void Awake()
        {
            RegenerateGrid();
        }

        [ContextMenu("GenerateGrid")]
        public virtual void GenerateGrid()
        {
            _elementMoveCostDirection = new Vector3((_gridElementSize.x + _gridElementSize.y) / 2f, Mathf.Sqrt(Mathf.Pow(_gridElementSize.x, 2f) + Mathf.Pow(_gridElementSize.y, 2f)), 
                Mathf.Sqrt(Mathf.Pow(_gridElementSize.x, 2f) + Mathf.Pow(_gridElementSize.y, 2f) + Mathf.Pow(_gridElementSize.z, 2f)));

            switch (_gridType)
            {
                case GridType.TwoDimensional:
                {
                    int x = _gridSize.x;
                    int y = _gridSize.y;
                    _elements = new GridElement[x, y, 1];

                    for (int i = 0; i < x; ++i)
                    {
                        for (int j = 0; j < y; ++j)
                        {
                            GameObject element;
                            if (_gridElementObject == null)
                            {
                                element = new GameObject();
                            }
                            else
                            {
                                element =
                                    Instantiate(_gridElementObject, Vector3.zero, Quaternion.identity) as GameObject;
                            }
                            element.name = string.Format("GridElement[{0}][{1}]", j, i);

                            if (_useMyPositionAsStart)
                            {
                                Vector3 startPosition = gameObject.transform.position;
                                element.transform.position =
                                    new Vector3(startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                        startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                        startPosition.z);
                            }
                            else
                            {
                                element.transform.position =
                                    new Vector3(_startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                        _startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                        _startPosition.z);
                            }
                            element.transform.parent = gameObject.transform;

                            GridElement gridElement = element.GetComponent<GridElement>();
                            if (gridElement == null)
                            {
                                gridElement = element.AddComponent<GridElement>();
                            }

                            gridElement.ElementIndex = new IntVector3(j,i,0);
                            gridElement.ElementMoveCostDirection = _elementMoveCostDirection;
                            _elements[j, i, 0] = gridElement;
                        }
                    }
                }
                break;
                case GridType.ThreeDimensional:
                {
                    int x = _gridSize.x;
                    int y = _gridSize.y;
                    int z = _gridSize.z;
                    _elements = new GridElement[x, y, z];

                    for (int i = 0; i < x; ++i)
                    {
                        for (int j = 0; j < y; ++j)
                        {
                            for (int k = 0; k < z; ++k)
                            {
                                GameObject element;
                                if (_gridElementObject == null)
                                {
                                    element = new GameObject();
                                }
                                else
                                {
                                    element =
                                        Instantiate(_gridElementObject, Vector3.zero, Quaternion.identity) as GameObject;
                                }
                                element.name = string.Format("GridElement[{0}][{1}][{2}]", j, i, k);

                                if (_useMyPositionAsStart)
                                {
                                    Vector3 startPosition = gameObject.transform.position;
                                    element.transform.position =
                                        new Vector3(startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                            startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                            startPosition.z + (_gridElementSize.z * _generateDirection.z * k));
                                }
                                else
                                {
                                    element.transform.position =
                                        new Vector3(_startPosition.x + (_gridElementSize.x*_generateDirection.x*j),
                                            _startPosition.y + (_gridElementSize.y*_generateDirection.y*i),
                                            _startPosition.z + (_gridElementSize.z * _generateDirection.z * k));
                                    }
                                element.transform.parent = gameObject.transform;

                                GridElement gridElement = element.GetComponent<GridElement>();

                                if (gridElement == null)
                                {
                                    _elements[j, i, k] = element.AddComponent<GridElement>();
                                }
                                else
                                {
                                    _elements[j, i, k] = gridElement;
                                }
                                gridElement.ElementIndex = new IntVector3(j,i,k);
                            }
                        }
                    }
                }
                break;
            }
            if(_checkWalkability) CheckGridWalkability();
        }

        [ContextMenu("RemoveGrid")]
        public virtual void RemoveGrid()
        {
            int child = transform.childCount;

            for (int i = 0; i < child; ++i)
            {
                DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            }
        }

        [ContextMenu("ReGenerateGrid")]
        public virtual void RegenerateGrid()
        {
            RemoveGrid();
            GenerateGrid();
        }

        public virtual void CheckGridWalkability()
        {
            switch (_gridType)
            {
                case GridType.TwoDimensional:
                {
                    float radius = _gridElementSize.x < _gridElementSize.y ? _gridElementSize.x : _gridElementSize.y;
                    RaycastHit2D[] hits;
                    foreach (GridElement element in _elements)
                    {
                        hits = Physics2D.CircleCastAll(element.transform.position, radius, Vector2.zero, 0f, _collisionLayers);
                        element.Walkable = hits == null || hits.Length == 0;
                    }
                }
                break;
                case GridType.ThreeDimensional:
                {
                    float radius = _gridElementSize.x < _gridElementSize.y ? _gridElementSize.x : _gridElementSize.y;
                    radius = radius > _gridElementSize.z ? _gridElementSize.z : radius;
                    RaycastHit[] hits;
                    foreach (GridElement element in _elements)
                    {
                        hits = Physics.SphereCastAll(element.transform.position, radius, Vector3.zero, 0f, _collisionLayers);
                        element.Walkable = hits != null && hits.Length > 0;
                    }
                }
                break;
            }
        }
    }

    [Serializable]
    public class IntVector3
    {
        [SerializeField]
        protected int _x;
        public int x
        {
            get { return _x; }
            set { _x = value; }
        }
        [SerializeField]
        protected int _y;
        public int y
        {
            get { return _y; }
            set { _y = value; }
        }
        [SerializeField]
        protected int _z;
        public int z
        {
            get { return _z; }
            set { _z = value; }
        }

        public IntVector3(int x=0, int y=0, int z=0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public IntVector3(IntVector3 other)
        {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            IntVector3 objAsIntVec = obj as IntVector3;
            if (objAsIntVec == null) return false;
            return Equals(objAsIntVec);
        }

        public override int GetHashCode()
        {
            return x*y*z;
        }

        public bool Equals(IntVector3 other)
        {
            if (other == null) return false;
            return x == other.x && y == other.y && z == other.z;
        }
    }
}
