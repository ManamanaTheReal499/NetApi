﻿using ApiCore.Attributes;
using ApiCore.Com;
using System.Reflection;

namespace ApiCore.Models
{
    /// <summary>
    /// defines basic structure of a request; call <see cref="Extract"/> to reflect all properties with defined <see cref="PropertyTypeAttribute"/> into new <see cref="Request"/> object;
    /// </summary>
    public class RequestDefinition
    {
        /// <summary>
        /// extracts a <see cref="Request"/> object using the defined properties in this object;
        /// </summary>
        /// <returns>new <see cref="Request"/> containing all request information; run <see cref="Request.Submit(string)"/> to execute request</returns>
        public virtual Request Extract()
        {
            var request = new Request();
            foreach (var prop in this.GetType().GetProperties())
                prop.GetCustomAttributes(true)
                    .Where((x) => x.GetType() == typeof(PropertyTypeAttribute))
                    .ForEach((x) => Insert(x, request, prop));

            this.GetType().GetCustomAttributes(true)
                    .Where((x) => x.GetType() == typeof(RequestMethodAttribute))
                    .ForEach((x) => request.Method = ((RequestMethodAttribute)x).Method);

            return request;
        }

        /// <summary>
        /// inserts extracted properties into <see cref="Request"/> model
        /// </summary>
        /// <param name="x"></param>
        /// <param name="request"></param>
        /// <param name="prop"></param>
        private void Insert(object x, Request request, PropertyInfo prop)
        {
            var name = ((PropertyTypeAttribute)x).Name != null ? ((PropertyTypeAttribute)x).Name : prop.Name;

            switch (((PropertyTypeAttribute)x).Type)
            {
                case PropertyType.Header:
                    request.AppendHeader((name!, prop.GetValue(this, null)?.ToString()));
                    break;
                case PropertyType.Post:
                    request.AppendPost((name!, prop.GetValue(this, null)?.ToString()));
                    break;
                case PropertyType.Get:
                    request.AppendGet((name!, prop.GetValue(this, null)?.ToString()));
                    break;
                default:
                    break;
            }
        }
    }
}
