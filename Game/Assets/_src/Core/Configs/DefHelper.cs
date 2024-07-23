﻿using System;
using System.Collections.Concurrent;
using System.Reflection;

using System.Runtime.InteropServices;

using Common.Defs;

using Game.Views;

using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.VisualScripting;

using UnityEngine;

namespace Game.Core.Defs
{
    public static class DefHelper
    {
        private static readonly ConcurrentDictionary<int, ConstructorDefinable> m_Defs = new();
        private delegate void SetDef<T>(RefLink<T> link);

        private unsafe readonly struct ConstructorDefinable
        {
            private readonly delegate* <void*, void*, void> m_SetDefMethodPtr;
            private readonly delegate* <void*, Entity, IDefinableContext, void> m_AddComponentMethodPtr;
            private readonly delegate* <void*, Entity, IDefinableContext, void> m_RemoveComponentMethodPtr;
            private readonly delegate* <void*, IView, Entity, void> m_InitializationViewMethodPtr;
            
            private readonly RefLink m_Link;
            private readonly void* m_AddrLink;

            public ConstructorDefinable(MethodBase setDefMethod, 
                MethodBase addMethod, MethodBase removeMethod, 
                MethodBase initializationViewMethod, object link)
            {
                m_SetDefMethodPtr = (delegate* <void*, void*, void>)setDefMethod.MethodHandle.GetFunctionPointer();
                
                m_AddComponentMethodPtr = (delegate* <void*, Entity, IDefinableContext, void>)(addMethod?.MethodHandle.GetFunctionPointer() ?? IntPtr.Zero);
                m_RemoveComponentMethodPtr = (delegate* <void*, Entity, IDefinableContext, void>)(removeMethod?.MethodHandle.GetFunctionPointer() ?? IntPtr.Zero);
                m_InitializationViewMethodPtr = (delegate* <void*, IView, Entity, void>)(initializationViewMethod?.MethodHandle.GetFunctionPointer() ?? IntPtr.Zero);
                m_Link = UnsafeUtility.As<object, RefLink>(ref link);
                m_AddrLink = UnsafeUtility.AddressOf(ref m_Link);
            }

            public void FillDefinable(void* data)
            {
                m_SetDefMethodPtr(data, m_AddrLink);
            }

            public void AddComponentData(void* data, Entity entity, IDefinableContext context)
            {
                if (m_AddComponentMethodPtr != null)
                    m_AddComponentMethodPtr(data, entity, context);
            }

            public void RemoveComponentData(void* data, Entity entity, IDefinableContext context)
            {
                if (m_RemoveComponentMethodPtr != null)
                    m_RemoveComponentMethodPtr(data, entity, context);
            }

            public void InitializationView(void* data, IView view, Entity entity)
            {
                if (m_InitializationViewMethodPtr != null)
                    m_InitializationViewMethodPtr(data, view, entity);
            }
        }
        
        public static unsafe void AddComponentData(this IDef self, Entity entity, EntityManager manager,
            IDefinableContext context)
        {
            var data = manager.GetComponentDataRawRO(entity, self.GetTypeIndexDefinable());
            var cacheDef = GetConstructorDefinable(self); 
            cacheDef.FillDefinable(data);
            cacheDef.AddComponentData(data, entity, context);
        }

        private static ConstructorDefinable GetConstructorDefinable(IDef self)
        {
            if (m_Defs.TryGetValue(self.GetTypeIndexDefinable(), out var rec)) return rec;

            var methodAdd = ComponentType.FromTypeIndex(self.GetTypeIndexDefinable()).GetManagedType()
                .GetMethod(nameof(IDefinableCallback.AddComponentData));
            var methodRemove = ComponentType.FromTypeIndex(self.GetTypeIndexDefinable()).GetManagedType()
                .GetMethod(nameof(IDefinableCallback.RemoveComponentData));

            var methodView = ComponentType.FromTypeIndex(self.GetTypeIndexDefinable()).GetManagedType()
                .GetMethod(nameof(IDefinableCallback.InitializationView));


            var defType = typeof(RefLink<>).MakeGenericType(self.GetType());
            var link = defType
                .GetMethod("From", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)!
                .Invoke(null, new object[] {self});

            var methodInfo = ComponentType.FromTypeIndex(self.GetTypeIndexDefinable())
                .GetManagedType().GetMethod(nameof(IDefinable<IDef>.SetDef));

            rec = new ConstructorDefinable(methodInfo, methodAdd, methodRemove, methodView, link);
            m_Defs.TryAdd(self.GetTypeIndexDefinable(), rec);
            return rec;
        }
        
        public static unsafe void RemoveComponentData(this IDef self, Entity entity, EntityManager manager, IDefinableContext context)
        {
            var data = manager.GetComponentDataRawRO(entity, self.GetTypeIndexDefinable());
            
            var cacheDef = GetConstructorDefinable(self); 
            cacheDef.FillDefinable(data);
            cacheDef.RemoveComponentData(data, entity, context);
        }

        public static unsafe void InitializationView(this IDef self, IView view, Entity entity, EntityManager manager)
        {
            var data = manager.GetComponentDataRawRO(entity, self.GetTypeIndexDefinable());
            var cacheDef = GetConstructorDefinable(self); 
            cacheDef.InitializationView(data, view, entity);
        }
    }
}