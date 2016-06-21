using UnityEngine;
using System.Collections;

namespace Utility
{
	[System.Serializable]
	public class MemberObjectPool
	{
		#region Variables

		private GameObject _prefab = null;
		private GameObject[] _pool = null;
		public GameObject[] Pool { get { return _pool; } }
		private int _poolSize = 0;
		public int PoolSize { get { return _poolSize; } }
		private Transform _parent = null;

		private int _lastFoundIndex = 0;

		#endregion Variables

		#region Methods

		public MemberObjectPool(GameObject prefab, Transform parent, int initialPoolSize = 0)
		{
			_prefab = prefab;
			_parent = parent;
			_poolSize = initialPoolSize;

			_pool = new GameObject[_poolSize];
			for(int i = 0;i < _poolSize;++i)
			{
				_pool[i] = GameObject.Instantiate(_prefab) as GameObject;
				if(_parent != null) _pool[i].transform.SetParent(_parent);
				_pool[i].transform.localScale = _prefab.transform.localScale;
				_pool[i].SetActive(false);
			}
		}

		public GameObject GetPooledObject()
		{
			GameObject result = null;

			for(int i = 0;i < _poolSize;++i)
			{
				if(!_pool[_lastFoundIndex].activeSelf)
				{
					result = _pool[_lastFoundIndex];
				} else {
					_lastFoundIndex = (_lastFoundIndex + 1) % _poolSize;
                }
			}

			if(result == null)
			{
				GameObject[] oldPool = _pool;
				_pool = new GameObject[_poolSize + 1];
				for(int i = 0;i < _poolSize;++i)
				{
					_pool[i] = oldPool[i];
				}
				_pool[_poolSize] = GameObject.Instantiate(_prefab) as GameObject;
				_pool[_poolSize].transform.SetParent(_parent);
				_pool[_poolSize].transform.localScale = _prefab.transform.localScale;
                _pool[_poolSize].SetActive(false);
				result = _pool[_poolSize];
				++_poolSize;
			}

			return result;
		}

		#endregion Methods
	}
}
