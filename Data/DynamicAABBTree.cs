using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public class DynamicAABBTree<T>
    {
        public class TreeNode<T2> //TODO: make private
        {
            public TreeNode(int id)
            {
                ID = id;
            }
            public Rectangle2D AABB;
            public T2 Data;
            public TreeNode<T2> Parent = null;
            public TreeNode<T2> Child1 = null;
            public TreeNode<T2> Child2 = null;
            public int Height = 0;
            public int ID = -1;
            public int Flags = 0;

            public bool IsLeaf
            {
                get { return Child1 == null; }
            }
            
            public bool IsBranch
            {
                get { return Child1 != null; }
            }

            public void SwapChild(TreeNode<T2> child, TreeNode<T2> replacement)
            {
                if (Child1 == child)
                    Child1 = replacement;
                else
                {
                    Debug.Assert(Child2 == child);
                    Child2 = replacement;
                }
                replacement.Parent = this;
            }
        }

        List<TreeNode<T>> _nodes = new List<TreeNode<T>>(16);
        Stack<int> _freeNodes = new Stack<int>();
        TreeNode<T> _root = null;
        int _insertionCount = 0;

        private int AllocateNode()
        {
            if (_freeNodes.Count > 0)
            {
                int id = _freeNodes.Pop();
                //Debug.Assert(WalkTree().Any(n => n.ID == id) == false);
                var node = _nodes[id];
                node.Parent = null;
                node.Child1 = null;
                node.Child2 = null;
                node.Data = default(T);
                node.Height = 0;
                node.Flags = 0;
                return id;
            }
            else
            {
                int id = _nodes.Count;
                _nodes.Add(new TreeNode<T>(id));
                return id;
            }
        }

        private void FreeNode(int nodeID)
        {
            _nodes[nodeID].Data = default(T);
            _freeNodes.Push(nodeID);
        }

        private void InsertLeaf(int nodeID)
        {
            _insertionCount++;
            if(_root == null)
            {
                _root = _nodes[nodeID];
                _root.Parent = null;
                return;
            }

            //find the best sibling for this node
            var leaf = _nodes[nodeID];
            var node = _root;
            while (node.IsBranch)
            {
                var child1 = node.Child1;
                var child2 = node.Child2;

                float combinedSize = (leaf.AABB + node.AABB).Perimeter;
                // Cost of creating a new parent for this node and the new leaf
                float newBranchSize = 2 * combinedSize;
                // Minimum cost of pushing the leaf further down the tree
                float inheritanceSize = 2 * (combinedSize - leaf.AABB.Perimeter);
                
                // Cost of descending into child1
                float size1 = inheritanceSize + (leaf.AABB + child1.AABB).Perimeter;
                if (child1.IsBranch)
                    size1 -= child1.AABB.Perimeter; //cost reduced by old perimeter

                // Cost of descending into child2
                float size2 = inheritanceSize + (leaf.AABB + child2.AABB).Perimeter;
                if (child2.IsBranch)
                    size2 -= child2.AABB.Perimeter; //cost reduced by old perimeter

                //create ne branch here?
                if (newBranchSize < size1 && newBranchSize < size2)
                    break;

                //descend
                node = (size1 < size2) ? child1 : child2;
            }

            //create a new parent
            var oldParent = node.Parent;
            int newParentID = AllocateNode();
            var newParent = _nodes[newParentID];
            newParent.Parent = oldParent;
            newParent.AABB = node.AABB + leaf.AABB;
            newParent.Flags = node.Flags | leaf.Flags;
            newParent.Child1 = node;
            newParent.Child2 = leaf;
            node.Parent = newParent;
            leaf.Parent = newParent;
            if (oldParent == null) //e.g _root == node
                _root = newParent;
            else if (oldParent.Child1 == node)
                oldParent.Child1 = newParent;
            else
                oldParent.Child2 = newParent;

            newParent.Height = node.Height + 1;
            UpdateAncestors(newParent);
        }

        private void UpdateAncestors(TreeNode<T> node)
        {
            //fix heights and aabs up to root
            while (node != null)
            {
                node = Balance(node);
                node.Height = 1 + Math.Max(node.Child1.Height, node.Child2.Height);
                node.AABB = node.Child1.AABB + node.Child2.AABB;
                node.Flags = node.Child1.Flags | node.Child2.Flags;
                node = node.Parent;
            }
        }

        private void RemoveLeaf(int nodeID)
        {
            var leaf = _nodes[nodeID];
            if (_root == leaf)
            {
                _root = null;
                return;
            }
            
            var parent = leaf.Parent;
            var sibling = (parent.Child1 == leaf) ? parent.Child2 : parent.Child1;
            var grandparent = parent.Parent;
            if(grandparent == null)
            {
                //destroy parent and make sibling is the new root
                _root = sibling;
                sibling.Parent = null;
                FreeNode(parent.ID);
                return;
            }

            //destroy parent and connect sibling with grandparent
            grandparent.SwapChild(parent, sibling);
            
            FreeNode(parent.ID);
            UpdateAncestors(grandparent);
        }

        private TreeNode<T> Balance(TreeNode<T> node)
        {
            if (node.IsLeaf || node.Height < 2)
                return node;

            int balance = node.Child2.Height - node.Child1.Height;
            if(balance > 1)
                return Rotate(node, node.Child2, node.Child1);
            else if (balance < -1)
                return Rotate(node, node.Child1, node.Child2);
            else
                return node;
        }

        private TreeNode<T> Rotate(TreeNode<T> node, TreeNode<T> child, TreeNode<T> other)
        {
            var cc1 = child.Child1;
            var cc2 = child.Child2;
            var parent = node.Parent;
            //append node at it's child
            child.Child1 = node;
            child.Parent = parent;
            node.Parent = child;

            //node's old parent should now point to the child
            if (parent == null)
                _root = child;
            else if (parent.Child1 == node)
                parent.Child1 = child;
            else
                parent.Child2 = child;

            //rotate
            if(cc1.Height > cc2.Height)
            {
                child.Child2 = cc1;
                node.SwapChild(child, cc2);

                node.AABB = other.AABB + cc2.AABB;
                node.Flags = other.Flags | cc2.Flags;
                node.Height = 1 + Math.Max(other.Height, cc2.Height);
                
                child.AABB = node.AABB + cc1.AABB;
                child.Flags = node.Flags | cc1.Flags;
                child.Height = 1 + Math.Max(node.Height, cc1.Height);
            }
            else
            {
                child.Child2 = cc2;
                node.SwapChild(child, cc1);

                node.AABB = other.AABB + cc1.AABB;
                node.Flags = other.Flags | cc1.Flags;
                node.Height = 1 + Math.Max(other.Height, cc1.Height);

                child.AABB = node.AABB + cc2.AABB;
                child.Flags = node.Flags | cc2.Flags;
                child.Height = 1 + Math.Max(node.Height, cc2.Height);
            }
            
            return child;
        }
        
        //*************
        //**  PUBLIC  **
        //*************

        public int Insert(T data, Rectangle2D aabb, int flags = 0)
        {
            Debug.Assert(data != null);
            int proxyID = AllocateNode();
            _nodes[proxyID].AABB = aabb;
            _nodes[proxyID].Data = data;
            _nodes[proxyID].Flags = flags;
            InsertLeaf(proxyID);
            return proxyID;
        }

        public void Move(int proxyID, Rectangle2D aabb)
        {
            RemoveLeaf(proxyID);
            _nodes[proxyID].AABB = aabb;
            InsertLeaf(proxyID);
        }

        public void Remove(int proxyID)
        {
            RemoveLeaf(proxyID);
            FreeNode(proxyID);
        }

        public Rectangle2D GetAABB(int proxyID)
        {
            return _nodes[proxyID].AABB;
        }

        public T GetData(int proxyID)
        {
            return _nodes[proxyID].Data;
        }

        //**************
        //**  PUBLIC  **
        //**************

        Stack<TreeNode<T>> _queryStack = new Stack<TreeNode<T>>(128);

        public IEnumerable<T> Query(int flags)
        {
            if (_root == null)
                yield break;

            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                var node = _queryStack.Pop();
                if ((node.Flags & flags) != flags)
                    continue;

                if (node.IsBranch)
                {
                    _queryStack.Push(node.Child1);
                    _queryStack.Push(node.Child2);
                }
                else
                    yield return node.Data;
            }
        }

        public delegate bool QueryPredicate(Rectangle2D aabb);

        public IEnumerable<T> Query(QueryPredicate filter)
        {
            if (_root == null)
                yield break;

            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                var node = _queryStack.Pop();
                if (!filter(node.AABB))
                    continue;

                if (node.IsBranch)
                {
                    _queryStack.Push(node.Child1);
                    _queryStack.Push(node.Child2);
                }
                else
                    yield return node.Data;
            }
        }

        public IEnumerable<T> Query(QueryPredicate filter, int flags)
        {
            if (_root == null)
                yield break;

            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                var node = _queryStack.Pop();
                if ((node.Flags & flags) != flags)
                    continue;

                if (!filter(node.AABB))
                    continue;

                if (node.IsBranch)
                {
                    _queryStack.Push(node.Child1);
                    _queryStack.Push(node.Child2);
                }
                else
                    yield return node.Data;
            }
        }

        public struct Pair<T>
        {
            public Pair(T a, T b)
            {
                A = a;
                B = b;
            }
            public T A;
            public T B;
        }

        Stack<TreeNode<T>> _resultStack = new Stack<TreeNode<T>>(32);

        public IEnumerable<Pair<T>> EnumerateOverlappingLeafs()
        {
            if (_root == null)
                yield break;

            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                var node = _queryStack.Pop();
                if (node.IsBranch)
                {
                    _queryStack.Push(node.Child2);
                    _queryStack.Push(node.Child1);
                }
                else //backtrack only first childs
                {
                    //test the node against all other nodes that haven't been visited yet
                    _resultStack.Clear();
                    Backtrack(node);
                    foreach (var other in _resultStack)
                        yield return new Pair<T>(node.Data, other.Data);
                }
            }
        }

        public IEnumerable<Pair<T>> EnumerateOverlappingLeafs(int firstMask, int secondMask)
        {
            if (_root == null)
                yield break;

            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                var node = _queryStack.Pop();
                if ((node.Flags & firstMask) != firstMask && (node.Flags & secondMask) != secondMask)
                    continue;

                if (node.IsBranch)
                {
                    _queryStack.Push(node.Child2);
                    _queryStack.Push(node.Child1);
                }
                else //backtrack only first childs
                {
                    //test the node against all other nodes that haven't been visited yet
                    _resultStack.Clear();
                    var current = node;
                    var parent = node.Parent;
                    if ((node.Flags & firstMask) == firstMask)
                    {
                        while (parent != null)
                        {
                            if (parent.Child1 == current)
                                Expand(node, parent.Child2, secondMask); //child1 has been visited

                            current = parent;
                            parent = current.Parent;
                        }
                        foreach (var other in _resultStack)
                            yield return new Pair<T>(node.Data, other.Data);
                    }
                    else if ((node.Flags & secondMask) == secondMask)
                    {
                        while (parent != null)
                        {
                            if (parent.Child1 == current)
                                Expand(node, parent.Child2, firstMask); //child1 has been visited

                            current = parent;
                            parent = current.Parent;
                        }
                        foreach (var other in _resultStack)
                            yield return new Pair<T>(other.Data, node.Data);
                    }
                }
            }
        }

        private void Backtrack(TreeNode<T> node)
        {
            var current = node;
            var parent = node.Parent;
            while (parent != null)
            {
                if (parent.Child1 == current)
                    Expand(node, parent.Child2); //child1 has been visited

                current = parent;
                parent = current.Parent;
            }
        }

        private void Expand(TreeNode<T> reference, TreeNode<T> node)
        {
            if (!reference.AABB.Overlaps(node.AABB))
                return;//discard this branch/leaf! (cause all node's childs will be contained in node's AABB
            if (node.IsBranch)
            {
                Expand(reference, node.Child2);
                Expand(reference, node.Child1);
            }
            else
                _resultStack.Push(node); //overlapping leaf! store it on the result stack!
        }

        private void Expand(TreeNode<T> reference, TreeNode<T> node, int mask)
        {
            if ((node.Flags & mask) != mask)
                return;

            if (!reference.AABB.Overlaps(node.AABB))
                return;//discard this branch/leaf! (cause all node's childs will be contained in node's AABB

            if (node.IsBranch)
            {
                Expand(reference, node.Child2, mask);
                Expand(reference, node.Child1, mask);
            }
            else
                _resultStack.Push(node); //overlapping leaf! store it on the result stack!
        }
        /*
        public IEnumerable<Pair<T>> EnumerateOverlappingLeafs()
        {
            if (_root == null)
                yield break;

            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                var node = _queryStack.Pop();
                if (node.IsBranch)
                {
                    _queryStack.Push(node.Child2);
                    _queryStack.Push(node.Child1);
                }
                else //backtrack only first childs
                {
                    //test the node against all other nodes that haven't been visited yet
                    var current = node;
                    var parent = node.Parent;
                    while (parent != null)
                    {
                        int cnt = _queryStack.Count;
                        if (parent.Child1 == current)
                            _queryStack.Push(parent.Child2);
                        while (_queryStack.Count > cnt)
                        {
                            var other = _queryStack.Pop();
                            if (!node.AABB.Overlaps(other.AABB))
                                continue;
                            if (other.IsBranch)
                            {
                                _queryStack.Push(other.Child2);
                                _queryStack.Push(other.Child1);
                            }
                            else
                                yield return new Pair<T>(node.Data, other.Data);
                        }
                        current = parent;
                        parent = current.Parent;
                    }
                }
            }
        }
        */
        /*
        public int LookupProxyID(T data) //stupidly expensive - just remember it next time, m'kay?
        {
            for (int i = 0; i < _nodes.Count; i++)
                if(EqualityComparer<T>.Default.Equals(_nodes[i].Data, data))
                    if (!_freeNodes.Contains(i))
                        return i;

            return -1;
        }
         */

        //*************
        //**  DEBUG  **
        //*************

        public int NodeCount
        {
            get { return _nodes.Count - _freeNodes.Count; }
        }

        public int TreeHeight
        {
            get { return (_root != null) ? _root.Height : -1; }
        }

        public TreeNode<T> Root //TODO: remove
        {
            get { return _root; }
        }

        public TreeNode<T> GetNode(int proxyID)
        {
            return _nodes[proxyID];
        }

        public IEnumerable<TreeNode<T>> WalkTree()
        {
            if (_root == null)
                yield break;

            _queryStack.Clear();
            _queryStack.Push(_root);
            while (_queryStack.Count > 0)
            {
                var node = _queryStack.Pop();
                yield return node;
                if (node.IsBranch)
                {
                    _queryStack.Push(node.Child2);
                    _queryStack.Push(node.Child1);
                }
            }
        }
        
	    /// Validate this tree. For testing.
	    public void Validate()
        {
            if(_root != null)
            {
                ValidateStructure(_root);
                ValidateMetrics(_root);
                Debug.Assert(TreeHeight == ComputeHeight(_root));
            }
        }

        private void ValidateMetrics(TreeNode<T> node)
        {
            if(node.IsBranch)
            {
                int height = 1 + Math.Max(node.Child1.Height, node.Child2.Height);
                Debug.Assert(node.Height == height);
                Rectangle2D aabb = node.Child1.AABB + node.Child2.AABB;
                Debug.Assert(node.AABB == aabb);
                ValidateMetrics(node.Child1);
                ValidateMetrics(node.Child2);
            }
        }

        private void ValidateStructure(TreeNode<T> node)
        {
            if (node == _root)
                Debug.Assert(node.Parent == null);

            if (node.IsLeaf)
            {
                Debug.Assert(node.Child1 == null);
                Debug.Assert(node.Child2 == null);
                Debug.Assert(node.Height == 0);
                return;
            }

            Debug.Assert(node.Child1.Parent == node);
            Debug.Assert(node.Child2 .Parent == node);
            ValidateStructure(node.Child1);
            ValidateStructure(node.Child2);
        }

        private int ComputeHeight(TreeNode<T> node)
        {
            if(node.IsLeaf)
                return 0;

            return 1 + Math.Max(ComputeHeight(node.Child1), ComputeHeight(node.Child2));
        }

	    /// Get the maximum balance of an node in the tree. The balance is the difference
	    /// in height of the two children of a node.
	    public int ComputeTreeBalance()
        {
            int maxBalance = 0;
            foreach (var node in WalkTree())
                if(node.IsBranch)
                {
                    int balance = Math.Abs(node.Child2.Height - node.Child1.Height);
                    maxBalance = Math.Max(maxBalance, balance);
                }
            return maxBalance;
        }

        public int ComputeLeafCount()
        {
            return WalkTree().Where(n => n.IsLeaf).Count();
        }

	    /// Get the ratio of the sum of the node perimeters to the root perimeter.
	    public float ComputeTreeQuality()
        {
            if(_root == null)
                return 0.0f;

            float rootSize = _root.AABB.Perimeter;
            float nodeSizeSum = 0.0f;
            foreach (var node in WalkTree())
                nodeSizeSum += node.AABB.Perimeter;
            return nodeSizeSum / rootSize;
        }

	    /// Build an optimal tree. Very expensive. For testing.
	    void RebuildBottomUp()
        {

        }
    }
}
