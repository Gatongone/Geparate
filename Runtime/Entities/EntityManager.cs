using System.Collections.Generic;
namespace Geparate.Entities
{
    public class EntityManager
    {
        private ulong m_MaxIndex;
        private Stack<Entity> m_RecycleEntityBuffer = new Stack<Entity>();
        public Entity CreateEntity()
        {
            if(m_RecycleEntityBuffer.Count == 0)
                return new Entity(++m_MaxIndex, 1);
            var entity = m_RecycleEntityBuffer.Pop();
            entity.version++;
            return entity;
        }
        public void RecycleEntity(Entity entity)
        {
            m_RecycleEntityBuffer.Push(entity);
        }
    }
}
