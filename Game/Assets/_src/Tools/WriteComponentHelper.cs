using System;
using System.Reflection;

namespace Unity.Entities
{
    public static class WriteComponentHelper
    {
        private static readonly MethodInfo m_WriterAddComponent = typeof(WriteComponentHelper).GetMethod(nameof(InternalWriterAddComponent), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo m_WriterAddBuffer = typeof(WriteComponentHelper).GetMethod(nameof(InternalWriterAddBuffer), BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo m_EcbAddComponent = typeof(WriteComponentHelper).GetMethod(nameof(InternalEcbAddComponent), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo m_EcbAddBuffer = typeof(WriteComponentHelper).GetMethod(nameof(InternalEcbAddBuffer), BindingFlags.Static | BindingFlags.NonPublic);
        
        public static void AddComponent(this EntityCommandBuffer.ParallelWriter self, int sortKey, Entity entity, object data, Type dataType)
        {
            var type = data.GetType();
            var method = (type.IsArray) ? m_WriterAddBuffer : m_WriterAddComponent;
            method = method.MakeGenericMethod(dataType);
            if (type.IsArray && ((object[]) data).Length == 0)
                return;
            method.Invoke(null, new[] {self, sortKey, entity, data});
        }

        public static void AddComponent(this EntityCommandBuffer self, Entity entity, object data, Type dataType)
        {
            var type = data.GetType();
            var method = (type.IsArray) ? m_EcbAddBuffer : m_EcbAddComponent;
            method = method.MakeGenericMethod(dataType);
            if (type.IsArray && ((object[]) data).Length == 0)
                return;
            method.Invoke(null, new[] {self, entity, data});
        }
        
        private static void InternalWriterAddComponent<T>(EntityCommandBuffer.ParallelWriter writer, int sortKey, Entity entity, T data)
            where T : unmanaged, IComponentData
        {
            writer.AddComponent(sortKey, entity, data);
        }

        private static void InternalWriterAddBuffer<T>(EntityCommandBuffer.ParallelWriter writer, int sortKey, Entity entity, T[] data)
            where T : unmanaged, IBufferElementData
        {
            var buff = writer.AddBuffer<T>(sortKey, entity);
            foreach (var iter in data)
            {
                buff.Add(iter);
            } 
        }

        private static void InternalEcbAddComponent<T>(EntityCommandBuffer writer, Entity entity, T data)
            where T : unmanaged, IComponentData
        {
            writer.AddComponent(entity, data);
        }

        private static void InternalEcbAddBuffer<T>(EntityCommandBuffer writer, Entity entity, T[] data)
            where T : unmanaged, IBufferElementData
        {
            var buff = writer.AddBuffer<T>(entity);
            foreach (var iter in data)
            {
                buff.Add(iter);
            } 
        }
    }
}