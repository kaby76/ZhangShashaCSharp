namespace ZhangShashaCSharp
{
	using Antlr4.Runtime;
	using System;
	using System.Collections.Generic;

	public class Tree
	{

		Node root;
		// function l() which gives the leftmost leaf for a given node (identified by post-order number).
		List<int> l = new List<int>();
		// list of keyroots for a tree root, i.e., nodes with a left sibling, or in the case of the root, the root itself.
		List<int> keyroots = new List<int>();
		// list of the labels of the nodes used for node comparison
		List<string> labels = new List<string>();

		// the following constructor handles s-expression notation. E.g., ( f a ( b c ) )
		public Tree(String s)
		{
			var str = new AntlrInputStream(s);
			var lexer = new sexprLexer(str);
			var tokens = new CommonTokenStream(lexer);
			var parser = new sexprParser(tokens);
			var tree = parser.sexpr().list();
			var visitor = new Convert();
			root = visitor.Visit(tree);
		}

		public void Traverse()
		{
			// put together an ordered list of node labels of the tree
			Traverse(root, labels);
		}

		private static List<string> Traverse(Node node, List<string> labels)
		{
			for (int i = 0; i < node.children.Count; i++)
			{
				labels = Traverse(node.children[i], labels);
			}
			labels.Add(node.label);
			return labels;
		}

		public void ComputePostOrderNumber()
		{
			// index each node in the tree according to traversal method
			ComputePostOrderNumber(root, 0);
		}

		private static int ComputePostOrderNumber(Node node, int index)
		{
			for (int i = 0; i < node.children.Count; i++)
			{
				index = ComputePostOrderNumber(node.children[i], index);
			}
			index++;
			node.postorder_number = index;
			return index;
		}

		public void ComputeLeftMostLeaf()
		{
			// put together a function which gives l()
			Leftmost();
			l = ComputeLeftMostLeaf(root, new List<int>());
		}

		private List<int> ComputeLeftMostLeaf(Node node, List<int> l)
		{
			for (int i = 0; i < node.children.Count; i++)
			{
				l = ComputeLeftMostLeaf(node.children[i], l);
			}
			l.Add(node.leftmost.postorder_number);
			return l;
		}

		private void Leftmost()
		{
			Leftmost(root);
		}

		private static void Leftmost(Node node)
		{
			if (node == null)
				return;
			for (int i = 0; i < node.children.Count; i++)
			{
				Leftmost(node.children[i]);
			}
			if (node.children.Count == 0)
			{
				node.leftmost = node;
			}
			else
			{
				node.leftmost = node.children[0].leftmost;
			}
		}

		public void Keyroots()
		{
			// calculate the keyroots
			for (int i = 0; i < l.Count; i++)
			{
				int flag = 0;
				for (int j = i + 1; j < l.Count; j++)
				{
					if (l[j] == l[i])
					{
						flag = 1;
					}
				}
				if (flag == 0)
				{
					this.keyroots.Add(i + 1);
				}
			}
		}

		static int[,] tree_dist;
		static List<Operation>[,] toperations;

		public static (int, List<Operation>) ZhangShasha(Tree tree1, Tree tree2)
		{
			tree1.ComputePostOrderNumber();
			tree1.ComputeLeftMostLeaf();
			tree1.Keyroots();
			tree1.Traverse();

			tree2.ComputePostOrderNumber();
			tree2.ComputeLeftMostLeaf();
			tree2.Keyroots();
			tree2.Traverse();

			List<int> l1 = tree1.l;
			List<int> keyroots1 = tree1.keyroots;
			List<int> l2 = tree2.l;
			List<int> keyroots2 = tree2.keyroots;

			// space complexity of the algorithm
			tree_dist = new int[l1.Count + 1, l2.Count + 1];
			toperations = new List<Operation>[l1.Count + 1, l2.Count + 1];
			for (int m = 0; m < l1.Count + 1; ++m)
				for (int n = 0; n < l2.Count + 1; ++n)
					toperations[m, n] = new List<Operation>();

			// solve subproblems
			for (int i1 = 1; i1 < keyroots1.Count + 1; i1++)
			{
				for (int j1 = 1; j1 < keyroots2.Count + 1; j1++)
				{
					int i = keyroots1[i1 - 1];
					int j = keyroots2[j1 - 1];
					(tree_dist[i, j], toperations[i, j]) = Treedist(l1, l2, i, j, tree1, tree2);
				}
			}

			return (tree_dist[l1.Count, l2.Count], toperations[l1.Count, l2.Count]);
		}

		private static (int, List<Operation>) Treedist(List<int> l1, List<int> l2, int i, int j, Tree tree1, Tree tree2)
		{
			int[,] forestdist = new int[i + 1, j + 1];
			List<Operation>[,] foperations = new List<Operation>[i + 1, j + 1];
			for (int m = 0; m < i + 1; ++m)
				for (int n = 0; n < j + 1; ++n)
					foperations[m, n] = new List<Operation>();

			// costs of the three atomic operations
			int Delete = 1;
			int Insert = 1;
			int Relabel = 1;

			forestdist[0, 0] = 0;
			forestdist[l1[i - 1] - 1, 0] = 0;
			forestdist[0, l2[j - 1] - 1] = 0;

			for (int i1 = l1[i - 1]; i1 <= i; i1++)
			{
				forestdist[i1, 0] = forestdist[i1 - 1, 0] + Delete;
				foperations[i1, 0] = new List<Operation>(foperations[i1 - 1, 0]);
				foperations[i1, 0].Add(new Operation() { O = Operation.Op.Delete });
			}
			for (int j1 = l2[j - 1]; j1 <= j; j1++)
			{
				forestdist[0, j1] = forestdist[0, j1 - 1] + Insert;
				foperations[0, j1] = new List<Operation>(foperations[0, j1 - 1]);
				foperations[0, j1].Add(new Operation() { O = Operation.Op.Insert });
			}
			for (int i1 = l1[i - 1]; i1 <= i; i1++)
			{
				for (int j1 = l2[j - 1]; j1 <= j; j1++)
				{
					int i_temp = (l1[i - 1] > i1 - 1) ? 0 : i1 - 1;
					int j_temp = (l2[j - 1] > j1 - 1) ? 0 : j1 - 1;
					if ((l1[i1 - 1] == l1[i - 1]) && (l2[j1 - 1] == l2[j - 1]))
					{
						int Cost = (tree1.labels[i1 - 1].Equals(tree2.labels[j1 - 1])) ? 0 : Relabel;
						
						int test1;
						List<Operation> list1;
						if (forestdist[i_temp, j1] + Delete > forestdist[i1, j_temp] + Insert)
                        {
							test1 = forestdist[i1, j_temp] + Insert;
							list1 = new List<Operation>(foperations[i1, j_temp]);
							list1.Add(new Operation() { O = Operation.Op.Insert });
							if (list1.Count != test1) throw new Exception();
						}
						else
                        {
							test1 = forestdist[i_temp, j1] + Delete;
							list1 = new List<Operation>(foperations[i_temp, j1]);
							list1.Add(new Operation() { O = Operation.Op.Delete });
							if (list1.Count != test1) throw new Exception();
						}

						int test2;
						List<Operation> list2;
						if (test1 > forestdist[i_temp, j_temp] + Cost)
						{
							test2 = forestdist[i_temp, j_temp] + Cost;
							list2 = new List<Operation>(foperations[i_temp, j_temp]);
							if (Cost > 0)
								list2.Add(new Operation() { O = Operation.Op.Change });
							if (list2.Count != test2) throw new Exception();
						}
						else
						{
							test2 = test1;
							list2 = new List<Operation>(list1);
							if (list2.Count != test2) throw new Exception();
						}

						var temp = Math.Min(
							Math.Min(forestdist[i_temp, j1] + Delete, forestdist[i1, j_temp] + Insert),
							forestdist[i_temp, j_temp] + Cost);
						if (test2 != temp) throw new Exception();

						forestdist[i1, j1] = test2;
						foperations[i1, j1] = list2;
						if (temp != test2) throw new Exception();
						
						tree_dist[i1, j1] = test2;
						toperations[i1, j1] = list2;
					}
					else
					{
						int i1_temp = l1[i1 - 1] - 1;
						int j1_temp = l2[j1 - 1] - 1;

						int i_temp2 = (l1[i - 1] > i1_temp) ? 0 : i1_temp;
						int j_temp2 = (l2[j - 1] > j1_temp) ? 0 : j1_temp;

						int test1;
						List<Operation> list1;
						if (forestdist[i_temp, j1] + Delete > forestdist[i1, j_temp] + Insert)
						{
							test1 = forestdist[i1, j_temp] + Insert;
							list1 = new List<Operation>(foperations[i1, j_temp]);
							list1.Add(new Operation() { O = Operation.Op.Insert });
							if (list1.Count != test1) throw new Exception();
						}
						else
						{
							test1 = forestdist[i_temp, j1] + Delete;
							list1 = new List<Operation>(foperations[i_temp, j1]);
							list1.Add(new Operation() { O = Operation.Op.Delete });
							if (list1.Count != test1) throw new Exception();
						}

						int test2;
						List<Operation> list2;
						if (test1 > forestdist[i_temp2, j_temp2] + tree_dist[i1, j1])
						{
							test2 = forestdist[i_temp2, j_temp2] + tree_dist[i1, j1];
							list2 = new List<Operation>(foperations[i_temp2, j_temp2]);
							list2.AddRange(toperations[i1, j1]);
							if (list2.Count != test2) throw new Exception();
						}
						else
						{
							test2 = test1;
							list2 = new List<Operation>(list1);
							if (list2.Count != test2) throw new Exception();
						}

						var temp = Math.Min(
							Math.Min(forestdist[i_temp, j1] + Delete, forestdist[i1, j_temp] + Insert),
							forestdist[i_temp2, j_temp2] + tree_dist[i1, j1]);
						if (test2 != temp) throw new Exception();

						forestdist[i1, j1] = test2;
						foperations[i1, j1] = list2;
					}
				}
			}
			return (forestdist[i, j], toperations[i, j]);
		}
	}
}

