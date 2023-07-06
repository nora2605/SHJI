using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHJI.Interpreter
{
    internal class JaneEnvironment
    {
        public Dictionary<string, IJaneObject> Store { get; set; }
        public JaneEnvironment()
        {
            Store = new();
        }
        public IJaneObject Get(string key)
        {
            if (Store.TryGetValue(key, out IJaneObject? obj)) return obj;
            return IJaneObject.JANE_UNINITIALIZED;
        }

        public IJaneObject Set(string key, IJaneObject value)
        {
            Store[key] = value;
            return value;
        }

        public bool Has(string key)
        {
            return Store.ContainsKey(key);
        }

        public IJaneObject this[string key] { get => Get(key); set => Set(key, value); }
    }
}
