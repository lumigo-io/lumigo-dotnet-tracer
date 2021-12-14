using System;
using System.Collections.Generic;

namespace Lumigo.DotNET.Instrumentation
{
    public abstract class BaseFactory<TKey, TVal> : IFactory<TKey, TVal>
    {
        protected abstract Dictionary<TKey, Func<TVal>> Operations { get; }
        public TVal Default { get; set; }
        private Dictionary<TKey, Func<TVal>> _operations;

        protected BaseFactory(TVal defaultValue)
        {
            this._operations = this.Operations;
            this.Default = defaultValue;
        }

        public TVal GetInstace(TKey key)
        {
            if (Operations.ContainsKey(key))
                return Operations[key]();
            return Default;
        }
    }
}
