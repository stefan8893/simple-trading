namespace SimpleTrading.WebApi.Infrastructure;

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerUiControllerPositionAttribute(ushort position) : Attribute
{
    public ushort Position { get; } = position;
}