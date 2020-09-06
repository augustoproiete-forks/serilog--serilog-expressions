﻿using System.Collections.Generic;
using Serilog.Expressions.Ast;

namespace Serilog.Expressions.Compilation.Transformations
{
    class IdentityTransformer : SerilogExpressionTransformer<Expression>
    {
        bool TryTransform(Expression expr, out Expression result)
        {
            result = Transform(expr);
            return !ReferenceEquals(expr, result);
        }
        
        protected override Expression Transform(CallExpression lx)
        {
            var any = false;
            var operands = new List<Expression>();
            foreach (var op in lx.Operands)
            {
                if (TryTransform(op, out var result))
                    any = true;
                operands.Add(result);
            }

            if (!any)
                return lx;
            
            return new CallExpression(lx.OperatorName, operands.ToArray());
        }

        protected override Expression Transform(ConstantExpression cx)
        {
            return cx;
        }

        protected override Expression Transform(AmbientPropertyExpression px)
        {
            return px;
        }

        protected override Expression Transform(AccessorExpression spx)
        {
            if (!TryTransform(spx.Receiver, out var recv))
                return spx;
                
            return new AccessorExpression(recv, spx.MemberName);
        }

        protected override Expression Transform(LambdaExpression lmx)
        {
            if (!TryTransform(lmx.Body, out var body))
                return lmx;
            
            // By default we maintain the parameters available in the body
            return new LambdaExpression(lmx.Parameters, body);
        }

        // Only touches uses of the parameters, not decls
        protected override Expression Transform(ParameterExpression prx)
        {
            return prx;
        }

        protected override Expression Transform(IndexerWildcardExpression wx)
        {
            return wx;
        }

        protected override Expression Transform(ArrayExpression ax)
        {
            var any = false;
            var elements = new List<Expression>();
            foreach (var el in ax.Elements)
            {
                if (TryTransform(el, out var result))
                    any = true;
                elements.Add(result);
            }

            if (!any)
                return ax;

            return new ArrayExpression(elements.ToArray());
        }

        protected override Expression Transform(IndexerExpression ix)
        {
            var transformedRecv = TryTransform(ix.Receiver, out var recv);
            
            if (!TryTransform(ix.Index, out var index) && !transformedRecv)
                return ix;
            
            return new IndexerExpression(recv, index);
        }

        protected override Expression Transform(IndexOfMatchExpression mx)
        {
            if (!TryTransform(mx.Corpus, out var corpus))
                return mx;
            
            return new IndexOfMatchExpression(corpus, mx.Regex);
        }
    }
}