using System;
using UnityEngine;
using System.Collections.Generic;

namespace AStar
{
    public static class AStar
    {
        private static List<GridElement> _openElements = new List<GridElement>();
        private static List<GridElement> _closedElements = new List<GridElement>();

        private static GridElement _currentElement;
        private static GridElement _startElement;
        private static GridElement _targetElement;


        public static void GetStaringElement(AStarAgent agent, Vector3 position)
        {
            _startElement = FindClosestElement(agent, position);
        }

        public static void GetTargetElement(AStarAgent agent, Vector3 position)
        {
            _targetElement = FindClosestElement(agent, position);
        }

        private static GridElement FindClosestElement(AStarAgent agent, Vector3 position)
        {
            GridElement closestElement = null;
            try
            {
                int xMax, xMin, yMax,yMin,/* zMax, zMin,*/ i, j;
                i = xMax = agent.MyGrid.GridSize.y;
                j = yMax = agent.MyGrid.GridSize.x;
                //zMax = agent.MyGrid.GridSize.z;


                xMin =  yMin =/* zMin =*/ 0;

                switch (agent.MyGrid.GridType)
                {
                    case GridType.TwoDimensional:
                    {
                        Debug.Log( agent.MyGrid.Elements.Length);
                            
                            List<GridElement> leftPoints = new List<GridElement>();
                            List<GridElement> middlePoints = new List<GridElement>();
                            List<GridElement> rightPoints = new List<GridElement>();

                            leftPoints.Add(agent.MyGrid.Elements[xMin, yMin, 0]);
                            leftPoints.Add(agent.MyGrid.Elements[xMin, yMax / 2, 0]);
                            leftPoints.Add(agent.MyGrid.Elements[xMin, yMax - 1, 0]);

                            middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMin, 0]);
                            middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                            middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                            rightPoints.Add(agent.MyGrid.Elements[xMax - 1, 0, 0]);
                            rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                            rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax - 1, 0]);

                            while (i > 1 || j > 1)
                            {
                                i /= 2;
                                j /= 2;
                                if (FragmentsContaisPosition(agent.GridDirection, leftPoints[0],middlePoints[1], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMax /= 2;
                                    yMax /= 2;

                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                                if (FragmentsContaisPosition(agent.GridDirection, middlePoints[0], rightPoints[1], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMin = xMax / 2;
                                    yMax /= 2;

                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                                if (FragmentsContaisPosition(agent.GridDirection, leftPoints[1], middlePoints[2], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMax /= 2;
                                    yMin = yMax / 2;

                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                                if (FragmentsContaisPosition(agent.GridDirection, middlePoints[1], rightPoints[2], position))
                                {
                                    leftPoints.Clear();
                                    middlePoints.Clear();
                                    rightPoints.Clear();

                                    xMin = xMax / 2;
                                    yMin = yMax / 2;

                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMin, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax / 2, 0]);
                                    leftPoints.Add(agent.MyGrid.Elements[xMin, yMax - 1, 0]);

                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMin, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax / 2, 0]);
                                    middlePoints.Add(agent.MyGrid.Elements[xMax / 2, yMax - 1, 0]);

                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, 0, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax / 2, 0]);
                                    rightPoints.Add(agent.MyGrid.Elements[xMax - 1, yMax - 1, 0]);
                                    continue;
                                }
                            }

                            float minDist, curDist;
                            minDist= Vector3.Distance(leftPoints[0].transform.position, position);
                            closestElement = leftPoints[0];
                            curDist = Vector3.Distance(rightPoints[0].transform.position, position);
                            if (minDist > curDist)
                            {
                                minDist = curDist;
                                closestElement = rightPoints[0];
                            }
                            curDist = Vector3.Distance(leftPoints[2].transform.position, position);
                            if (minDist > curDist)
                            {
                                minDist = curDist;
                                closestElement = leftPoints[2];
                            }
                            curDist = Vector3.Distance(rightPoints[2].transform.position, position);
                            if (minDist > curDist)
                            {
                                minDist = curDist;
                                closestElement = rightPoints[2];
                            }
                            return closestElement;
                        }
                    case GridType.ThreeDimensional:
                    {
                        Debug.LogError("A* 3D not implemented");
                        if(agent.MyGrid.GridType == GridType.ThreeDimensional) throw new NotImplementedException();
                    }
                    break;
                }
            }
            catch (NotImplementedException)
            {
                return null;
            }
            return null;
        }

        private static bool FragmentsContaisPosition(String direction, GridElement leftUp, GridElement rightDown, Vector3 position)
        {
            switch (direction)
            {
                case "ru":
                    if (position.x >= leftUp.transform.position.x && position.x <= rightDown.transform.position.x
                        && position.y >= rightDown.transform.position.y && position.y <= leftUp.transform.position.y)
                        return true;
                    else
                        return false;
                case "rd":
                    if (position.x >= leftUp.transform.position.x && position.x <= rightDown.transform.position.x
                        && position.y <= leftUp.transform.position.y && position.y >= rightDown.transform.position.y)
                        return true;
                    else
                        return false;
                case "lu":
                    if (position.x <= rightDown.transform.position.x && position.x >= leftUp.transform.position.x
                        && position.y >= rightDown.transform.position.y && position.y <= leftUp.transform.position.y)
                        return true;
                    else
                        return false;
                case "ld":
                    if (position.x <= rightDown.transform.position.x && position.x >= leftUp.transform.position.x
                         && position.y <= leftUp.transform.position.y && position.y >= rightDown.transform.position.y)
                        return true;
                    else
                        return false;
            }
            return false;
        }

        public static void Process(AStarAgent agent)
        {
            foreach (GridElement element in agent.MyGrid.Elements)
            {
                element.PathParentField = null;
            }

            _openElements.Clear();
            _closedElements.Clear();
            _startElement.PathParentField = null;
            int lowestCost = 0,openSize = 0;

            bool complete = false;
            if (_startElement != null)
            {
                _openElements.Add(_startElement);
                _currentElement = _startElement;
                _currentElement.CalculateHeuristic(_targetElement);
            }
            while (_openElements.Count > 0)
            {
                openSize = _openElements.Count;
                lowestCost = Int32.MaxValue;
                for (int i = 0; i < openSize; ++i)
                {
                    if (_openElements[i].TotalFieldMoveCost < lowestCost)
                    {
                        _currentElement = _openElements[i];
                        lowestCost = _currentElement.TotalFieldMoveCost;
                    }
                }
                _openElements.Remove(_currentElement);
                _closedElements.Add(_currentElement);

                if (_currentElement.ElementIndex == _targetElement.ElementIndex)
                {
                    complete = true;
                    break;
                }
                CheckNeighbours(agent.MyGrid);
            }

            if (complete)
            {
                GridElement step = _currentElement;
                agent.Path.Clear();
                do
                {
                    agent.Path.Add(step);
                    step = step.PathParentField;
                } while (step != null);

                agent.Path.Reverse();
            }
        }

        private static void CheckNeighbours(Grid grid)
        {
            IntVector3 index = _currentElement.ElementIndex;
            index.x -= 1;
            CheckNeighbourHelper(grid, index);

            index = _currentElement.ElementIndex;
            index.x += 1;
            CheckNeighbourHelper(grid, index);

            index = _currentElement.ElementIndex;
            index.y -= 1;
            CheckNeighbourHelper(grid, index);

            index = _currentElement.ElementIndex;
            index.y += 1;
            CheckNeighbourHelper(grid, index);

            index = _currentElement.ElementIndex;
            index.x -= 1;
            index.y -= 1;
            CheckNeighbourHelper(grid, index);

            index = _currentElement.ElementIndex;
            index.x -= 1;
            index.y += 1;
            CheckNeighbourHelper(grid, index);

            index = _currentElement.ElementIndex;
            index.x += 1;
            index.y -= 1;
            CheckNeighbourHelper(grid, index);

            index = _currentElement.ElementIndex;
            index.x += 1;
            index.y += 1;
            CheckNeighbourHelper(grid, index);
        }

        private static void CheckNeighbourHelper(Grid grid, IntVector3 index)
        {
            if (IsIndexInGrid(grid, index))
            {
                GridElement element = grid.Elements[index.x, index.y, index.z];
                if (element.Walkable && !_closedElements.Contains(element))
                {
                    if (!_openElements.Contains(element))
                    {
                        element.PathParentField = _currentElement;
                        element.CalculateHeuristic(_targetElement);
                        _openElements.Add(element);
                    }
                    else
                    {
                        if (GridElement.CheckMoveDistance(element) >
                            GridElement.CheckMoveDistance(element, _currentElement))
                        {
                            element.PathParentField = _currentElement;
                            element.CalculateHeuristic(_targetElement);
                        }
                    }
                }
            }
        }

        private static bool IsIndexInGrid(Grid grid, IntVector3 index)
        {
            if (index.x >= 0 && index.x < grid.GridSize.x &&
                index.y >= 0 && index.y < grid.GridSize.y)
                return true;
            else
                return false;
        }
    }
}
