using System;
using System.Collections.Generic;

namespace Trees
{
    public enum TreeType
    {
        BTree,
        BST
    }

    public class ColumnIndex
    {
        private SortedDictionary<string, List<string>> bstIndex; // Usando BST (SortedDictionary)
        private BTree bTreeIndex; // Usando B-Tree

        public TreeType TreeType { get; private set; }

        public ColumnIndex(TreeType treeType)
        {
            TreeType = treeType;
            bstIndex = new SortedDictionary<string, List<string>>(); // Inicialización predeterminada
            bTreeIndex = new BTree(3); // Inicialización predeterminada

            if (treeType == TreeType.BST)
            {
                bstIndex = new SortedDictionary<string, List<string>>();
            }
            else if (treeType == TreeType.BTree)
            {
                bTreeIndex = new BTree(3); // Grado mínimo del B-Tree
            }
        }

        // Insertar un valor en el índice
        public void Insert(string value, string position)
        {
            if (TreeType == TreeType.BST)
            {
                if (!bstIndex.ContainsKey(value))
                {
                    bstIndex[value] = new List<string>();
                }
                bstIndex[value].Add(position);
            }
            else if (TreeType == TreeType.BTree)
            {
                bTreeIndex.Insert(value, position);
            }
        }

        // Obtener todas las entradas del índice
        public Dictionary<string, List<string>> GetEntries()
        {
            if (TreeType == TreeType.BST)
            {
                return new Dictionary<string, List<string>>(bstIndex);
            }
            else if (TreeType == TreeType.BTree)
            {
                return bTreeIndex.GetEntries();
            }
            return new Dictionary<string, List<string>>();
        }

        // Buscar un valor en el índice
        public List<string> Search(string value)
        {
            if (TreeType == TreeType.BST)
            {
                if (bstIndex.ContainsKey(value))
                {
                    return bstIndex[value];
                }
            }
            else if (TreeType == TreeType.BTree)
            {
                return bTreeIndex.Search(value);
            }
            return new List<string>();
        }

        // Verificar si un valor existe en el índice
        public bool Contains(string value)
        {
            if (TreeType == TreeType.BST)
            {
                return bstIndex.ContainsKey(value);
            }
            else if (TreeType == TreeType.BTree)
            {
                return bTreeIndex.Contains(value);
            }
            return false;
        }
    }

    // Implementación básica de B-Tree
    public class BTreeNode
    {
        public List<string> Keys { get; set; }
        public List<List<string>> Positions { get; set; }
        public List<BTreeNode> Children { get; set; }
        public bool IsLeaf { get; set; }

        public BTreeNode(bool isLeaf)
        {
            Keys = new List<string>();
            Positions = new List<List<string>>();
            Children = new List<BTreeNode>();
            IsLeaf = isLeaf;
        }
    }

    public class BTree
    {
        private int T;
        private BTreeNode Root;

        public BTree(int t)
        {
            T = t;
            Root = new BTreeNode(true);
        }

        public void Insert(string key, string position)
        {
            if (Root.Keys.Count == 2 * T - 1)
            {
                BTreeNode newRoot = new BTreeNode(false);
                newRoot.Children.Add(Root);
                SplitChild(newRoot, 0, Root);
                Root = newRoot;
            }
            InsertNonFull(Root, key, position);
        }

        private void SplitChild(BTreeNode parent, int index, BTreeNode fullChild)
        {
            BTreeNode newChild = new BTreeNode(fullChild.IsLeaf);
            parent.Children.Insert(index + 1, newChild);
            parent.Keys.Insert(index, fullChild.Keys[T - 1]);
            parent.Positions.Insert(index, fullChild.Positions[T - 1]);

            newChild.Keys.AddRange(fullChild.Keys.GetRange(T, T - 1));
            newChild.Positions.AddRange(fullChild.Positions.GetRange(T, T - 1));
            fullChild.Keys.RemoveRange(T - 1, T);
            fullChild.Positions.RemoveRange(T - 1, T);

            if (!fullChild.IsLeaf)
            {
                newChild.Children.AddRange(fullChild.Children.GetRange(T, T));
                fullChild.Children.RemoveRange(T, T);
            }
        }

        private void InsertNonFull(BTreeNode node, string key, string position)
        {
            int i = node.Keys.Count - 1;
            if (node.IsLeaf)
            {
                node.Keys.Add(string.Empty); // Inicializar con un valor no nulo
                node.Positions.Add(new List<string>());
                while (i >= 0 && string.Compare(key, node.Keys[i]) < 0)
                {
                    node.Keys[i + 1] = node.Keys[i];
                    node.Positions[i + 1] = node.Positions[i];
                    i--;
                }
                node.Keys[i + 1] = key;
                node.Positions[i + 1].Add(position);
            }
            else
            {
                while (i >= 0 && string.Compare(key, node.Keys[i]) < 0)
                {
                    i--;
                }
                i++;
                if (node.Children[i].Keys.Count == 2 * T - 1)
                {
                    SplitChild(node, i, node.Children[i]);
                    if (string.Compare(key, node.Keys[i]) > 0)
                    {
                        i++;
                    }
                }
                InsertNonFull(node.Children[i], key, position);
            }
        }

        public List<string> Search(string key)
        {
            return Search(Root, key);
        }

        private List<string> Search(BTreeNode node, string key)
        {
            int i = 0;
            while (i < node.Keys.Count && string.Compare(key, node.Keys[i]) > 0)
            {
                i++;
            }
            if (i < node.Keys.Count && key == node.Keys[i])
            {
                return node.Positions[i];
            }
            if (node.IsLeaf)
            {
                return new List<string>();
            }
            return Search(node.Children[i], key);
        }

        public bool Contains(string key)
        {
            return Search(key).Count > 0;
        }

        public Dictionary<string, List<string>> GetEntries()
        {
            var entries = new Dictionary<string, List<string>>();
            GetEntries(Root, entries);
            return entries;
        }

        private void GetEntries(BTreeNode node, Dictionary<string, List<string>> entries)
        {
            if (node == null) return;

            for (int i = 0; i < node.Keys.Count; i++)
            {
                if (!entries.ContainsKey(node.Keys[i]))
                {
                    entries[node.Keys[i]] = new List<string>();
                }
                entries[node.Keys[i]].AddRange(node.Positions[i]);
            }

            if (!node.IsLeaf)
            {
                for (int i = 0; i <= node.Keys.Count; i++)
                {
                    GetEntries(node.Children[i], entries);
                }
            }
        }
    }
}