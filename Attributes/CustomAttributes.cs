using System;

namespace Ordem_Servicos_Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MonetarioAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class QuantidadeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class NumerosAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class MinusculoAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class MaiusculoAttribute : Attribute { }
}