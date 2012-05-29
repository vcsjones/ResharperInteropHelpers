using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace InteropHelpers.KnownImports
{
    public class Kernel32 : Library
    {
        protected internal override void Intialize()
        {
            AddDeclaration("AcquireSRWLockExclusive").
                AddParameter("SRWLock", UnmanagedType.LPStruct, Modifiers.In | Modifiers.Out);

            AddDeclaration("AcquireSRWLockShared").
                AddParameter("SRWLock", UnmanagedType.LPStruct, Modifiers.In | Modifiers.Out);

            AddDeclaration("ActivateActCtx", typeof(bool), UnmanagedType.Bool)
                .AddParameter("hActCtx", typeof(IntPtr), UnmanagedType.SysInt)
                .AddParameter("lpCookie", typeof(IntPtr), UnmanagedType.SysInt, Modifiers.Out);

            AddDeclaration("AddAtom", typeof(ushort), UnmanagedType.U2, isUnicodeConvention : true)
                .AddParameter("lpString", typeof(string), UnmanagedType.LPWStr);

            AddDeclaration("CloseHandle", typeof(bool), UnmanagedType.Bool)
                .AddParameter("hObject", typeof(IntPtr), UnmanagedType.SysInt);

        }
    }

    public abstract class Library : IEnumerable<Declaration>
    {
        private readonly Dictionary<string, Declaration> _declarations = new Dictionary<string, Declaration>();
        protected Declaration AddDeclaration(string name, UnmanagedType? returnType = null, CallingConvention callingConvention = CallingConvention.StdCall, bool isUnicodeConvention = false)
        {
            return AddDeclaration(name, typeof(void), returnType, callingConvention);
        }

        protected Declaration AddDeclaration(string name, Type returnCLRType, UnmanagedType? returnType = null, CallingConvention callingConvention = CallingConvention.StdCall, bool isUnicodeConvention = false)
        {
            var declaration = new Declaration(name, returnCLRType, returnType, callingConvention);
            _declarations.Add(name, declaration);
            return declaration;
        }

        protected internal abstract void Intialize();


        public IEnumerator<Declaration> GetEnumerator()
        {
            return _declarations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Declaration this[string name]
        {
            get { return _declarations[name]; }
        }
    }

    [Flags]
    public enum Modifiers
    {
        None = 0x0,
        In = 0x001,
        Out = 0x002,
        Optional = 0x004
    }

    public class Declaration
    {
        public string Name { get; private set; }
        public Type CLRReturnType { get; private set; }
        public UnmanagedType? ReturnType { get; private set; }
        public CallingConvention CallingConvention { get; private set; }
        private readonly List<Parameter> _parameters;
        public ReadOnlyCollection<Parameter> Parameters
        {
            get { return _parameters.AsReadOnly(); }
        }

        public Declaration(string name, Type clrReturnType, UnmanagedType? returnType, CallingConvention callingConvention)
        {
            Name = name;
            CLRReturnType = clrReturnType;
            ReturnType = returnType;
            CallingConvention = callingConvention;
            _parameters = new List<Parameter>();
        }

        public Declaration AddParameter(string name, Type clrType, UnmanagedType? unmanagedType = null, Modifiers modifiers = Modifiers.In)
        {
            _parameters.Add(new Parameter(name, clrType, unmanagedType, modifiers));
            return this;
        }

        public Declaration AddParameter(string name, UnmanagedType? unmanagedType = null, Modifiers modifiers = Modifiers.In)
        {
            _parameters.Add(new Parameter(name, unmanagedType, modifiers));
            return this;
        }
    }

    public class Parameter
    {
        public string Name { get; private set; }
        public Type CLRType { get; private set; }
        public UnmanagedType? UnmanagedType { get; private set; }
        public Modifiers Modifiers { get; set; }

        public Parameter(string name, Type type, UnmanagedType? unmanagedType, Modifiers modifiers = Modifiers.In)
        {
            Name = name;
            CLRType = type;
            UnmanagedType = unmanagedType;
            Modifiers = modifiers;
        }

        public Parameter(string name, UnmanagedType? unmanagedType = null, Modifiers modifiers = Modifiers.In)
            : this(name, typeof(object), unmanagedType, modifiers)
        {
        }
    }
}
