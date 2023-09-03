using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class Entity : MonoBehaviour
    {
        public bool Enabled = true;
        public EntityType Type;
        public Dictionary<Type, Component> Components = new Dictionary<Type, Component>();

        private void Awake()
        {
            Component[] components = GetComponents<Component>();
            foreach (Component component in components)
            {
                Components[component.GetType()] = component;
            }
        }

        public T Get<T>() where T : Component
        {
            if (Components.TryGetValue(typeof(T), out Component component))
                return (T)component;

            return null;
        }
    }

    [RequireComponent(typeof(Entity))]
    public class EntityEXT : MonoBehaviour
    {
        public Entity Entity;
        public EntityType EType => Entity.Type;
        public bool Enabled => Entity.Enabled;

        private void Awake()
        {
            Entity = GetComponent<Entity>();
        }

        public T Get<T>() where T : Component
        {
            if (Entity.Components.TryGetValue(typeof(T), out Component component))
                return (T)component;

            return null;
        }
    }

    public enum EntityType : short
    {
        Player = 0,
        HumanAI = 1,
        AnimalAI = 2,
        ZombieAI = 3,
        BossAI = 4,
        Sttucture = 5,
        Spawner = 6,
        NPC = 7,
        Vehicle = 8,
        SatisfyArea = 9,
        WorldArea = 10,
        Pickup = 11
    }
}
