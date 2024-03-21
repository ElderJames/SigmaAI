
using System.ComponentModel;

namespace LLMJson
{
    public enum UpdateStates
    {
        Unchanged,
        InvalidUpdate,
        Updated
    };

    public class JsonProp<T> 
    {
        private T _value;


        public string Description           { get; set; } = "";
        public UpdateStates UpdateState     { get; set; } = UpdateStates.Unchanged;
        public bool Visible                 { get; set; } = true;
        public bool Immutable               { get; set; } = false;
        public bool HasSetter              { get; private set; } = false;
        private readonly Func<string, Tuple<T,bool>>? _valueSetter; 


        public T Value {
            get {                return _value;  }
            set { if(!Immutable) _value = value; }
        }

        public JsonProp(T value, string description= "", bool visible = true, bool immutable =false, Func<string, Tuple<T, bool>>? valueSetter = null)
        {
            _value       = value;
            Description  = description;
            Visible      = visible;  
            Immutable    = immutable;
            _valueSetter = valueSetter;
            HasSetter    = valueSetter != null;
        }

       
        public override string ToString()    { try { return Value?.ToString() ?? ""; }catch {return "";} }

        public bool FromString(string value)
        {
            try
            {
                if (_valueSetter != null)
                {
                    var result = _valueSetter(value);
                    if (result.Item2)
                    {
                        _value = result.Item1;
                        return true;
                    }
                } 
            } catch { return false; }
            return false;
        }


        public static implicit operator T(JsonProp<T> instance)
       {
            //if (instance._value == null) { return default(T)! ; }
            return instance._value;
        }
    }
}

