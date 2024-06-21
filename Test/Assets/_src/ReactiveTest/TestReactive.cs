using System;
using System.ComponentModel;

using Reactives;

using TMPro;
using UniMob;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


namespace TestReactives
{

    public interface IStorageScope
    {
        public Entity Entity { get; }
    }

    public class TestReactive : MonoBehaviour, ILifetimeScope
    {
        public TMP_Text counterText;
        public Button incrementButton;

        // declare reactive property
        [Reactive] private IReactive<StoreModel> m_StoreModel;
        [Reactive] private IReactive<StoreModel> m_StoreModel2;

        private int m_Count;

        public struct StoreModelWritable : IReactiveWritable<StoreModel>
        {
            public ref StoreModel Value => 
        }

        private void Awake()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;


            var entity = manager.CreateEntity(new ComponentType[] 
            {
                typeof(UserStorageDataTag),
                typeof(UserStorageDataChangeTag),
                typeof(DataChangeEvent),
                typeof(StoreModel),
            });

            m_StoreModel  = new ReactiveValue<StoreModel>(entity);
            m_StoreModel2 = new ReactiveValue<StoreModel>(entity);
            m_Count = 0;
        }

        protected void Start()
        {
            var storeModel = m_StoreModel;
            m_StoreModel.Subscribe(() => counterText.text = $"Tap count: {storeModel.Value.ID}, call: {++m_Count}");

            using (var write = m_StoreModel.Writable<>())
            {
                write.Value.ID = 10;
                write.Value.ID = 20;
            }

            using (var write = m_StoreModel2.Writable())
            {
                write.Value.ID = 30;
            }



            //var test = Atom.When() 
            //IObservable<int> tets;
            //tets.Subscribe(_ => Lifetime.CreateNested());

            // increment Counter on button click
            //incrementButton.onClick.AddListener(() => Counter = 1);

            // Update counterText when Counter changed until Lifetime terminated
            //Atom.Reaction(Lifetime, () => counterText.text = "Tap count: " + Counter);
        }

        public Lifetime Lifetime => default(Lifetime);
    }
}