using System;

using Game.Core.Repositories;

using Reflex.Attributes;

using UnityEngine;

namespace Game.Views
{
    public class InteractionViewComponent : MonoBehaviour, IViewComponent
    {
        [Inject] private InteractionViewRepository m_InteractionViewRepository;
        private Collider m_Collider;
        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            if (!m_Collider)
                throw new Exception($"For InteractionViewComponent need Collider");
        }

        public void Initialization(IView view)
        {
            m_InteractionViewRepository.Insert(m_Collider.GetInstanceID(), view);
        }

        private void OnDestroy()
        {
            m_InteractionViewRepository.Remove(m_Collider.GetInstanceID());
        }
    }
}
