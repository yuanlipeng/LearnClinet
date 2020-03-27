//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public BoxColliderComp boxColliderComp { get { return (BoxColliderComp)GetComponent(GameComponentsLookup.BoxColliderComp); } }
    public bool hasBoxColliderComp { get { return HasComponent(GameComponentsLookup.BoxColliderComp); } }

    public void AddBoxColliderComp(int newColliderX) {
        var index = GameComponentsLookup.BoxColliderComp;
        var component = (BoxColliderComp)CreateComponent(index, typeof(BoxColliderComp));
        component.ColliderX = newColliderX;
        AddComponent(index, component);
    }

    public void ReplaceBoxColliderComp(int newColliderX) {
        var index = GameComponentsLookup.BoxColliderComp;
        var component = (BoxColliderComp)CreateComponent(index, typeof(BoxColliderComp));
        component.ColliderX = newColliderX;
        ReplaceComponent(index, component);
    }

    public void RemoveBoxColliderComp() {
        RemoveComponent(GameComponentsLookup.BoxColliderComp);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherBoxColliderComp;

    public static Entitas.IMatcher<GameEntity> BoxColliderComp {
        get {
            if (_matcherBoxColliderComp == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.BoxColliderComp);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherBoxColliderComp = matcher;
            }

            return _matcherBoxColliderComp;
        }
    }
}
