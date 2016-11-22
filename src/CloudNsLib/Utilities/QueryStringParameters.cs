using System;
using System.Collections;
using System.Collections.Generic;

namespace CloudNsLib.Utilities
{
    internal class QueryStringParameters : IEnumerable<KeyValuePair<string, string[]>>
    {
        private readonly Dictionary<string, string[]> _internal;

        public QueryStringParameters()
        {
            _internal = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count => _internal.Count;

        public string this[string key]
        {
            get { return string.Join(",", _internal[key]); }
            set { _internal[key] = new[] { value }; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        public void Add(string key, string value)
        {
            string[] internalVal;
            if (!_internal.TryGetValue(key, out internalVal))
            {
                this[key] = value;
            }
            else
            {
                Array.Resize(ref internalVal, internalVal.Length + 1);
                internalVal[internalVal.Length - 1] = value;
                _internal[key] = internalVal;
            }
        }
    }
}