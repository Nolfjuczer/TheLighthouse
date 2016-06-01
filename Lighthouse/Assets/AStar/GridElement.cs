using UnityEngine;
using System.Collections;

namespace AStar
{
    public class GridElement : MonoBehaviour
    {
        [SerializeField]
        protected bool _walkable = true;
        public virtual bool Walkable
        {
            get { return _walkable; }
            set { _walkable = value; }
        }

        [SerializeField]
        protected float _fieldMoveCost;
        public virtual float FieldMoveCost
        {
            get { return _fieldMoveCost; }
            set { _fieldMoveCost = value; }
        }

        protected float _heuristicCost;

        protected IntVector3 _elementIndex;
        public virtual IntVector3 ElementIndex
        {
            get { return _elementIndex; }
            set { _elementIndex = value; }
        }

        protected GridElement _pathParentField = null;
        public virtual GridElement PathParentField 
        {
            get { return _pathParentField; }
            set
            {
                _pathParentField = value;
                int dist = CheckMoveDistance(this);
                if (dist <= 1)
                {
                    _fieldMoveCost = _elementMoveCostDirection.x;
                }
                else if (dist == 2)
                {
                    _fieldMoveCost = _elementMoveCostDirection.y;
                }
                else
                {
                    _fieldMoveCost = _elementMoveCostDirection.z;
                }
            }
        }

        public virtual float TotalFieldMoveCost
        {
            get
            {
                GridElement element = this;
                float totalCost = _heuristicCost;
                while (element != null)
                {
                    if (element == null || element == element.PathParentField) break;
                    totalCost += element.FieldMoveCost;
                    element = element.PathParentField;
                }
                return totalCost;
            }
        }

        protected Vector3 _elementMoveCostDirection;
        public virtual Vector3 ElementMoveCostDirection
        {
            get { return _elementMoveCostDirection; }
            set { _elementMoveCostDirection = value; }
        }


        public static int CheckMoveDistance(GridElement element)
        {
            if (element.PathParentField != null)
            {
                return Mathf.Abs(element.ElementIndex.x - element.PathParentField.ElementIndex.x) +
                    Mathf.Abs(element.ElementIndex.y - element.PathParentField.ElementIndex.y) +
                    Mathf.Abs(element.ElementIndex.z - element.PathParentField.ElementIndex.z);               
            }
            else
            {
                return 0;
            }
        }

        public static int CheckMoveDistance(GridElement element, GridElement newParent)
        {
            return Mathf.Abs(element.ElementIndex.x - newParent.ElementIndex.x) +
                Mathf.Abs(element.ElementIndex.y - newParent.ElementIndex.y) +
                Mathf.Abs(element.ElementIndex.z - newParent.ElementIndex.z);
        }

        public virtual void CalculateHeuristic(GridElement element)
        {
            _heuristicCost = Mathf.CeilToInt(Vector3.Distance(gameObject.transform.position, element.transform.position));
        }

        public virtual void CheckNewParent(GridElement newParent)
        {
            GridElement oldParentElement = PathParentField;
            float cost = TotalFieldMoveCost;
            PathParentField = newParent;
            if (cost < TotalFieldMoveCost)
            {
                PathParentField = oldParentElement;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            GridElement objAsGridElement = obj as GridElement;
            if (objAsGridElement == null) return false;
            return Equals(objAsGridElement);
        }
        public override int GetHashCode()
        {
            return _elementIndex.x * _elementIndex.y * _elementIndex.z;
        }
        public bool Equals(GridElement other)
        {
            if (other == null) return false;
            return _elementIndex.Equals(other._elementIndex);
        }
    }
}
