using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
    public struct Map<TKey, TValue>: IEnumerable<KeyValuePair<TKey, TValue>>, IDisposable
        where TKey : unmanaged, IComparable<TKey>
        where TValue: unmanaged
    {
        DRedBlackTree<TKey, TValue> m_Tree;

        public Map(int capacity, AllocatorManager.AllocatorHandle allocator, bool insertAlways)
        {
            m_Tree = new DRedBlackTree<TKey, TValue>(capacity, allocator, insertAlways);
        }

        public void Dispose()
        {
            m_Tree.Dispose();
        }

        public void Add(TKey key, TValue value)
        {
            DRedBlackTree<TKey, TValue>.DPair pair;
            pair.Key = key;
            pair.Value = value;
            m_Tree.Insert(pair);
        }

        public bool ContainsKey(TKey key)
        {
            var iter = m_Tree.Lower_bound(key);
            return !m_Tree.atEnd(iter);
        }

        public bool Remove(TKey key)
        {
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var iter = m_Tree.Find(key);
            unsafe
            {
                value = iter.Node->pair.Value;
            }
            return !m_Tree.atEnd(iter);
        }

        public bool TryGetValues(TKey key, out IEnumerable<TValue> values)
        {
            var low = m_Tree.Find(key);
            var high = m_Tree.Upper_bound(key);
            values = !m_Tree.atEnd(low)
                ? new Enumerable(low, high) 
                : (new TValue[] { });

            return !m_Tree.atEnd(low);
        }

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(m_Tree.Start(), m_Tree.Finish());
        }

        private struct Enumerable : IEnumerable<TValue>
        {
            private DRedBlackTree<TKey, TValue>.DIterator m_Start;
            private DRedBlackTree<TKey, TValue>.DIterator m_Finish;

            public Enumerable(DRedBlackTree<TKey, TValue>.DIterator start, DRedBlackTree<TKey, TValue>.DIterator finaish)
            {
                m_Start = start;
                m_Finish = finaish;
            }

            IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();

            public IEnumerator<TValue> GetEnumerator()
            {
                return new EnumeratorValues(m_Start, m_Finish);
            }

        }

        private unsafe struct Enumerator: IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
        {
            private DRedBlackTree<TKey, TValue>.DIterator m_Start;
            private DRedBlackTree<TKey, TValue>.DIterator m_Finish;
            private DRedBlackTree<TKey, TValue>.DIterator m_Iter;

            public Enumerator(DRedBlackTree<TKey, TValue>.DIterator start, DRedBlackTree<TKey, TValue>.DIterator finaish)
            {
                m_Start = start;
                m_Iter = default;
                m_Finish = finaish;
            }

            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(m_Iter.Node->pair.Key, m_Iter.Node->pair.Value);

            object IEnumerator.Current => Current;
            
            public bool MoveNext()
            {
                if (m_Iter.Node == null)
                {
                    m_Iter = m_Start;
                    return !m_Start.Tree.atEnd(m_Iter);
                }
                else
                {
                    m_Start.Tree.Advance(ref m_Iter);
                    return !m_Start.Tree.atEnd(m_Iter) && m_Iter.Node != m_Finish.Node;
                }
            }

            public void Reset()
            {
                m_Iter = default;
            }

            public void Dispose() { }

        }

        private unsafe struct EnumeratorValues : IEnumerator<TValue>, IEnumerator, IDisposable
        {
            private DRedBlackTree<TKey, TValue>.DIterator m_Start;
            private DRedBlackTree<TKey, TValue>.DIterator m_Finish;
            private DRedBlackTree<TKey, TValue>.DIterator m_Iter;

            public EnumeratorValues(DRedBlackTree<TKey, TValue>.DIterator start, DRedBlackTree<TKey, TValue>.DIterator finaish)
            {
                m_Start = start;
                m_Iter = default;
                m_Finish = finaish;
            }

            public TValue Current => m_Iter.Node->pair.Value;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (m_Iter.Node == null)
                {
                    m_Iter = m_Start;
                    return !m_Start.Tree.atEnd(m_Iter);
                }
                else
                {
                    m_Start.Tree.Advance(ref m_Iter);
                    return !m_Start.Tree.atEnd(m_Iter) && m_Iter.Node != m_Finish.Node;
                }
            }

            public void Reset()
            {
                m_Iter = default;
            }

            public void Dispose() { }
        }
    }

    public readonly unsafe struct DRedBlackTree<TKey, TValue> : IDisposable
        where TKey: unmanaged, IComparable<TKey>
        where TValue: unmanaged
    {
        public struct DPair
        {
            public TKey Key;
            public TValue Value;
        }

        private struct Data: IDisposable
        {
            private DTreeNode* m_Ptr;
            private DTreeNode m_null_node;
            private DTreeNode m_Header;

            public AllocatorManager.AllocatorHandle Allocator;
            public DTreeNode* Header => (DTreeNode*)UnsafeUtility.AddressOf(ref m_Header);
            public DTreeNode* null_node => (DTreeNode*)UnsafeUtility.AddressOf(ref m_null_node);
            public bool InsertAlways;
            public int NodeCount;
            public int Capacity;
            
            public void Init(AllocatorManager.AllocatorHandle allocator)
            {
                Allocator = allocator;
                m_Header = new DTreeNode();
                m_null_node = new DTreeNode();
                NodeCount = 0;
                Capacity = 0;
                m_Ptr = null;
            }

            public void Dispose()
            {
                AllocatorManager.Free(Allocator, m_Ptr, Capacity);
            }

            public DTreeNode* Add(DPair pair, ref DTreeNode* x, ref DTreeNode* y)
            {
                if (NodeCount + 1 > Capacity)
                {
                    var offset = SetCapacity(ref Allocator, Capacity + 1);
                    if (x != null_node && x != Header)
                        x = (DTreeNode*)((ulong)x + offset);
                    if (y != null_node && y != Header)
                        y = (DTreeNode*)((ulong)y + offset);
                }
                var node = m_Ptr + NodeCount;
                node->pair = pair;
                NodeCount++;
                return node;

            }

            public void SetCapacity(int value)
            {
                SetCapacity(ref Allocator, value);
            }

            private unsafe ulong Realloc(ref AllocatorManager.AllocatorHandle allocator, int newCapacity)
            {
                DTreeNode* ptr = null;
                ulong offset = 0;
                int alignOf = UnsafeUtility.AlignOf<DTreeNode>();
                int num = sizeof(DTreeNode);
                if (newCapacity > 0)
                {
                    ptr = (DTreeNode*)AllocatorManager.Allocate(allocator, num, alignOf, newCapacity);
                    if (Capacity > 0)
                    {
                        int num2 = math.min(newCapacity, Capacity);
                        int num3 = num2 * num;
                        UnsafeUtility.MemCpy(ptr, m_Ptr, num3);
                        offset = (ulong)ptr - (ulong)m_Ptr;
                    }
                }
                AllocatorManager.Free(allocator, m_Ptr, Capacity);
                m_Ptr = ptr;
                if (offset != 0) Remap(offset);
                Capacity = newCapacity;
                return offset;
            }

            private void Remap(ulong offset)
            {
                int num = sizeof(DTreeNode);
                RemapItem(Header, null_node, Header);
                for (int i = 0; i < NodeCount; i++) 
                {
                    DTreeNode* dest = m_Ptr + i;
                    RemapItem(dest, null_node, Header); 
                }

                void RemapItem(DTreeNode* node, DTreeNode* null_node, DTreeNode* header_node)
                {
                    if (node->left != null_node && node->left != header_node)
                        node->left = (DTreeNode*)((ulong)node->left + offset);

                    if (node->right != null_node && node->right != header_node)
                        node->right = (DTreeNode*)((ulong)node->right + offset);
                    if (node->parent != null_node && node->parent != header_node)
                        node->parent = (DTreeNode*)((ulong)node->parent + offset);
                }
            }

            private void Realloc(int capacity)
            {
                if (capacity < NodeCount)
                    throw new ArgumentOutOfRangeException(nameof(capacity));
                Realloc(ref Allocator, capacity);
            }

            private unsafe ulong SetCapacity(ref AllocatorManager.AllocatorHandle allocator, int capacity)
            {
                ulong offset = 0;
                int num = sizeof(DTreeNode);
                int x = math.max(capacity, 64 / num);
                x = math.ceilpow2(x);
                if (x != Capacity)
                {
                    offset = Realloc(ref allocator, x);
                }
                return offset;
            }
        }

        private readonly Data* m_Data;
        public int Size => m_Data->NodeCount;
        private DTreeNode* Header => m_Data->Header;
        private DTreeNode* null_node => m_Data->null_node;

        public DRedBlackTree(int capacity, AllocatorManager.AllocatorHandle allocator, bool insertAlways)
        {
            m_Data = AllocatorManager.Allocate<Data>(allocator);
            m_Data->Init(allocator);
            m_Data->SetCapacity(capacity);

            m_Data->InsertAlways = insertAlways;

            m_Data->Header->parent = null_node;
            m_Data->Header->left = m_Data->Header;
            m_Data->Header->right = m_Data->Header;
            m_Data->Header->color = DColor.Red;
        }

        public void Dispose()
        {
            AllocatorManager.Free(m_Data->Allocator, m_Data);
        }

        public DIterator Start()
        {
            DIterator result = default;
            result.Flags = (Size == 0)
                    ? DIteratorFlag.diBidirectional | DIteratorFlag.diMarkFinish
                    : DIteratorFlag.diBidirectional | DIteratorFlag.diMarkStart;
            result.Tree = this;
            result.Node = Header->left;
            return result;
        }

        public DIterator Finish()
        {
            DIterator result = default;
            result.Flags = DIteratorFlag.diBidirectional | DIteratorFlag.diMarkFinish;
            result.Tree = this;
            result.Node = Header;
            return result;
        }

        void RBInternalInsert(DTreeNode* x, DTreeNode* y, DPair pair)
        {
            var z = m_Data->Add(pair, ref x, ref y);
            bool toLeft = (y == m_Data->Header || x != null_node || z->pair.Key.CompareTo(y->pair.Key) < 0);
            RBinsert(toLeft, x, y, z);
        }

        public bool Insert(DPair pair)
        {
            var y = Header;
            var x = Header->parent;
            bool comp = true;

            while (x != null_node) 
            {
                y = x;
                int cmp = pair.Key.CompareTo(x->pair.Key);
                if (cmp == 0 && !m_Data->InsertAlways)
                {
                    x->pair.Value = pair.Value;
                    return true;
                }
                comp = cmp < 0;
                x = comp 
                    ? x->left 
                    : x->right;
            }

            if (m_Data->InsertAlways)
            {
                RBInternalInsert(x, y, pair);
                return true;
            }
            var j = y;
            if (comp)
            {
                if (j == Header->left)
                {
                    RBInternalInsert(x, y, pair);
                    return true;
                }
                else
                    RBDecrement(ref j);
            };

            if (j->pair.Key.CompareTo(pair.Key) < 0)
            {
                RBInternalInsert(x, y, pair);
                return true;
            }
            return false;
        }

        /*

        void RBEraseTree(DTreeNode node, bool direct)
        {
            if (node != DTreeNode.Null)
            {
                RBEraseTree(node.left, direct);
                RBEraseTree(node.right, direct);
                if direct then
                    node.kill
                else
                    node.free;
            }
        }

        void erase(bool direct)
        {
            RBEraseTree(m_Header.parent, direct);
            m_Header.left = m_Header;
            m_Header.parent = DTreeNode.Null;
            m_Header.right = m_Header;
            m_NodeCount = 0;
        }

        void eraseAt(DIterator pos)
        {
            //assert(not atEnd(pos));
            var node = RBerase(pos.Node);
            m_NodeCount--;
            //node.free;
        }

        int eraseKeyN(TKey obj, int count)
        {
            DRange p = equal_range(obj);

            if (p.start.Node == p.finish.Node)
                return 0;
            else
            {
                // assert(not atEnd(p.start), 'Cannot erase non-existent key');
                if (count != int.MaxValue)
                {
                    int result = distance(p.start, p.finish);
                    if (result > count)
                        retreatBy(p.finish, result - count);
                }
                return eraseIn(p.start, p.finish);
            }
        }
        int eraseKey(TKey obj)
        {
            return eraseKeyN(obj, int.MaxValue);
        }

        int eraseIn(DIterator start, DIterator finish)
        {
            var result = 0;
            if (equals(start, this.start()) && equals(finish, this.finish()))
                erase(false);
            else
                while (!equals(start, finish))
                {
                    var iter = advanceF(start);
                    eraseAt(start);
                    result++;
                    start = iter;
                }
            return result;
        }
        */

        public DIterator Find(TKey obj)
        {
            var value = Lower_bound(obj);
            return atEnd(value) || obj.CompareTo(value.Node->pair.Key) < 0 
                ? Finish() 
                : value;
        }

        public DIterator Lower_bound(TKey obj)
        {
            var y = Header;
            var x = Header->parent;
            var comp = false;

            int i = 0;

            while (x != null_node)
            {
                i++;
                y = x;
                comp = x->pair.Key.CompareTo(obj) < 0;
                x = comp
                    ? x->right
                    : x->left;
            }
            
            var result = Start();

            if (!atEnd(result))
            {
                result.Node = y;
                if (Start().Node != result.Node)
                    result.Flags &= ~(DIteratorFlag.diMarkStart | DIteratorFlag.diMarkFinish);
                if (Finish().Node == result.Node)
                    result.Flags &= ~DIteratorFlag.diMarkStart | DIteratorFlag.diMarkFinish;

                if (comp)
                    Advance(ref result);
            }
            return result;
        }

        public DIterator Upper_bound(TKey obj)
        {
            var y = Header;
            var x = y->parent;
            var comp = true;

            while (x != null_node)
            {
                y = x;
                comp = obj.CompareTo(x->pair.Key) < 0;
                x = comp 
                    ? x->left 
                    : x->right;
            }

            var result = Start();
            if (!atEnd(result))
            {
                result.Node = y;
                if (Start().Node != result.Node)
                    result.Flags &= ~(DIteratorFlag.diMarkStart | DIteratorFlag.diMarkFinish);
                if (Finish().Node == result.Node)
                    result.Flags &= ~DIteratorFlag.diMarkStart | DIteratorFlag.diMarkFinish;

                if (!comp)
                    Advance(ref result);
            }
            return result;
        }

        /*
        DRange equal_range(TKey obj)
        {
            var result = new DRange
            {
                start = lower_bound(obj),
                finish = upper_bound(obj),
            };
            return result;
        }
        */

        public void Advance(ref DIterator iterator)
        {
            iterator.Tree.RBincrement(ref iterator.Node);
            if (iterator.Node == iterator.Tree.endNode)
                iterator.Flags |= DIteratorFlag.diMarkFinish & ~DIteratorFlag.diMarkStart;
            else
                iterator.Flags &= ~(DIteratorFlag.diMarkFinish | DIteratorFlag.diMarkStart);
        }

        /*
        bool equals(DIterator iter1, DIterator iter2)
        {
            return iter1.Node == iter2.Node;
        }
        */

        public bool atEnd(DIterator iter)
        {
            return (iter.Flags & DIteratorFlag.diMarkFinish) != 0;
        }

        void RBincrement(ref DTreeNode* node)
        {
            if (node->right != null_node)
            {
                node = node->right;
                while (node->left != null_node)
                    node = node->left;
            }
            else
            {
                var y = node->parent;
                while (node == y->right)
                {
                    node = y;
                    y = y->parent;
                }
                if (node->right != y)
                    node = y;
            }
        }

        void RBDecrement(ref DTreeNode* node)
        {
            if (node->color == DColor.Red && node->parent->parent == node)
                node = node->right;
            else if (node->left != null_node)
            {
                var y = node->left;
                while (y->right != null_node)
                    y = y->right;
                node = y;
            }
            else
            {
                var y = node->parent;
                while (node == y->left)
                {
                    node = y;
                    y = y->parent;
                }
                node = y;
            }
        }

        void RBLeftRotate(DTreeNode* node)
        {
            var y = node->right;
            node->right = y->left;
            if (y->left != null_node)
                y->left->parent = node;
            y->parent = node->parent;
            if (node == Header->parent)
                Header->parent = y;
            else if (node == node->parent->left)
                node->parent->left = y;
            else
                node->parent->right = y;
            y->left = node;
            node->parent = y;
        }

        void RBRightRotate(DTreeNode* node)
        {
            var y = node->left;
            node->left = y->right;
            if (y->right != null_node)
                y->right->parent = node;
            y->parent = node->parent;
            if (node == Header->parent)
                Header->parent = y;
            else if (node == node->parent->right)
                node->parent->right = y;
            else
                node->parent->left = y;
            y->right = node;
            node->parent = y;
        }

        void RBinsert(bool insertToLeft, DTreeNode* x, DTreeNode* y, DTreeNode* z)
        {
            if (insertToLeft)
            {
                y->left = z;
                if (y == Header)
                {
                    Header->parent = z;
                    Header->right = z;
                }
                else if (y == Header->left)
                    Header->left = z;
            }
            else
            {
                y->right = z;
                if (y == Header->right)
                    Header->right = z;
            }


            z->parent = y;
            z->left = null_node;
            z->right = null_node;
            x = z;
            x->color = DColor.Red;

            while (x != Header->parent && x->parent->color == DColor.Red)
            {
                if (x->parent == x->parent->parent->left)
                {
                    y = x->parent->parent->right;
                    if (y->color == DColor.Red)
                    {
                        x->parent->color = DColor.Black;
                        y->color = DColor.Black;
                        x->parent->parent->color = DColor.Red;
                        x = x->parent->parent;
                    }
                    else
                    {
                        if (x == x->parent->right)
                        {
                            x = x->parent;
                            RBLeftRotate(x);
                        }
                        x->parent->color = DColor.Black;
                        x->parent->parent->color = DColor.Red;
                        RBRightRotate(x->parent->parent);
                    }
                }
                else
                {
                    y = x->parent->parent->left;
                    if (y->color == DColor.Red)
                    {
                        x->parent->color = DColor.Black;
                        y->color = DColor.Black;
                        x->parent->parent->color = DColor.Red;
                        x = x->parent->parent;
                    }
                    else
                    {
                        if (x == x->parent->left)
                        {
                            x = x->parent;
                            RBRightRotate(x);
                        }
                        x->parent->color = DColor.Black;
                        x->parent->parent->color = DColor.Red;
                        RBLeftRotate(x->parent->parent);
                    }
                }
            }
            Header->parent->color = DColor.Black;
        }

        DTreeNode* startNode => Header->left;
 
        DTreeNode* endNode => Header;

        [Flags]
        public enum DIteratorFlag
        {
            diSimple = 0,
            diForward = 1 << 0,
            diBidirectional = 1 << 1,
            diRandom = 1 << 2,
            diMarkStart = 1 << 3,
            diMarkFinish = 1 << 4,
            diKey = 1 << 5,
            diIteration = 1 << 6,
        };

        public struct DIterator
        {
            public DIteratorFlag Flags;
            public DRedBlackTree<TKey, TValue> Tree;
            public DTreeNode* Node;
        }

        public enum DColor { Black, Red };

        public unsafe struct DTreeNode
        {
            public DPair pair;
            public DColor color;

            public DTreeNode* left;
            public DTreeNode* right;
            public DTreeNode* parent;

            public DTreeNode(DPair pair)
            {
                this.pair = pair;
                color = default;
                left = null;
                right = null;
                parent = null;
            }
        }
    }
}