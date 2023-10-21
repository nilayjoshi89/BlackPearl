using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Prism.Mvvm;
using Prism.Regions;

namespace BlackPearl.PrismUI
{
    public class BlackPearlViewModel : BindableBase, IDisposable, INavigationAware, IRegionMemberLifetime, INotifyDataErrorInfo
    {
        #region Members
        private readonly IEnumerable<string> availableProperties;
        private ConcurrentDictionary<string, List<string>> propertyErrors = new ConcurrentDictionary<string, List<string>>();
        #endregion

        #region Constructor
        public BlackPearlViewModel()
        {
            availableProperties = GetType()
                                    .GetProperties()
                                    .Where(p => p.Name != nameof(HasErrors))
                                    .Select(p => p.Name);
            PropertyChanged += ViewModel_PropertyChanged;
        }
        #endregion

        #region Methods
        public virtual Task OnLoad() => Task.CompletedTask;
        public virtual Task OnUnload() => Task.CompletedTask;
        protected virtual IEnumerable<string> GetValidationErrors(string propertyName) => null;

        protected void AddValidations(string propertyName, IEnumerable<string> errors)
        {
            propertyErrors?.AddOrUpdate(propertyName, errors.ToList(),
               (key, oldErrors) => errors.Union(oldErrors).Distinct().ToList());

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        protected void Validate(string propertyName)
        {
            if (disposedValue || string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            ClearPropertyError(propertyName);
            IEnumerable<string> errors = null;

            try
            {
                errors = GetValidationErrors(propertyName);
            }
            catch { }

            if (errors?.Any() != true)
            {
                return;
            }

            propertyErrors?.AddOrUpdate(propertyName, errors.ToList(),
                (key, oldErrors) => errors.Union(oldErrors).Distinct().ToList());

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        protected void ValidateAll()
        {
            if (disposedValue)
            {
                return;
            }

            foreach (string p in availableProperties)
            {
                Validate(p);
            }
        }
        protected void ClearValidation(string propertyName)
        {
            if (disposedValue)
            {
                return;
            }

            ClearPropertyError(propertyName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearPropertyError(string propertyName)
        {
            if (propertyErrors?.ContainsKey(propertyName) != true)
            {
                return;
            }

            propertyErrors[propertyName]?.Clear();
        }
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) => Validate(e?.PropertyName);
        #endregion

        public virtual bool KeepAlive => false;

        #region INotifyDataErrorInfo
        public bool HasErrors => propertyErrors?.Any(pe => pe.Value?.Any() == true) == true;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable GetErrors(string propertyName)
        => (!disposedValue && !string.IsNullOrEmpty(propertyName) && propertyErrors?.ContainsKey(propertyName) == true)
                ? propertyErrors[propertyName]
                : (IEnumerable)new List<string>(0);
        #endregion

        #region INavigationAware

        public virtual void OnNavigatedTo(NavigationContext navigationContext) { }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => !disposedValue;
        public virtual void OnNavigatedFrom(NavigationContext navigationContext) { }
        #endregion

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }

            if (disposing)
            {
                PropertyChanged -= ViewModel_PropertyChanged;
                propertyErrors = null;
            }

            disposedValue = true;
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}