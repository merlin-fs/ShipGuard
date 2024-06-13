using System.Linq;

using Common.Core;

using UnityEngine;

namespace Reflex.Core
{
    public static class CreateProjectContainer
    {
        public static Container Create()
        {
            var builder = new ContainerBuilder().SetName("ProjectContainer");
            builder.AddSingleton(new InitializationManager(), typeof(IInitialization));
#if !REFLEX_DISABLED
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            var activeProjectScopes = projectScopes.Where(s => s.gameObject.activeSelf);

            foreach (var projectScope in activeProjectScopes)
            {
                projectScope.InstallBindings(builder);
            }
#endif
            return builder.Build();
        }
    }
}