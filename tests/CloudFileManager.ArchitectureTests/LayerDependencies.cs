using System.Reflection;
using System.Reflection.Emit;

// Inspect compiled dependencies, including fully-qualified calls invisible to a using-only check.
internal static class LayerDependencies
{
    internal sealed record Edge(Type Source, Type Target, string Location, MemberInfo? Member = null);
    private static readonly Dictionary<short, OpCode> Codes = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(OpCode)).Select(f => (OpCode)f.GetValue(null)!).ToDictionary(c => c.Value);
    internal static IEnumerable<Edge> Read(Type source)
    {
        var edges = new List<Edge>();
        void TypeRef(Type? type, string where, MemberInfo? member = null)
        {
            if (type is null) return;
            if (type.HasElementType) { TypeRef(type.GetElementType(), where, member); return; }
            if (type.IsGenericParameter) { foreach (var c in type.GetGenericParameterConstraints()) TypeRef(c, where, member); return; }
            edges.Add(new(source, type, where, member));
            if (type.IsGenericType) foreach (var a in type.GetGenericArguments()) TypeRef(a, where, member);
        }
        void MemberRef(MemberInfo member, string where)
        {
            TypeRef(member.DeclaringType, where, member);
            switch (member)
            {
                case Type t: TypeRef(t, where, member); break;
                case FieldInfo f: TypeRef(f.FieldType, where, member); break;
                case MethodBase m:
                    if (m is MethodInfo mi) TypeRef(mi.ReturnType, where, member);
                    foreach (var p in m.GetParameters()) TypeRef(p.ParameterType, where, member);
                    if (m.IsGenericMethod) foreach (var a in m.GetGenericArguments()) TypeRef(a, where, member);
                    break;
            }
        }
        void Attributes(IEnumerable<CustomAttributeData> attrs, string where)
        {
            foreach (var a in attrs)
            {
                TypeRef(a.AttributeType, where);
                foreach (var arg in a.ConstructorArguments) AttributeArg(arg, where);
                foreach (var arg in a.NamedArguments) AttributeArg(arg.TypedValue, where);
            }
        }
        void AttributeArg(CustomAttributeTypedArgument arg, string where)
        {
            TypeRef(arg.ArgumentType, where);
            if (arg.Value is Type t) TypeRef(t, where);
            if (arg.Value is IEnumerable<CustomAttributeTypedArgument> args) foreach (var item in args) AttributeArg(item, where);
        }
        TypeRef(source.BaseType, "base");
        foreach (var i in source.GetInterfaces()) TypeRef(i, "interface");
        if (source.IsGenericTypeDefinition) foreach (var g in source.GetGenericArguments()) TypeRef(g, "generic constraint");
        Attributes(source.GetCustomAttributesData(), "type attribute");
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach (var f in source.GetFields(flags)) { TypeRef(f.FieldType, "field:" + f.Name); Attributes(f.GetCustomAttributesData(), "field attribute"); }
        foreach (var p in source.GetProperties(flags)) TypeRef(p.PropertyType, "property:" + p.Name);
        foreach (var e in source.GetEvents(flags)) TypeRef(e.EventHandlerType, "event:" + e.Name);
        foreach (var method in source.GetMethods(flags).Cast<MethodBase>().Concat(source.GetConstructors(flags)))
        {
            MemberRef(method, "signature:" + method.Name); Attributes(method.GetCustomAttributesData(), "method attribute");
            foreach (var p in method.GetParameters()) Attributes(p.GetCustomAttributesData(), "parameter attribute");
            var body = method.GetMethodBody(); if (body is null) continue;
            foreach (var l in body.LocalVariables) TypeRef(l.LocalType, "local:" + method.Name);
            foreach (var clause in body.ExceptionHandlingClauses)
                if (clause.Flags == ExceptionHandlingClauseOptions.Clause) TypeRef(clause.CatchType, "catch:" + method.Name);
            var il = body.GetILAsByteArray()!;
            for (int offset = 0; offset < il.Length;)
            {
                short value = il[offset++]; if (value == 0xfe) value = (short)(0xfe00 | il[offset++]);
                var op = Codes[value]; var where = "IL:" + method.Name;
                switch (op.OperandType)
                {
                    case OperandType.InlineField: case OperandType.InlineMethod: case OperandType.InlineType: case OperandType.InlineTok:
                        var token = BitConverter.ToInt32(il, offset);
                        var member = method.Module.ResolveMember(token, source.IsGenericType ? source.GetGenericArguments() : null,
                            method.IsGenericMethod ? method.GetGenericArguments() : null);
                        if (member is null) throw new InvalidOperationException("Unresolved dependency token");
                        MemberRef(member, where); offset += 4; break;
                    case OperandType.InlineSig:
                        // Fail closed for calli/function-pointer signatures not handled by this small guard.
                        throw new InvalidOperationException("Unsupported InlineSig in " + source.FullName + "." + method.Name);
                    case OperandType.InlineSwitch: var count = BitConverter.ToInt32(il, offset); offset += 4 + 4 * count; break;
                    case OperandType.InlineI8: case OperandType.InlineR: offset += 8; break;
                    case OperandType.InlineBrTarget: case OperandType.InlineI: case OperandType.InlineString: case OperandType.ShortInlineR: offset += 4; break;
                    case OperandType.InlineVar: offset += 2; break;
                    case OperandType.ShortInlineBrTarget: case OperandType.ShortInlineI: case OperandType.ShortInlineVar: offset++; break;
                    case OperandType.InlineNone: break;
                    default: throw new InvalidOperationException("Unsupported IL operand " + op.OperandType);
                }
            }
        }
        return edges;
    }
}
