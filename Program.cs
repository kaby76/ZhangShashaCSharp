using System;

namespace ZhangShashaCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
			// Sample trees (in preorder).
			string tree1Str1 = "( f ( d a ( c b ) ) e )";
			string tree1Str2 = "( f ( c ( d a b ) ) e )"; // Nuke c, add c
			// Distance: 2 (main example used in the Zhang-Shasha paper)

			string tree1Str3 = "( f ( d a ( c b ) ) e )";
			string tree1Str4 = "( f ( d a ( c x ) ) e )"; // Change b
			// Distance: 1

			String tree1Str5 = "( d )";
			String tree1Str6 = "( g h )";
			// Distance: 2

			Tree tree1 = new Tree(tree1Str1);
			Tree tree2 = new Tree(tree1Str2);

			Tree tree3 = new Tree(tree1Str3);
			Tree tree4 = new Tree(tree1Str4);

			Tree tree5 = new Tree(tree1Str5);
			Tree tree6 = new Tree(tree1Str6);

			var (distance1, list1) = Tree.ZhangShasha(tree1, tree2);
			System.Console.WriteLine("Expected 2; got " + distance1);
			foreach (var o in list1)
			{
				System.Console.WriteLine(o.O.ToString() + " " + o.N1 + " => " + o.N2);
			}

			var (distance2, list2) = Tree.ZhangShasha(tree3, tree4);
			System.Console.WriteLine("Expected 1; got " + distance2);
			foreach (var o in list2)
			{
				System.Console.WriteLine(o.O.ToString() + " " + o.N1 + " => " + o.N2);
			}

			var (distance3, list3) = Tree.ZhangShasha(tree5, tree6);
			System.Console.WriteLine("Expected 2; got " + distance3);
			foreach (var o in list3)
			{
				System.Console.WriteLine(o.O.ToString() + " " + o.N1 + " => " + o.N2);
			}
		}
	}
}
