namespace SimpleTrading.WebApi.Infrastructure;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SwaggerUiControllerPositionAttribute(ushort position) : Attribute
{
    public ushort Position { get; } = position;
}