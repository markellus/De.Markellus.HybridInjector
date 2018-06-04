using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace De.Markellus.HybridInjector.Misc
{
    /// <summary>
    /// Stellt Hilfsfunktionen zur Verfügung, die als Basis für eine Klasse dienen können.
    /// </summary>
    public abstract class BaseClass : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Speichert Variablen einer Klasse zentral in einem Wörterbuch ab.
        /// </summary>
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        #endregion

        #region Properties

        /// <summary>
        /// Stellt ein, ob das Auslösen von Events erlaubt sein soll.
        /// </summary>
        protected internal bool AllowRaiseEvent { get; set; } = true;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event, welches ausgelöst wird, wenn sich eine Eigenschaft des Klassenobjektes ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Functions

        /// <summary>
        /// Zerstört die aufrufende Instanz dieser Klasse und alle angehängten Eigenschaften,
        /// sofern diese <see cref="IDisposable"/> implementieren.
        /// </summary>
        public void Dispose()
        {
            foreach (IDisposable obj in _values.Values.OfType<IDisposable>())
            {
                (obj).Dispose();
            }
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Ruft den Wert einer Eigenschaft dieser Klasse ab.
        /// </summary>
        /// <typeparam name="T">Klassentyp der festzulegenden Eigenschaft</typeparam>
        /// <param name="key">Der Name der Eigenschaft</param>
        /// <returns>Der Wert der Eigenschaft</returns>
        protected T GetValue<T>(string key)
        {
            object value = null;

            if (_values.ContainsKey(key))
            {
                value = _values[key];
            }

            if (value is T)
            {
                return (T)value;
            }

            return default(T);
        }

        /// <summary>
        /// Legt den Wert einer Eigenschaft dieser Klasse fest.
        /// Diese Aktion löst das <see cref="PropertyChanged"/>-Event aus.
        /// </summary>
        /// <param name="key">Der Name der Eigenschaft</param>
        /// <param name="value">Der Wert der Eigenschaft</param>
        protected virtual void SetValue(string key, object value)
        {
            object old = null;

            if (!_values.ContainsKey(key))
            {
                _values.Add(key, value);
            }
            else
            {
                old = _values[key];
                _values[key] = value;
            }

            if (old != value)
            {
                OnPropertyChanged(key, old, value);
            }

        }

        /// <summary>
        /// Ermittelt, ob eine angegebene Eigenschaft definiert ist.
        /// </summary>
        /// <param name="key">Der Name der Eigenschaft</param>
        /// <returns>TRUE, wenn die Eigenschaft definiert ist, ansonsten FALSE.</returns>
        protected bool ValueDefined(string key)
        {
            return _values.ContainsKey(key);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Löst das <see cref="PropertyChanged"/>-Event aus.
        /// </summary>
        /// <param name="propertyName">Der Name der geänderten Eigenschaft</param>
        /// <param name="oldValue">Der alte Wert</param>
        /// <param name="newValue">Der neue Wert</param>
        private void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (this.AllowRaiseEvent && !ReferenceEquals(PropertyChanged, null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
