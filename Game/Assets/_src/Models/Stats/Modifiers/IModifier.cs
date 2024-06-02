using System;
using Unity.Entities;

namespace Game.Model.Stats
{
    public interface IModifier
    {
        public delegate void Execute(Entity entity, ref StatValue stat, float delta);
        
        void Estimation(Entity entity, ref Stat stat, float delta);
        void Attach(Entity entity);
        void Dettach(Entity entity);
    }
}