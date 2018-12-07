using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Pools;

namespace EcsRx.Lookups
{
    public class LookupList<TK, TV> : ILookupList<TK, TV>
    {
        public readonly IndexPool IndexPool = new IndexPool();
        public readonly Dictionary<TK, int> Lookups = new Dictionary<TK, int>();
        public readonly List<TV> InternalList = new List<TV>();

        public IEnumerator<TV> GetEnumerator() => Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => Lookups.Count;
        public bool ContainsKey(TK key) => Lookups.ContainsKey(key);      
        public TV this[int index] => InternalList[index];

        public ICollection<TK> Keys => Lookups.Keys;
        public IEnumerable<TV> Values => InternalList.Where(x => x != null);

        public TV GetByKey(TK key) => InternalList[Lookups[key]];

        public void Add(TK key, TV value)
        {
            var nextIndex = IndexPool.AllocateInstance();
            Lookups.Add(key, nextIndex);

            if (nextIndex < InternalList.Count)
            { InternalList[nextIndex] = value; }
            else
            { InternalList.Add(value); }
        }

        public bool Remove(TK key)
        {
            if (!Lookups.ContainsKey(key)) 
            { return false;}
            
            var index = Lookups[key];
            InternalList[index] = default(TV);
            Lookups.Remove(key);
            IndexPool.ReleaseInstance(index);
            return true;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            if (!Lookups.ContainsKey(key))
            {
                value = default(TV);
                return false;
            }

            value = InternalList[Lookups[key]];
            return true;
        }

        public void Clear()
        {
            InternalList.Clear();
            Lookups.Clear();
            IndexPool.Clear();
        }
    }
}