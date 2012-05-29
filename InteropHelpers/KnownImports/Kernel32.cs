﻿using System;
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
            AddDeclaration("CloseHandle", typeof (bool))
                .AddParameter("hObject", typeof (IntPtr), UnmanagedType.SysInt);
        }
    }

    public abstract class Library : IEnumerable<Declaration>
    {
        private readonly Dictionary<string, Declaration> _declarations = new Dictionary<string, Declaration>();
        protected Declaration AddDeclaration(string name, Modifiers modifiers = Modifiers.In, CallingConvention callingConvention = CallingConvention.StdCall)
        {
            return AddDeclaration(name, typeof (void), modifiers, callingConvention);
        }

        protected Declaration AddDeclaration(string name, Type returnType, Modifiers modifiers = Modifiers.In, CallingConvention callingConvention = CallingConvention.StdCall)
        {
            var declaration = new Declaration(name, returnType, modifiers, callingConvention);
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
        public Modifiers Modifiers { get; private set; }
        public CallingConvention CallingConvention { get; private set; }
        private readonly List<Parameter> _parameters;
        public ReadOnlyCollection<Parameter> Parameters
        {
            get { return _parameters.AsReadOnly(); }
        }

        public Declaration(string name, Type clrReturnType, Modifiers modifiers, CallingConvention callingConvention)
        {
            Name = name;
            CLRReturnType = clrReturnType;
            Modifiers = modifiers;
            CallingConvention = callingConvention;
            _parameters = new List<Parameter>();
        }

        public Declaration AddParameter(string name, Type clrType, UnmanagedType? unmanagedType = null)
        {
            _parameters.Add(new Parameter(name, clrType, unmanagedType));
            return this;
        }

        public Declaration AddParameter(string name, UnmanagedType? unmanagedType = null)
        {
            _parameters.Add(new Parameter(name));
            return this;
        }
    }

    public class Parameter
    {
        public string Name { get; private set; }
        public Type CLRType { get; private set; }
        public UnmanagedType? UnmanagedType { get; private set; }

        public Parameter(string name, Type type, UnmanagedType? unmanagedType)
        {
            Name = name;
            CLRType = type;
            UnmanagedType = unmanagedType;
        }

        public Parameter(string name, UnmanagedType? unmanagedType = null) : this(name, typeof(object), unmanagedType)
        {
        }
    }
}
