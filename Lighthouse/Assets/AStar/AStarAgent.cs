using System;
using UnityEngine;
using System.Collections.Generic;

namespace AStar
{
    public class AStarAgent : MonoBehaviour
    {
        [SerializeField]
        protected Grid _myGrid;
        public Grid MyGrid
        {
            get { return _myGrid; }
            set { _myGrid = value; }
        }

        [SerializeField]
        protected List<GridElement> _path;
        public List<GridElement> Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [SerializeField]
        protected bool _useMyPositionAsStart;

        [SerializeField]
        protected Vector3 _startPosition;
        public Vector3 StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        [SerializeField]
        protected Transform _targetObject;
        public Transform TargetObject
        {
            get { return _targetObject; }
            set { _targetObject = value; }
        }

        [SerializeField]
        protected Vector3 _targetPosition;
        public Vector3 TargetPosition
        {
            get { return _targetPosition; }
            set { _targetPosition = value; }
        }

        protected string _gridDirection = string.Empty;
        public string GridDirection
        {
            get { return _gridDirection; }
        }

        [ContextMenu("CalculatePath")]
        public virtual void CalculatePath()
        {
            OnEnable();
            _path.Clear();
            AStar.GetStaringElement(this, _useMyPositionAsStart ? transform.position : StartPosition);
            AStar.GetTargetElement(this, _targetObject == null ? _targetPosition : _targetObject.position);
            AStar.Process(this);
        }

        public virtual void OnEnable()
        {
            try
            {
                switch (MyGrid.GridType)
                {
                    case GridType.TwoDimensional:
                        {
                            if (MyGrid.GenerateDirection.x >= 0)
                            {
                                if (MyGrid.GenerateDirection.y >= 0)
                                {
                                    _gridDirection = "ru";
                                }
                                else
                                {
                                    _gridDirection = "rd";
                                }
                            }
                            else
                            {
                                if (MyGrid.GenerateDirection.y >= 0)
                                {
                                    _gridDirection = "lu";
                                }
                                else
                                {
                                    _gridDirection = "ld";
                                }
                            }
                        }
                        break;
                    case GridType.ThreeDimensional:
                    {
                        if(MyGrid.GridType == GridType.ThreeDimensional) throw new NotImplementedException();
                    }
                    break;
                }
            }
            catch (NotImplementedException)
            {
                Debug.LogError("A* 3D not implemented");
                this.enabled = false;
            }
        }

    }
}
