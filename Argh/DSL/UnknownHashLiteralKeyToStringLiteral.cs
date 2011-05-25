namespace Argh.DSL
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;

    public class UnknownHashLiteralKeyToStringLiteral : ProcessMethodBodiesWithDuckTyping
    {
        public override void OnReferenceExpression(ReferenceExpression node)
        {
            IEntity entity = NameResolutionService.Resolve(node.Name);
            //search for the left side of a key in a hash literal expression
            if (node.ParentNode is ExpressionPair
                    && ((ExpressionPair)node.ParentNode).First == node
                    && node.ParentNode.ParentNode is HashLiteralExpression)
            {
                ExpressionPair parent = (ExpressionPair)node.ParentNode;
                StringLiteralExpression literal = CodeBuilder.CreateStringLiteral(node.Name);
                parent.First = literal;
                parent.Replace(node, literal);
                return;
            }
            base.OnReferenceExpression(node);
        }
    }
}