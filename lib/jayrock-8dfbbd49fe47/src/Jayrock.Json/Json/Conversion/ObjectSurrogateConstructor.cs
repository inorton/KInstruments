﻿#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;

    #endregion

    public class ObjectSurrogateConstructor : IObjectConstructor
    {
        private readonly Type _surrogateType;

        public ObjectSurrogateConstructor(Type surrogateType)
        {
            if (surrogateType == null) 
                throw new ArgumentNullException("surrogateType");

            if (!typeof(IObjectSurrogateConstructor).IsAssignableFrom(surrogateType))
            {
                throw new ArgumentException(string.Format(
                    "Surrogate type must implement {0} whereas {1} does not.",
                        typeof (IObjectSurrogateConstructor), surrogateType), 
                    "surrogateType");
            }

            _surrogateType = surrogateType;
        }

        public Type SurrogateType { get { return _surrogateType; } }

        public virtual ObjectConstructionResult CreateObject(ImportContext context, JsonReader reader)
        {
            if (context == null) 
                throw new ArgumentNullException("context");
            if (reader == null) 
                throw new ArgumentNullException("reader");

            IObjectSurrogateConstructor ctor = (IObjectSurrogateConstructor) context.Import(_surrogateType, reader);
            return ctor.CreateObject(context);
        }
    }
}
