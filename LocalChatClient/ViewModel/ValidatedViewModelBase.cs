using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace LocalChatClient.ViewModel
{
    public class ValidatedViewModelBase : ViewModelBase, IDataErrorInfo
    {
        #region IDataErrorInfo

        private readonly Dictionary<string, Func<string>> validationRules = new Dictionary<string, Func<string>>();
        private readonly Dictionary<string, bool> propertiesValidity = new Dictionary<string, bool>();

        protected void AddValidationRule(string property, Func<string> rule)
        {
            if (validationRules.ContainsKey(property))
                return;
            validationRules.Add(property, rule);
            propertiesValidity.Add(property, false);
        }

        protected void AddValidationRule<T>(Expression<Func<T>> propertyExpression, Func<string> rule)
        {
            string propertyName = GetPropertyName(propertyExpression);
            if (validationRules.ContainsKey(propertyName))
                return;
            validationRules.Add(propertyName, rule);
            propertiesValidity.Add(propertyName, false);
        }

        public void Validate()
        {
            foreach (var property in validationRules.Keys)
            {
                _ = this[property];
            }
        }

        public event EventHandler<ValidityChangedEventArgs> ValidityChanged = delegate { };

        private bool isValid;
        public bool IsValid
        {
            get => isValid;
            set
            {
                if(Set(ref isValid, value))
                {
                    ValidityChanged(this, new ValidityChangedEventArgs(value));
                }
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null; 
                if (propertiesValidity.ContainsKey(columnName))
                {
                    result = validationRules[columnName].Invoke();
                    propertiesValidity[columnName] = result == null;
                }
                IsValid = !propertiesValidity.Values.Contains(false);
                return result;
            }
        }

        public string Error
        {
            get => null;
        }

        #endregion
    }

    public class ValidityChangedEventArgs : EventArgs
    {
        public ValidityChangedEventArgs(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; }
    }
}
