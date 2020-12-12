using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhangShashaCSharp
{
    public class Convert : sexprBaseVisitor<Node>
    {
        public override Node VisitSexpr([NotNull] sexprParser.SexprContext context)
        {
            return VisitList(context.list());
        }

        public override Node VisitList([NotNull] sexprParser.ListContext context)
        {
            var atom = VisitAtom(context.atom());
            foreach (var c in context.item())
            {
                atom.children.Add(VisitItem(c));
            }
            return atom;
        }

        public override Node VisitItem([NotNull] sexprParser.ItemContext context)
        {
            var a = context.atom();
            if (a != null) return VisitAtom(a);
            else return VisitList(context.list());
        }

        public override Node VisitAtom([NotNull] sexprParser.AtomContext context)
        {
            return new Node() { label = context.GetText() };
        }
    }
}
