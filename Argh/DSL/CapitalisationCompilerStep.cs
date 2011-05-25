namespace Argh.DSL
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;

    public class CapitalisationCompilerStep : AbstractVisitorCompilerStep
    {
        /// <summary>
        /// Called when we encounter a reference expression
        /// </summary>
        /// <param name="node">The node.</param>
        public override void OnReferenceExpression(ReferenceExpression node)
        {
            if (this.ShouldTransformNodeName(node))
            {
                this.TransformNodeName(node);
            }

            base.OnReferenceExpression(node);
        }

        /// <summary>
        /// Determain if the the name of the node should be transformed.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected virtual bool ShouldTransformNodeName(ReferenceExpression node)
        {
            var firstLetter = node.Name[0];

            return firstLetter != char.ToUpperInvariant(firstLetter);
        }

        /// <summary>
        /// Called when we encounters a member reference expression
        /// </summary>
        /// <param name="node">The node.</param>
        public override void OnMemberReferenceExpression(MemberReferenceExpression node)
        {
            if (this.ShouldTransformNodeName(node))
            {
                this.TransformNodeName(node);
            }

            base.OnMemberReferenceExpression(node);
        }

        /// <summary>
        /// Sets the node name to pascal case.
        /// </summary>
        /// <param name="node">The node.</param>
        protected virtual void TransformNodeName(ReferenceExpression node)
        {
            var nameCharacters = node.Name.ToCharArray();

            nameCharacters[0] = char.ToUpperInvariant(nameCharacters[0]);

            node.Name = new string(nameCharacters);
        }

        /// <summary>
        /// Start visiting the current compile unit
        /// </summary>
        public override void Run()
        {
            Visit(CompileUnit);
        }
    }
}