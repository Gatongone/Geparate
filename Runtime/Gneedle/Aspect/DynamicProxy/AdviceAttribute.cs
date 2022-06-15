using System;

namespace Geparate.Gneedle
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AspectAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class AdviceBeforeAttribute : Attribute
    {
        public string pointCut;

        public AdviceBeforeAttribute(string pointCut) => this.pointCut = pointCut;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AdviceAfterAttribute : Attribute
    {
        public string pointCut;

        public AdviceAfterAttribute(string pointCut) => this.pointCut = pointCut;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AdviceFailedAttribute : Attribute
    {
        public string pointCut;

        public AdviceFailedAttribute(string pointCut) => this.pointCut = pointCut;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AdviceFinishAttribute : Attribute
    {
        public string pointCut;

        public AdviceFinishAttribute(string pointCut) => this.pointCut = pointCut;
    }
}