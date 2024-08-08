using System;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        public partial class LogicDef
        {
            public class ConfigTransition
            {
                private GOAPAction m_Action;

                public static ConfigTransition Create(Type value, Action<GOAPAction> callback)
                {
                    var configTransition = new ConfigTransition();
                    configTransition.m_Action = new GOAPAction();
                    configTransition.m_Action.Action = LogicActionHandle.FromType(value);
                    callback.Invoke(configTransition.m_Action);
                    return configTransition;
                }

                //TODO: сделать чтобы можно было добавлять только "enum State"
                public ConfigTransition AddPreconditions<T>(T condition, bool value)
                    where T : struct, IConvertible
                {
                    m_Action.Preconditions.Add(WorldStateHandle.FromEnum(condition, value));
                    return this;
                }

                public ConfigTransition AddAction<T>(ActionType actionType)
                    where T : IAction
                {
                    var handle = LogicActionHandle.From<T>();
                    switch (actionType)
                    {
                        case ActionType.Query:
                            m_Action.QueryActions.Add(handle);
                            break;
                        case ActionType.Result:
                            m_Action.ResultActions.Add(handle);
                            break;
                    }
                    return this;
                }

                //TODO: сделать чтобы можно было добавлять только "enum State"
                public ConfigTransition AddEffect<T>(T effect, bool value)
                    where T : struct, IConvertible
                {
                    m_Action.Effects.Add(WorldStateHandle.FromEnum(effect, value));
                    return this;
                }

                public ConfigTransition Cost(float value)
                {
                    m_Action.Cost = value;
                    return this;
                }
            }
        }
    }
}
