// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq.Expressions;

namespace Coddee
{
    /// <summary>
    /// Helper class for working with <see cref="Expression"/> objects
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// Returns the name of a member expression
        /// <remarks>e.g. GetMember(()=>Name) return "Name"</remarks>
        /// </summary>
        public static string GetMemberName(LambdaExpression expression)
        {
            var body = expression.Body;
            {
                if (body is MemberExpression member)
                {
                    return member.Member.Name;
                }
            }
            {
                if (body is UnaryExpression unary)
                {
                    if (unary.Operand is MemberExpression member)
                        return member.Member.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the type of a member expression
        /// <remarks>e.g.
        /// string Name;
        /// GetMember(()=>Name) return <see langword="typeof"/>(string)</remarks>
        /// </summary>
        public static Type GetMemberType<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body;
            {
                if (body is MemberExpression member)
                {
                    return member.Member.DeclaringType;
                }
            }
            {
                if (body is UnaryExpression unary)
                {
                    if (unary.Operand is MemberExpression member)
                        return member.Member.DeclaringType;
                }
            }

            return null;
        }
    }
}
