using Game.AI.GOAP;
using Game.Model.Units;

namespace Game.Model.AI
{
    public class LogicPA_Warior: Logic.ILogicConfig
    {
        public void Initialize(Logic.LogicDef logic)
        {
            /*
            1. Находит цель
            2. выбирает точку возле цели. Цель должна быть в радиусе действия оружия. 
            3. перемещается к выбранной точке
            4. Кокда цель будет в радиусе действия оружия - останавливается 
            5. Активирует оружие
            6. начинает атаку.
            7. повтор...
            */
        
            logic.AddTransition<WeaponsActivate>()
                .AddPreconditions(Unit.State.WeaponInRange, false)
                .AddEffect(Unit.State.WeaponInRange, true)
                .Cost(0);

            //поиск цели
            logic.AddTransition<FindScanner>()
                .AddAction<UnitFindEnemyQuery>(Logic.ActionType.Query)
                .AddPreconditions(Target.State.HasPossibleTargets, true)
                .AddPreconditions(Target.State.Found, false)
                .AddEffect(Target.State.Found, true)
                .Cost(2);
                
            /*
            //установка точки возле цели, в радиусе действия оружия.
            logic.CustomAction<Unit, Unit.FindPathRadius>()
                .AfterChangeState(Target.State.Found, true);
            */

            /*
            //поиск пути к выбранной точке
            logic.AddTransition(Move.Action.FindPath)
                .AddPreconditions(Target.State.Found, true)
                .AddEffect(Move.State.PathFound, true)
                .Cost(1);
            */

            /*
            //перемещение к выбранной точке, 
            logic.AddTransition(Move.Action.MoveToPoint)
                .AddPreconditions(Move.State.PathFound, true)
                .AddPreconditions(Unit.State.WeaponInRange, false)
                .AddEffect(Move.State.MoveDone, true)
                .AddEffect(Unit.State.WeaponInRange, true)
                .Cost(1);
            */
            /*
            //атака
            logic.AddTransition(Unit.Action.Attack)
                .AddPreconditions(Move.State.MoveDone, true)
                .AddPreconditions(Unit.State.WeaponInRange, true)
                .AddEffect(Target.State.Dead, true)
                .Cost(1);

            logic.AddTransition(Global.Action.Destroy)
                .AddPreconditions(Global.State.Dead, false)
                .Cost(0);

            logic.EnqueueGoal(Move.State.Init, true);
            logic.EnqueueGoal(Unit.State.WeaponInRange, true);
            logic.EnqueueGoalRepeat(Target.State.Dead, true);
            */
        }
    }
}