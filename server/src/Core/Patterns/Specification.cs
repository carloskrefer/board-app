using System.Linq.Expressions;

namespace Core.Patterns;

/// <summary>
/// Defines a set of criteria to be met by an entity. Is also able to be translated into SQL.
/// </summary>
/// <remarks>
/// <para>
/// It is an interpretation of the Specification Pattern.
/// </para>
/// <para>
/// Allows for <b>reusable</b> and <b>composable</b> business or validation rules.
/// </para>
/// <para>
/// It can be used with in-memory calculations, by using <see cref="IsSatisfiedBy"/>, because it returns a boolean.
/// </para>
/// <para>
/// It can also be used to generate SQL in database queries, by using <see cref="ToExpression"/>, because it returns an 
/// <see cref="Expression"/>.
/// </para>
/// <para>
/// If you are doing in memory calculations, you can simply compose it by using the <see cref="IsSatisfiedBy"/> and
/// then using C#'s native AND, OR, and NOT operators to combine multiple specifications, for example.
/// </para>
/// <para>
/// But when it comes to database queries, for example you must compose it by  using the <see cref="And"/>, 
/// <see cref="Or"/>, and <see cref="Not"/> methods, because it returns another <see cref="Expression"/>.
/// </para>
/// <para>
/// Important: if the expression can not be translated into SQL then this class <b>should not be used</b>. Create a  
/// global predicate instead (for the purposes of sharing common logic, unrelated to database queries) or fix your logic
/// so that it can be translated into SQL. For example, calling myRandomMethod() inside an Expression won't break the 
/// Expression itself, since it can store method references in it's data structure. But, a SQL generator will not be 
/// able to translate a compiled method reference into SQL.
/// </para>
/// <para>
/// Important: TEntity <b>must be an entity</b>, because IQueryable.Where(user => ...) requires that user is an entity, 
/// otherwise it does not know what table is being queried. So, Specification&lt;string&gt; makes no sense, while 
/// Specification&lt;User&gt; does.
/// </para>
/// </remarks>
// TODO: Check if creating a IEntity interface and restricting TEntity to it would make it more type-safe.
public abstract class Specification<TEntity>
{
    public abstract Expression<Func<TEntity, bool>> ToExpression();

    public bool IsSatisfiedBy(TEntity entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public Specification<TEntity> And(Specification<TEntity> specification) => new AndSpecification<TEntity>(this, specification);
    public Specification<TEntity> Or(Specification<TEntity> specification) => new OrSpecification<TEntity>(this, specification);
    public Specification<TEntity> Not() => new NotSpecification<TEntity>(this);
}

public class AndSpecification<TEntity>(
    Specification<TEntity> left, 
    Specification<TEntity> right) : Specification<TEntity>
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression();
        var rightExpression = right.ToExpression();

        var parameter = Expression.Parameter(typeof(TEntity));
        var body = Expression.AndAlso(
            Expression.Invoke(leftExpression, parameter),
            Expression.Invoke(rightExpression, parameter)
        );

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}

public class OrSpecification<TEntity>(
    Specification<TEntity> left, 
    Specification<TEntity> right) : Specification<TEntity>
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression();
        var rightExpression = right.ToExpression();

        var parameter = Expression.Parameter(typeof(TEntity));
        var body = Expression.OrElse(
            Expression.Invoke(leftExpression, parameter),
            Expression.Invoke(rightExpression, parameter)
        );

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}

public class NotSpecification<TEntity>(
    Specification<TEntity> specification) : Specification<TEntity>
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var expression = specification.ToExpression();

        var parameter = Expression.Parameter(typeof(TEntity));
        var body = Expression.Not(Expression.Invoke(expression, parameter));

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}
