using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Translation
{
    internal class Cache<T>
    {
        private ConcurrentDictionary<int, T> indexValuePairs = [];
        private ConcurrentDictionary<int,LinkedListNode<int>> indexNodes = [];
        private LinkedList<int> availableIndexes = new LinkedList<int>();
        private SemaphoreSlim availableIndexesSemaphore = new SemaphoreSlim(1, 1);

        public async Task<(bool,T?)> TryGetIndex(int index)
        {
            if(indexValuePairs.TryGetValue(index,out T result))
            {
                var node = indexNodes[index];
                await availableIndexesSemaphore.WaitAsync();
                availableIndexes.Remove(node);
                availableIndexes.AddLast(node);
                availableIndexesSemaphore.Release();
                return (true,result);
            }

            return (false,default(T));
        }

        public async Task Insert(int index,T item)
        {
            await availableIndexesSemaphore.WaitAsync();
            if (availableIndexes.Count >= 1000)
            {
                var previousNode = availableIndexes.First;
                availableIndexes.RemoveFirst();
                indexValuePairs.Remove(previousNode.Value,out _);
                indexNodes.Remove(previousNode.Value,out _);
            }

            var newNode = availableIndexes.AddLast(index);
            indexNodes.TryAdd(index,newNode);
            indexValuePairs.TryAdd(index,item);
            availableIndexesSemaphore.Release();
        }
    }
}
