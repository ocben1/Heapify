using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public class Heap<K, D> where K : IComparable<K>
    {

        // This is a nested Node class whose purpose is to represent a node of a heap.
        private class Node : IHeapifyable<K, D>
        {
            // The Data field represents a payload.
            public D Data { get; set; }
            // The Key field is used to order elements with regard to the Binary Min (Max) Heap Policy, i.e. the key of the parent node is smaller (larger) than the key of its children.
            public K Key { get; set; }
            // The Position field reflects the location (index) of the node in the array-based internal data structure.
            public int Position { get; set; }

            public Node(K key, D value, int position)
            {
                Data = value;
                Key = key;
                Position = position;
            }

            // This is a ToString() method of the Node class.
            // It prints out a node as a tuple ('key value','payload','index')}.
            public override string ToString()
            {
                return "(" + Key.ToString() + "," + Data.ToString() + "," + Position + ")";
            }
        }

        // ---------------------------------------------------------------------------------
        // Here the description of the methods and attributes of the Heap<K, D> class starts

        public int Count { get; private set; }

        // The data nodes of the Heap<K, D> are stored internally in the List collection. 
        // Note that the element with index 0 is a dummy node.
        // The top-most element of the heap returned to the user via Min() is indexed as 1.
        private List<Node> data = new List<Node>();

        // We refer to a given comparer to order elements in the heap. 
        // Depending on the comparer, we may get either a binary Min-Heap or a binary  Max-Heap. 
        // In the former case, the comparer must order elements in the ascending order of the keys, and does this in the descending order in the latter case.
        private IComparer<K> comparer;

        // We expect the user to specify the comparer via the given argument.
        public Heap(IComparer<K> comparer)
        {
            this.comparer = comparer;

            // We use a default comparer when the user is unable to provide one. 
            // This implies the restriction on type K such as 'where K : IComparable<K>' in the class declaration.
            if (this.comparer == null) this.comparer = Comparer<K>.Default;

            // We simplify the implementation of the Heap<K, D> by creating a dummy node at position 0.
            // This allows to achieve the following property:
            // The children of a node with index i have indices 2*i and 2*i+1 (if they exist).
            data.Add(new Node(default(K), default(D), 0));
        }

        // This method returns the top-most (either a minimum or a maximum) of the heap.
        // It does not delete the element, just returns the node casted to the IHeapifyable<K, D> interface.
        public IHeapifyable<K, D> Min()
        {
            if (Count == 0) throw new InvalidOperationException("The heap is empty.");
            return data[1];
        }

        // Insertion to the Heap<K, D> is based on the private UpHeap() method
        public IHeapifyable<K, D> Insert(K key, D value)
        {
            Count++;
            Node node = new Node(key, value, Count);
            data.Add(node);
            UpHeap(Count);
            return node;
        }
        public IHeapifyable<K, D> Delete()
        {
            if (Count == 0) throw new InvalidOperationException("The heap is empty.");

            IHeapifyable<K, D> answer = data[1];
            Swap(1, Count); //replace the root key with the key of the last node w
            data.Remove(data[Count]); //remove w
            DownHeap(1); //restore the heap order property
            Count--;
            return answer;
        }
        //Algorithm UpHeap restores the heap-order property by swapping k along an upward path from the insertion node
        //It terminates when the key k reaches the root or a node whose parent has a key smaller than or equal to k
        //Since a heap has height O(log n), UpHeap runs in O(log n) time.
        private void UpHeap(int start)
        {
            int position = start;
            while (position != 1) //continue until reaching root (or break statement)
            {
                //if parent is bigger, swap.
                if (comparer.Compare(data[position].Key, data[position / 2].Key) < 0) Swap(position, position / 2);
                position = position / 2; //continue from parent's location
            }
        }
        protected int getLeft(int j) { return 2 * j; }
        protected int getRight(int j) { return 2 * j + 1; }
        protected int getParent(int j) { return j / 2; }
        protected bool hasLeft(int j) { return getLeft(j) < Count; }
        protected bool hasRight(int j) { return getRight(j) < Count; }
        protected bool isLeft(int j)
        {
            if (j >= Count / 2 && j <= Count)
            {
                return true;
            }
            return false;
        }

        
        //After replacing the root key with the key k of the last node, the heap order property may be violated.
        //Algorithm DownHeap restores the heap order proeprty by swapping key k along a downward path from the root.
        //DownHeap terminates when key k reaches a leaf or a node whose children have keys greater than or equal to k.
        //Since a heap has height O(log n), DownHeap runs in O(log n) time.
        //Used when we have a large element at the top and want to shuffle it down.
        private void DownHeap(int j)
        {
            while (hasLeft(j))
            {
                int leftIndex = getLeft(j);
                int smallChildIndex = leftIndex;
                if(hasRight(j))
                {
                    int rightIndex = getRight(j);
                    if(comparer.Compare(data[leftIndex].Key, data[rightIndex].Key) > 0)
                    {
                        smallChildIndex = rightIndex;
                    }
                }
                if (comparer.Compare(data[smallChildIndex].Key, data[j].Key) >= 0)
                {
                    break;
                }
                Swap(j, smallChildIndex);
                j = smallChildIndex;
           }
                
    }
    private void Swap(int from, int to)
    {
        Node temp = data[from];
        data[from] = data[to];
        data[to] = temp;
        data[to].Position = to;
        data[from].Position = from;
    }

    public void Clear()
    {
        for (int i = 0; i <= Count; i++) data[i].Position = -1;
        data.Clear();
        data.Add(new Node(default(K), default(D), 0));
        Count = 0;
    }

    public override string ToString()
    {
        if (Count == 0) return "[]";
        StringBuilder s = new StringBuilder();
        s.Append("[");
        for (int i = 0; i < Count; i++)
        {
            s.Append(data[i + 1]);
            if (i + 1 < Count) s.Append(",");
        }
        s.Append("]");
        return s.ToString();
    }

        // TODO: Your task is to implement all the remaining methods.
        // Read the instruction carefully, study the code examples from above as they should help you to write the rest of the code.


        // Builds a minimum binary heap using the specified data according to the bottom-up approach.
        //Assume the heap property holds for all subtrees of height h. Then we can establish the heap property for height h+1 by
        //DownHeap

        //Merging Two Heaps
        //Given two heaps and a key k, we create a new heap with the root node storing k and with the heaps as subtrees.
        //We perform DownHeap to restore the heap-order property.
        protected void heapify()
        {
            int startIndex = getParent(Count - 1);
            for (int j = startIndex; j >= 0; j--)
            {
                DownHeap(j);
            }
        }
        public IHeapifyable<K, D>[] BuildHeap(K[] keys, D[] data)
        {
            if (Count == 0) Clear();
            else throw new Exception("The Heap is empty.");
            
            IHeapifyable<K, D>[] Interface = new IHeapifyable<K, D>[Count+1];
            for(int i = 0; i < Math.Min(keys.Length, data.Length); i++)
            {
                Interface[i] = new Node(keys[i], data[i], i);
                
            }
            heapify();
            //Console.WriteLine(Result);
            return Interface;
        }        
        
        public void DecreaseKey(IHeapifyable<K, D> element, K new_key)
        {
            // You should replace this plug by your code.
            throw new NotImplementedException();
        }

    }
}
