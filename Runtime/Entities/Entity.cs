using System;
namespace Geparate.Entities
{
    public struct Entity: IEquatable<Entity>
    {
        public ulong index{ get; internal set; }
        public uint version{ get; internal set; }
        public bool actived{ get; internal set; }

        public Entity(ulong index, uint version) => (this.index, this.version, this.actived) = (index, version, true);


        public bool Equals(Entity other) => index == other.index && version == other.version;

        public override bool Equals(object other) => other is Entity entity ? Equals(entity) : false;

        public override int GetHashCode() => index.GetHashCode();

        public override string ToString() => $"index:{index}, version:{version}";

        public static bool operator ==(Entity lhs, Entity rhs) => lhs.index == rhs.index && lhs.version == rhs.version;

        public static bool operator !=(Entity lhs, Entity rhs) => lhs.index != rhs.index || lhs.version != rhs.version;
    }
}
