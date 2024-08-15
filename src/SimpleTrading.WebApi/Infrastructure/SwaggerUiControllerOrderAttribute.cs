namespace SimpleTrading.WebApi.Infrastructure;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SwaggerUiControllerOrderAttribute(ushort position) : Attribute
{
    public ushort Position { get; } = position;
}