using Nickel;

namespace APurpleApple;

internal interface IModCard
{
    static abstract void Register(IModHelper helper);
}

internal interface IModArtifact
{
    static abstract void Register(IModHelper helper);
    static virtual void AfterDBInit(IModHelper helper) { }
}
