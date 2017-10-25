using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Coddee
{
    public class ExpressionHelper
    {
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
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
